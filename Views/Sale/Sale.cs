using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa.InventoryApp.Views
{
    public partial class Sale : Form
    {
        private readonly CartManager cartManager;

        public Sale()
        {
            InitializeComponent();
            cartManager = new CartManager();

            // Cargar asíncrono cuando el form se muestra (permite await)
            this.Shown -= Sale_Shown;
            this.Shown += Sale_Shown;
        }

        private async void Sale_Shown(object sender, EventArgs e)
        {
            await DisplayCartItemAsync();
        }

        // Cargar carrito y ocultar columnas internas
        private async Task DisplayCartItemAsync()
        {
            var dt = await cartManager.GetCartItemsAsync();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = dt;

            HideCol("ProductId");  // interno
            HideCol("Uid");        // interno

            void HideCol(string name)
            {
                var col = dataGridView1.Columns[name];
                if (col != null) col.Visible = false;
            }
        }

        // CHECKOUT BUTTON
        private async void button1_Click(object sender, EventArgs e)
        {
            decimal totalPrice = await cartManager.GetTotalPriceAsync();
            if (totalPrice > 0)
            {
                using (var dlg = new Checkout(totalPrice))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        await DisplayCartItemAsync();
                    }
                }
            }
            else
            {
                MessageBox.Show(
                    "Cart is empty.",
                    "Empty Cart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        // ADD QUANTITY BUTTON
        private async void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Cart is empty.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dataGridView1.SelectedRows[0];

            // OJO: aunque estén ocultas, siguen estando en el DataSource
            int cartId = Convert.ToInt32(row.Cells["Id"].Value);          // <-- para actualizar/eliminar
            int productId = Convert.ToInt32(row.Cells["ProductId"].Value);   // <-- para validar stock en el diálogo
            int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);

            using (var dlg = new Quantity(quantity, cartId, productId)) // ver abajo
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    await DisplayCartItemAsync();
                }
            }
        }


        // REMOVE BUTTON
        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Cart is empty.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int cartId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Are you sure you want to remove this item from your cart?",
                                "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await cartManager.RemoveCartItemAsync(cartId);
                await DisplayCartItemAsync();
            }
        }
        public async Task UpdateQuantityInCartAsync(int cartId, int quantity)
        {
            // 1) Ubicar la fila (1-based con header)
            var tuple = await SheetsRepo.FindRowByAsync("Cart", "Id", cartId.ToString());
            int row1 = tuple.Item1;
            if (row1 == 0) return;

            // 2) Escribir SOLO la columna Quantity (F) en esa fila
            // Cart schema: A:Id, B:ProductId, C:Uid, D:Name, E:Price, F:Quantity
            var range = $"Cart!F{row1}";
            await SheetsRepo.UpdateCellAsync(range, quantity); // ver helper abajo
        }

    }
}
