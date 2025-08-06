using System.Data;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace InventoryApp.InventoryApp.dlg
{
    public partial class Details : Form
    {
        public Details(string id)
        {
            InitializeComponent();
            DisplayTransactionItems(id);
        }

        // FETCH DATA FROM ORDERS TABLE
        private void DisplayTransactionItems(string transactionId)
        {
            using (SqliteConnection con = ConnectionManager.GetConnection())
            {
                using (SqliteCommand cmd = new SqliteCommand(
                    "SELECT Name, Price, Quantity FROM Orders WHERE TransactionId = @transactionId", con))
                {
                    cmd.Parameters.AddWithValue("@transactionId", transactionId);

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }
    }
}
