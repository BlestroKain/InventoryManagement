using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Utility;
using RapiMesa.Data;

namespace RapiMesa
{
    public partial class UserDialog : Form
    {
        private readonly AccountManager accountManager;
        private readonly int uid; // 0 = nuevo

        public UserDialog(AccountManager manager)
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            accountManager = manager ?? throw new ArgumentNullException(nameof(manager));
            uid = 0;
            Text = "Agregar Usuario";
            comboBox1.DataSource = Enum.GetValues(typeof(UserRole));
        }

        public UserDialog(AccountManager manager, int uid, string username, UserRole role)
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            accountManager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.uid = uid;
            Text = "Editar Usuario";
            textBox1.Text = username;
            comboBox1.DataSource = Enum.GetValues(typeof(UserRole));
            comboBox1.SelectedItem = role;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await SaveUserAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            Close();
        }

        private async Task<bool> ValidateAllAsync()
        {
            errorProvider1.Clear();
            var name = (textBox1.Text ?? "").Trim();
            var pass = textBox2.Text ?? "";

            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider1.SetError(textBox1, "El nombre es obligatorio.");
                return false;
            }

            if (uid == 0 && string.IsNullOrWhiteSpace(pass))
            {
                errorProvider1.SetError(textBox2, "La contraseña es obligatoria.");
                return false;
            }

            var dt = await accountManager.GetAccountsAsync();
            foreach (DataRow row in dt.Rows)
            {
                var existing = row["Username"]?.ToString();
                int existingUid = Convert.ToInt32(row["Uid"]);
                if (string.Equals(existing, name, StringComparison.OrdinalIgnoreCase) && existingUid != uid)
                {
                    errorProvider1.SetError(textBox1, "El usuario ya existe.");
                    return false;
                }
            }
            return true;
        }

        private async Task SaveUserAsync()
        {
            if (!await ValidateAllAsync()) return;

            string username = textBox1.Text.Trim();
            string password = textBox2.Text;
            var role = (UserRole)comboBox1.SelectedItem;

            try
            {
                if (uid == 0)
                    await accountManager.RegisterUserAsync(username, password, role);
                else
                    await accountManager.UpdateUserAsync(uid, username, password, role);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar usuario:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
