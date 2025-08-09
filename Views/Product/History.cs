using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa.InventoryApp.dlg
{
    public partial class History : Form
    {
        private readonly int productId;

        public History(int id)
        {
            InitializeComponent();
            productId = id;

            // Cargar cuando el form ya está visible (para poder usar await)
            this.Shown -= History_Shown;
            this.Shown += History_Shown;
        }

        private async void History_Shown(object sender, EventArgs e)
        {
            await DisplayHistoryAsync();
        }

        // FETCH DATA FROM HISTORY (Google Sheets)
        private async Task DisplayHistoryAsync()
        {
            try
            {
                // Leemos toda la hoja History
                DataTable all = await SheetsRepo.ReadTableAsync("History");

                // Filtramos por ProductID
                DataTable dt = all.Clone();
                foreach (DataRow r in all.Rows)
                {
                    // ProductID puede venir como string; hacemos parse seguro
                    int pid = 0;
                    int.TryParse(r["ProductID"]?.ToString(), out pid);
                    if (pid == productId)
                    {
                        dt.Rows.Add(r.ItemArray);
                    }
                }

                // (Opcional) ordenar por fecha si la columna Date es ISO (YYYY-MM-DD HH:MM:SS)
                // dt.DefaultView.Sort = "[Date] DESC";
                // dt = dt.DefaultView.ToTable();

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar el historial:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
