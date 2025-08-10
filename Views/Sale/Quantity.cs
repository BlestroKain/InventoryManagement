// UI/Quantity.cs
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa
{
    public partial class Quantity : Form
    {
        private readonly CartManager cartManager = new CartManager();
        private readonly StockManager stockManager = new StockManager();

        private readonly int _cartId;     // fila de Cart a actualizar
        private readonly int _productId;  // para validar stock

        public Quantity(int currentQty, int cartId, int productId)
        {
            InitializeComponent();
            _cartId = cartId;
            _productId = productId;
            textBox2.Text = currentQty.ToString();
        }

        private void button3_Click(object sender, EventArgs e) // -
        {
            if (int.TryParse(textBox2.Text, out var v) && v > 1)
                textBox2.Text = (v - 1).ToString();
        }

        private async void button4_Click(object sender, EventArgs e) // +
        {
            if (!int.TryParse(textBox2.Text, out var v)) v = 1;
            try
            {
                var stock = await stockManager.GetProductStockAsync(_productId);
                if (v < stock) textBox2.Text = (v + 1).ToString();
                else MessageBox.Show("Stock limit reached.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking stock:\r\n" + ex.Message);
            }
        }

        private async void button1_Click(object sender, EventArgs e) // Save
        {
            if (!int.TryParse(textBox2.Text, out var qty) || qty <= 0)
            {
                MessageBox.Show("Invalid quantity.");
                return;
            }

            try
            {
                Toggle(false);
                await cartManager.UpdateQuantityInCartAsync(_cartId, qty);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating cart:\r\n" + ex.Message);
            }
            finally
            {
                Toggle(true);
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e) => Close();

        private void Toggle(bool en)
        {
            button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = en;
        }
    }
}
