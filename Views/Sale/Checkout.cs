using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;      // CartManager (lee del cache)
using RapiMesa.Utility;   // PointOfSale con ProcessTransactionAsync()

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

            // Valores iniciales UI
            SubtotalLbl.Text = subtotal.ToString(CultureInfo.InvariantCulture);
            DiscountLbl.Text = "0";
            TotalLbl.Text = SubtotalLbl.Text;
            ChangeLbl.Text = "0";

            // Inicializa descuentos y calcula
            pointOfSale.InitializeComboBox(DiscountCmb);
            pointOfSale.CalculateDiscount(SubtotalLbl.Text, DiscountCmb.SelectedItem, DiscountLbl, TotalLbl);

            // Cableo de eventos (evita depender de nombres “comboBox1_…”, etc.)
            this.Shown += Checkout_Shown;
            DiscountCmb.SelectedIndexChanged += DiscountCmb_SelectedIndexChanged;
            CashTxt.TextChanged += CashTxt_TextChanged;
            TotalLbl.TextChanged += TotalLbl_TextChanged;

        }

        // Cargar carrito sin congelar UI
        private async void Checkout_Shown(object sender, EventArgs e)
        {
            await LoadCartItemsAsync();
        }

        private async Task LoadCartItemsAsync()
        {
            listBox1.Items.Clear();

            DataTable dt = await cartManager.GetCartItemsAsync(); // ya viene del cache
            foreach (DataRow r in dt.Rows)
            {
                string name = r["Name"]?.ToString() ?? "";
                decimal price = ParseDec(r["Price"]);
                int qty = SafeInt(r["Quantity"]);
                listBox1.Items.Add($"{qty} x {name} - ${price}");
            }
        }

        // Recalcula cambio cuando cambian Total o Cash
        private void DiscountCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            pointOfSale.CalculateDiscount(SubtotalLbl.Text, DiscountCmb.SelectedItem, DiscountLbl, TotalLbl);
            RecalcChange();
        }

        private void CashTxt_TextChanged(object sender, EventArgs e) => RecalcChange();

        private void TotalLbl_TextChanged(object sender, EventArgs e) => RecalcChange();

        private void RecalcChange()
        {
            if (decimal.TryParse(CashTxt.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                pointOfSale.CalculateChange(TotalLbl, CashTxt, ChangeLbl);
            }
            else
            {
                ChangeLbl.Text = "0";
            }
        }

        // Procesar/Vender
        private async void button1_Click(object sender, EventArgs e)
        {
            var transactionId = new TransactionIdGenerator().GenerateTransactionId();

            try
            {
                // Recalcula por si cambió el descuento
                pointOfSale.CalculateDiscount(SubtotalLbl.Text, DiscountCmb.SelectedItem, DiscountLbl, TotalLbl);
                RecalcChange();

                // Validaciones básicas
                decimal total = ParseDec(TotalLbl.Text);
                decimal cash = ParseDec(CashTxt.Text);

                if (total <= 0)
                {
                    MessageBox.Show("El total debe ser mayor a 0.");
                    return;
                }
                if (cash < total)
                {
                    MessageBox.Show("El efectivo no alcanza para cubrir el total.");
                    return;
                }

                // Procesa transacción contra cache + encola sync a Sheets (inside)
                bool ok = await pointOfSale.ProcessTransactionAsync(
                    SubtotalLbl.Text,
                    CashTxt.Text,
                    DiscountCmb.SelectedItem,
                    transactionId,
                    listBox1
                );

                if (ok)
                {
                    DialogResult = DialogResult.OK; // cierra y refresca caller
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

        // Cancelar
        private void button2_Click(object sender, EventArgs e) => Close();

        // ------- helpers -------
        private static int SafeInt(object v) =>
            int.TryParse(v?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var n) ? n : 0;

        private static decimal ParseDec(object v)
        {
            if (v == null) return 0m;
            var s = v.ToString().Trim().Replace("$", "").Replace("€", "").Replace("£", "").Replace(",", "");
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
        }
    }
}
