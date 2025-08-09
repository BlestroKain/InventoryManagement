using RapiMesa.Data;
using RapiMesa.Views;
using System;
using System.Windows.Forms;

namespace RapiMesa
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Inicializar conexión a Google Sheets
                ConnectionManager.Init(
                    "client_secret.json",
                    "1bW47Pi4_UOG8gUfpjvkGpwiHgTLdis6hydf5jf1_5b8"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al conectar con Google Sheets:\n{ex.Message}",
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            Application.Run(new UserAuth());
        }
    }
}
