using System.Data;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace InventoryApp.InventoryApp.dlg
{
    public partial class History : Form
    {
        private readonly int productId;

        public History(int id)
        {
            InitializeComponent();
            productId = id;
            DisplayHistory();
        }

        // FETCH DATA FROM HISTORY TABLE
        private void DisplayHistory()
        {
            using (SqliteConnection con = ConnectionManager.GetConnection())
            {
                // la conexión ya viene abierta desde ConnectionManager
                using (SqliteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT ProductID,
                               [Added Stocks],
                               [Date]
                          FROM History
                         WHERE ProductID = @id";
                    cmd.Parameters.AddWithValue("@id", productId);

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
