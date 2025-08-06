using System;
using System.Windows.Forms;

namespace InventoryApp.Views.Dashboard
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            LoadStats();
        }

        private void LoadStats()
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();

            cmd.CommandText = "SELECT IFNULL(SUM(Total),0) FROM \"Transaction\"";
            var revenue = cmd.ExecuteScalar();
            label2.Text = Convert.ToDecimal(revenue ?? 0).ToString();

            cmd.CommandText = "SELECT COUNT(*) FROM Product";
            label3.Text = Convert.ToInt32(cmd.ExecuteScalar() ?? 0).ToString();

            cmd.CommandText = "SELECT COUNT(*) FROM Category";
            label5.Text = Convert.ToInt32(cmd.ExecuteScalar() ?? 0).ToString();

            cmd.CommandText = "SELECT COUNT(*) FROM \"Transaction\"";
            label7.Text = Convert.ToInt32(cmd.ExecuteScalar() ?? 0).ToString();
        }
    }
}
