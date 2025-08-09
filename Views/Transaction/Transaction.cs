using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;
using RapiMesa.Views.Transaction;

namespace RapiMesa.Views.Transaction
{
    public partial class Transaction : Form
    {
        public Transaction()
        {
            InitializeComponent();

            this.Shown -= Transaction_Shown;
            this.Shown += Transaction_Shown;
        }

        private async void Transaction_Shown(object sender, EventArgs e)
        {
            await DisplayTransactionAsync();
        }

        // FETCH DATA FROM "Transaction" SHEET
        private async Task DisplayTransactionAsync()
        {
            try
            {
                int currentUID = UserSession.SessionUID;

                DataTable all = await SheetsRepo.ReadTableAsync("Transaction"); // columnas: TransactionId, Subtotal, Cash, DiscountPercent, DiscountAmount, ChangeAmt, Total, Date, Uid

                // Filtrar por Uid
                var dt = all.Clone();
                foreach (DataRow r in all.Rows)
                {
                    if (int.TryParse(r["Uid"]?.ToString(), out var uid) && uid == currentUID)
                    {
                        dt.Rows.Add(r.ItemArray);
                    }
                }

                // Reordenar/renombrar columnas para que coincida con tu grid original
                var view = dt.DefaultView;
                // Si quieres orden por fecha desc:
                // view.Sort = "Date DESC";
                var show = view.ToTable(false,
                    "Date",
                    "Subtotal",
                    "DiscountPercent",
                    "DiscountAmount",
                    "Total",
                    "ChangeAmt",       // renombramos en el grid para mostrar "Change"
                    "TransactionId"
                );

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = show;

                // Opcional: renombrar encabezado "ChangeAmt" -> "Change"
                if (dataGridView1.Columns["ChangeAmt"] != null)
                    dataGridView1.Columns["ChangeAmt"].HeaderText = "Change";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transactions:\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // CELL DOUBLE CLICK: abrir detalles
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string id = dataGridView1.SelectedRows[0].Cells["TransactionId"].Value?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(id))
                {
                    using (var dlg = new Details(id))
                    {
                        dlg.ShowDialog();
                    }
                }
            }
        }
    }
}
