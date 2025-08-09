using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa.Utility
{
    public class PointOfSale
    {
        public void InitializeComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add(new ComboBoxItem { Value = 0, Description = "Ninguno" });
            comboBox.Items.Add(new ComboBoxItem { Value = 10, Description = "10% off" });
            comboBox.Items.Add(new ComboBoxItem { Value = 15, Description = "15% off" });
            comboBox.Items.Add(new ComboBoxItem { Value = 30, Description = "30% off" });
            comboBox.Items.Add(new ComboBoxItem { Value = 50, Description = "50% off" });
            comboBox.SelectedIndex = 0;
        }

        // Calcula descuento -> actualiza labels
        public void CalculateDiscount(string subtotalText, object selectedItem, Label labelDiscount, Label labelTotalAfterDiscount)
        {
            decimal subtotal = ParseDec(subtotalText);
            decimal discountAmount = 0m;

            if (selectedItem is ComboBoxItem opt)
            {
                decimal pct = (decimal)opt.Value / 100m;
                discountAmount = Math.Round(subtotal * pct, 2, MidpointRounding.AwayFromZero);
            }

            decimal totalAfter = subtotal - discountAmount;
            labelDiscount.Text = discountAmount.ToString(CultureInfo.InvariantCulture);
            labelTotalAfterDiscount.Text = totalAfter.ToString(CultureInfo.InvariantCulture);
        }

        // Calcula cambio -> actualiza label
        public void CalculateChange(Label totalLabel, TextBox paidTextBox, Label changeLabel)
        {
            decimal total = ParseDec(totalLabel.Text);
            decimal paid = ParseDec(paidTextBox.Text);

            decimal change = paid - total;
            if (change < 0) change = 0;
            changeLabel.Text = change.ToString(CultureInfo.InvariantCulture);
        }

        // Procesa la transacción (Google Sheets) - ASYNC
        // Nota: listBox no es fuente de verdad; TransactionManager lee el Cart en Sheets.
        public async Task<bool> ProcessTransactionAsync(
            string subtotalText,
            string cashText,
            object selectedItem,
            string transactionId,
            ListBox _ /* ignorado */
        )
        {
            try
            {
                decimal subtotal = ParseDec(subtotalText);
                decimal cash = ParseDec(cashText);

                double discountPercent = 0;
                if (selectedItem is ComboBoxItem opt)
                    discountPercent = opt.Value;

                decimal discountAmount = Math.Round(subtotal * (decimal)(discountPercent / 100.0), 2, MidpointRounding.AwayFromZero);
                decimal total = subtotal - discountAmount;

                if (cash < total)
                {
                    MessageBox.Show("Not enough cash to complete the transaction.");
                    return false;
                }

                decimal change = cash - total;

                var tm = new TransactionManager();
                await tm.SaveTransactionAsync(
                    transactionId,
                    subtotal,                     // decimal
                    cash,                         // decimal
                    discountPercent,              // double
                    (double)discountAmount,       // double
                    (double)change,               // double
                    DateTime.Now,
                    (double)total                 // double
                );

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing transaction:\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ---- helpers ----
        private static decimal ParseDec(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0m;
            s = s.Replace("$", "").Trim();
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d)) return d;
            return 0m;
        }
    }

    public sealed class ComboBoxItem
    {
        public double Value { get; set; }
        public string Description { get; set; }
        public override string ToString() => Description;
    }
}
