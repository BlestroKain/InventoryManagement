using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa.Views.Dashboard
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();

            this.Shown -= Dashboard_Shown;
            this.Shown += Dashboard_Shown;
        }

        private async void Dashboard_Shown(object sender, EventArgs e)
        {
            await LoadStatsAsync();
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                // Transaction
                var tDt = await SheetsRepo.ReadTableAsync("Transaction");
                decimal revenue = 0m;
                foreach (DataRow r in tDt.Rows)
                {
                    revenue += ParseDecimal(r["Total"]);
                }
                label2.Text = revenue.ToString(CultureInfo.InvariantCulture); // total revenue

                // Product
                var pDt = await SheetsRepo.ReadTableAsync("Product");
                label3.Text = pDt.Rows.Count.ToString();

                // Category
                var cDt = await SheetsRepo.ReadTableAsync("Category");
                label5.Text = cDt.Rows.Count.ToString();

                // Count of Transactions
                label7.Text = tDt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard:\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Acepta "$1,234.50", "1.234,50", "1234.50", etc.
        private static decimal ParseDecimal(object v)
        {
            if (v == null) return 0m;
            var s = v.ToString().Trim();
            if (string.IsNullOrEmpty(s)) return 0m;

            // limpiar símbolos comunes
            s = s.Replace("$", "").Replace("€", "").Replace("£", "").Replace(",", "").Trim();

            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                return d;

            // último intento con cultura local
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                return d;

            return 0m;
        }
    }
}
