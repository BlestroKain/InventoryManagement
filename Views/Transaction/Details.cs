using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa.Views.Transaction
{
    public partial class Details : Form
    {
        private readonly string _transactionId;

        public Details(string id)
        {
            InitializeComponent();
            _transactionId = id ?? "";

            // Cargar cuando el form ya está visible (para poder usar await sin congelar la UI)
            this.Shown -= Details_Shown;
            this.Shown += Details_Shown;
        }

        private async void Details_Shown(object sender, EventArgs e)
        {
            await DisplayTransactionItemsAsync(_transactionId);
        }

        // FETCH DATA FROM Orders (Sheets)
        private async Task DisplayTransactionItemsAsync(string transactionId)
        {
            try
            {
                // Leemos toda la hoja Orders
                DataTable all = await SheetsRepo.ReadTableAsync("Orders");

                // Filtramos por TransactionId
                DataTable dt = all.Clone();
                foreach (DataRow r in all.Rows)
                {
                    if (string.Equals(r["TransactionId"]?.ToString(), transactionId, StringComparison.OrdinalIgnoreCase))
                    {
                        dt.Rows.Add(r.ItemArray);
                    }
                }

                // Si quieres controlar qué columnas mostrar (y orden):
                if (dt.Columns.Contains("Name") && dt.Columns.Contains("Price") && dt.Columns.Contains("Quantity"))
                {
                    var view = dt.DefaultView;
                    var show = view.ToTable(false, "Name", "Price", "Quantity");
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = show;
                }
                else
                {
                    // Por si los encabezados están distintos, mostramos todo
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading order details:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
