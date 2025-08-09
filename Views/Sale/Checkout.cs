using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;
using RapiMesa.Utility;

namespace RapiMesa
{
    public partial class Checkout : Form
    {
        private readonly PointOfSale pointOfSale;
        private readonly CartManager cartManager;
        private readonly decimal subtotal;

        public Checkout(decimal totalPrice)
        {
            InitializeComponent();

            pointOfSale = new PointOfSale();
            cartManager = new CartManager();
            subtotal = totalPrice;

            SubtotalLbl.Text = subtotal.ToString();

            // Inicializa descuentos y calcula totales iniciales
            pointOfSale.InitializeComboBox(DiscountCmb);
            pointOfSale.CalculateDiscount(SubtotalLbl.Text, DiscountCmb.SelectedItem, DiscountLbl, TotalLbl);

            // Cargar carrito en Shown para poder usar await sin congelar UI
            this.Shown -= Checkout_Shown;
            this.Shown += Checkout_Shown;
        }

        private async void Checkout_Shown(object sender, EventArgs e)
        {
            await LoadCartItemsAsync();
        }

        private async Task LoadCartItemsAsync()
        {
            listBox1.Items.Clear();
            DataTable dt = await cartManager.GetCartItemsAsync();

            foreach (DataRow r in dt.Rows)
            {
                string name = r["Name"]?.ToString() ?? "";
                decimal price = 0;
                int qty = 0;

                decimal.TryParse(r["Price"]?.ToString(), out price);
                int.TryParse(r["Quantity"]?.ToString(), out qty);

                listBox1.Items.Add($"{qty} x {name} - ${price}");
            }
        }

        // ON TEXT CHANGED
        public void ChangeEventHandler()
        {
            if (decimal.TryParse(CashTxt.Text, out _))
            {
                pointOfSale.CalculateChange(TotalLbl, CashTxt, ChangeLbl);
            }
        }

        // COMBOBOX EVENT
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pointOfSale.CalculateDiscount(SubtotalLbl.Text, DiscountCmb.SelectedItem, DiscountLbl, TotalLbl);
        }

        // TEXTBOX EVENT
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ChangeEventHandler();
        }

        // LABEL8 EVENT
        private void label8_TextChanged(object sender, EventArgs e)
        {
            ChangeEventHandler();
        }

        // INSERT / PROCESS BUTTON
        private async void button1_Click(object sender, EventArgs e)
        {
            // Generar TransactionId
            var transactionId = new TransactionIdGenerator().GenerateTransactionId();

            try
            {
                // Recalcula por si cambió el descuento o cash
                pointOfSale.CalculateDiscount(SubtotalLbl.Text, DiscountCmb.SelectedItem, DiscountLbl, TotalLbl);

                // Asegura parseo de valores
                decimal total = 0, cash = 0, discountAmount = 0;
                double discountPercent = 0, change = 0;

                decimal.TryParse(TotalLbl.Text, out total);
                decimal.TryParse(CashTxt.Text, out cash);
                decimal.TryParse(DiscountLbl.Text?.Replace("%",""), out var discParsed);
                // Si DiscountLbl muestra “15%” usa percent, si muestra “$10” ajusta según tu UI
                if (DiscountLbl.Text != null && DiscountLbl.Text.Contains("%"))
                {
                    discountPercent = (double)discParsed;
                }
                else
                {
                    decimal.TryParse(DiscountLbl.Text, out discountAmount);
                }
                decimal.TryParse(ChangeLbl.Text, out var changeDec);
                change = (double)changeDec;

                // Procesar transacción (Sheets)
                // SUGERENCIA: crea PointOfSale.ProcessTransactionAsync que:
                // - Inserte en Transaction
                // - Inserte en Orders
                // - Descuente stock en Product
                // - Limpie Cart del usuario
                bool ok = await pointOfSale.ProcessTransactionAsync(
                    SubtotalLbl.Text,
                    CashTxt.Text,
                    DiscountCmb.SelectedItem,
                    transactionId,
                    listBox1
                );

                if (ok)
                {
                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al procesar la transacción:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // CANCEL BUTTON
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
