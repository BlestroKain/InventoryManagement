using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;
using RapiMesa.InventoryApp;

namespace RapiMesa.Views
{
    public partial class UserAuth : Form
    {
        private bool isRegisterMode = false;
        private readonly AccountManager accountManager;

        public UserAuth()
        {
            InitializeComponent();
            accountManager = new AccountManager();
        }

        // === LOGIN / REGISTER ===

        // Validate Users Credentials (async)
        private async Task<int> ValidateUserCredentialsAsync(string username, string password)
        {
            return await accountManager.ValidateUserCredentialsAsync(username, password);
        }

        // Register new user (async)
        private async Task RegisterUserAsync(string username, string password)
        {
            await accountManager.RegisterUserAsync(username, password);
        }

        // Process Login/Register form (async)
        private async Task ProcessLoginFormAsync()
        {
            errorProvider1.Clear();

            string username = textBox1.Text?.Trim() ?? "";
            string password = textBox2.Text ?? "";

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorProvider1.SetError(textBox1, "Ingrese un nombre de usuario.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                errorProvider1.SetError(textBox2, "Ingrese una contraseña.");
                isValid = false;
            }

            if (!isValid) return;

            try
            {
                if (isRegisterMode)
                {
                    await RegisterUserAsync(username, password);
                }
                else
                {
                    int uid = await ValidateUserCredentialsAsync(username, password);
                    if (uid > 0)
                    {
                        UserSession.SessionUID = uid;
                        var mainpage = new MainView(username);
                        mainpage.FormClosed += (s, args) => this.Close();
                        mainpage.Show();
                        Hide();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Usuario o contraseña inválidos. Intenta de nuevo.",
                            "Error de inicio de sesión",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error de autenticación:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // === UI Handlers ===

        // Toggle label mode
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            isRegisterMode = !isRegisterMode; // Toggle the mode

            errorProvider1.Clear();
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox1.Enabled = true;
            textBox2.Enabled = true;

            if (isRegisterMode)
            {
                linkLabel1.Text = "INICIAR SESIÓN";
                button1.Text = "REGISTRARSE";
            }
            else
            {
                linkLabel1.Text = "REGISTRARSE";
                button1.Text = "INICIAR SESIÓN";
            }
        }

        // Login or Register Button
        private async void button1_Click(object sender, EventArgs e)
        {
            await ProcessLoginFormAsync();
        }

        // Show password checkbox
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        //TextBox key press event
        #region
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && e.KeyChar == (char)Keys.Enter)
            {
                textBox2.Focus();
                e.Handled = true;
            }
        }

        private async void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text) && e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                await ProcessLoginFormAsync();
            }
        }
        #endregion
    }
}
