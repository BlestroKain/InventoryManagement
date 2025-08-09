using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;
using RapiMesa.InventoryApp.dlg;
using RapiMesa.InventoryApp.Views;
using RapiMesa.Views;
using RapiMesa.Views.Dashboard;
using RapiMesa.Views.Transaction;


namespace RapiMesa
{
    public partial class MainView : Form
    {
        private Form currentForm;
        private readonly CartManager cartManager;


        public MainView(string username)
        {
            InitializeComponent();

            cartManager = new CartManager();

            // Cargar Dashboard de entrada
            SwitchForm(new Dashboard());

            button1.Text = "Logout (" + username + ")";

            // Iniciar contador cuando el form ya está visible (permite await sin congelar)
            this.Shown -= MainView_Shown;
            this.Shown += MainView_Shown;

            // Timer
            itemCountTimer = new Timer { Interval = 1500 }; // 1.5s, ajusta a gusto
            itemCountTimer.Tick += async (s, e) => await RefreshCartCountAsync();
        }

        private async void MainView_Shown(object sender, EventArgs e)
        {
            await RefreshCartCountAsync(); // primer refresh inmediato
            itemCountTimer.Start();
        }

        //NAVIGATION CONTROL
        private void SwitchForm(Form newForm)
        {
            currentForm?.Hide();
            newForm.TopLevel = false;
            newForm.FormBorderStyle = FormBorderStyle.None;
            newForm.Dock = DockStyle.Fill;

            if (panel2.Controls.Count > 0)
            {
                var currentFormControl = panel2.Controls[0];
                currentFormControl.Hide();
                panel2.Controls.Remove(currentFormControl);
            }

            panel2.Controls.Add(newForm);
            newForm.Show();
            currentForm = newForm;
        }

        // DASHBOARD
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
                SwitchForm(new Dashboard());
        }

        // HOME TAB
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                SwitchForm(new Product());
        }

        // CATEGORY TAB
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                SwitchForm(new Category());
        }

        // CART TAB
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                SwitchForm(new Sale());
        }

        // TRANSACTION TAB
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
                SwitchForm(new Transaction());
        }

        // LOGOUT BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "Are you sure want to logout?",
                    "Warning!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var userauth = new UserAuth();
                userauth.FormClosed += (s, args) => this.Close();
                userauth.Show();
                Hide();
            }
        }

        // CART COUNTER (async)
        private async Task RefreshCartCountAsync()
        {
            try
            {
                int cartItemCount = await cartManager.GetCartItemCountAsync();
                radioButton3.Text = $"Sale ({cartItemCount})";
            }
            catch
            {
                // opcional: silenciar errores de red aquí para no molestar al usuario
            }
        }
        private async void itemCountTimer_Tick(object sender, EventArgs e)
        {
            await RefreshCartCountAsync();
        }

    }
}
