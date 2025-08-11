using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Utility;
using RapiMesa.Data;
using RapiMesa;

namespace RapiMesa.InventoryApp.Views
{
    public partial class Accounts : Form
    {
        private readonly AccountManager accountManager;

        public Accounts()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            accountManager = new AccountManager();

            this.Shown -= Accounts_Shown;
            this.Shown += Accounts_Shown;
        }

        private async void Accounts_Shown(object sender, EventArgs e)
        {
            await RefreshGridAsync();
        }

        private async Task RefreshGridAsync()
        {
            var dt = await accountManager.GetAccountsAsync();
            if (dt.Columns.Contains("Password"))
                dt.Columns.Remove("Password");
            dataGridView1.DataSource = dt;
        }

        private async void AddBtn_Click(object sender, EventArgs e)
        {
            using (var dlg = new UserDialog(accountManager))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    await RefreshGridAsync();
            }
        }

        private async void EditBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario para editar.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var drv = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null) return;

            int uid = Convert.ToInt32(drv["Uid"]);
            string username = drv["Username"]?.ToString() ?? "";
            Enum.TryParse<UserRole>(drv["Role"]?.ToString(), out var role);

            using (var dlg = new UserDialog(accountManager, uid, username, role))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    await RefreshGridAsync();
            }
        }

        private async void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario para eliminar.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var drv = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null) return;
            int uid = Convert.ToInt32(drv["Uid"]);

            if (MessageBox.Show("¿Está seguro de eliminar esta cuenta?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await accountManager.DeleteUserAsync(uid);
                await RefreshGridAsync();
            }
        }
    }
}
