using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa
{
    public partial class Quantity : Form
    {
        private readonly CartManager cartManager;
        private readonly StockManager stockManager;

        private readonly int cartId;     // <- para actualizar la fila del carrito
        private readonly int productId;  // <- para validar stock

        public Quantity(int quantity, int cartId, int productId)
        {
            InitializeComponent();
            this.cartId = cartId;
            this.productId = productId;

            textBox2.Text = quantity.ToString();

            cartManager = new CartManager();
            stockManager = new StockManager();
        }

        // MINUS
        private void button3_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text, out int v)) return;
            if (v > 1) textBox2.Text = (v - 1).ToString();
        }

        // PLUS (consulta stock en Sheets)
        private async void button4_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text, out int v)) v = 1;
            try
            {
                int stock = await stockManager.GetProductStockAsync(productId);
                if (v < stock) textBox2.Text = (v + 1).ToString();
                else MessageBox.Show("Límite de stock alcanzado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar stock:\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // SAVE
        private async void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }

            try
            {
                ToggleButtons(false);
                await cartManager.UpdateQuantityInCartAsync(cartId, qty); // <- cartId!
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el carrito:\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleButtons(true);
            }
        }

        // CANCEL
        private void button2_Click(object sender, EventArgs e) => Close();

        private void ToggleButtons(bool enabled)
        {
            button1.Enabled = enabled; // Save
            button2.Enabled = enabled; // Cancel
            button3.Enabled = enabled; // Minus
            button4.Enabled = enabled; // Plus
        }
    }
}
