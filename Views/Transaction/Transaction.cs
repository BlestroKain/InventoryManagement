using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace InventoryApp.InventoryApp.dlg
{
    public partial class Transaction : Form
    {
        public Transaction()
        {
            InitializeComponent();
            DisplayTransaction();
        }

        // FETCH DATA FROM "Transaction" TABLE
        private void DisplayTransaction()
        {
            int currentUID = UserSession.SessionUID;

            using (SqliteConnection con = ConnectionManager.GetConnection())
            {
                using (SqliteCommand cmd = new SqliteCommand(
                    @"SELECT 
                        Date,
                        Subtotal,
                        DiscountPercent,
                        DiscountAmount,
                        Total,
                        ChangeAmt AS [Change],
                        TransactionId
                      FROM ""Transaction""
                     WHERE Uid = @Uid", con))
                {
                    cmd.Parameters.AddWithValue("@Uid", currentUID);

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }

        // CELL DOUBLE CLICK: abrir detalles
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string id = dataGridView1.SelectedRows[0]
                              .Cells["TransactionId"].Value.ToString();
                using (var dlg = new Details(id))
                {
                    dlg.ShowDialog();
                }
            }
        }
    }
}
