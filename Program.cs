using RapiMesa.Utility;
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

            Application.ThreadException += (s, e) => HandleException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => HandleException(e.ExceptionObject as Exception);

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
                HandleException(ex);
                return;
            }
            SyncQueue.Start();
            BackupService.Start();

            Application.Run(new UserAuth());
        }

        private static void HandleException(Exception ex)
        {
            if (ex == null) return;
            ErrorLogger.Log(ex);
            MessageBox.Show(
                $"Ocurrió un error inesperado:\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
