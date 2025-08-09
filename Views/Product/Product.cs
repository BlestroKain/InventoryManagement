using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;
using RapiMesa.InventoryApp.dlg;

namespace RapiMesa
{
    public partial class Product : Form
    {
        private readonly ProductManager productManager;

        public Product()
        {
            InitializeComponent();
            productManager = new ProductManager();

            // Carga inicial asíncrona (mejor en Shown que en el ctor)
            this.Shown -= Product_Shown;
            this.Shown += Product_Shown;

            SetupAddToCartButton();
        }

        private async void Product_Shown(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            dataGridView1.DataSource = await productManager.GetProductsAsync();
        }

        // Búsqueda y visualización
        private async Task PerformSearchAsync()
        {
            DataTable dt = await productManager.SearchProductsAsync(textBox1.Text);
            dataGridView1.DataSource = dt;
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            await PerformSearchAsync();
        }

        private async void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                await PerformSearchAsync();
                e.Handled = true;
            }
        }

        private async void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                await PerformSearchAsync(); // tu Search ya maneja vacío → devuelve todo
            }
        }

        // INSERT BUTTON
        private async void Add_Click(object sender, EventArgs e)
        {
            using (var dlg = new ProductDialog(productManager))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    await LoadProductsAsync();
                }
            }
        }

        // UPDATE BUTTON
        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "No product is available for editing.",
                    "Empty!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var drv = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show(
                    "Unable to read the selected row.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            try
            {
                int id = Convert.ToInt32(drv["Id"]);
                string name = drv["Name"]?.ToString() ?? "";
                int price = Convert.ToInt32(drv["Price"]);
                int stock = Convert.ToInt32(drv["Stock"]);
                int unit = Convert.ToInt32(drv["Unit"]);
                string cat = drv["Category"]?.ToString() ?? "";

                using (var dlg = new ProductDialog(productManager, id, name, price, stock, unit, cat))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        await LoadProductsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error reading product data:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // DELETE BUTTON
        private async void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Please select a product to delete.",
                    "Empty!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show(
                    "Are you sure want to delete this item?",
                    "Warning!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) == DialogResult.Yes)
            {
                await productManager.DeleteProductAsync(id);
                await LoadProductsAsync();
            }
        }

        // ADD STOCK BUTTON
        private async void AddStockBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "No products are available for adding stock.",
                    "Empty!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            string name = dataGridView1.SelectedRows[0].Cells["Name"].Value?.ToString() ?? "";
            using (var dlg = new AddStock(name))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    await LoadProductsAsync();
                }
            }
        }

        // HISTORY BUTTON
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "No product history is available.",
                    "Empty!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
            using (var historyForm = new History(id))
            {
                historyForm.ShowDialog();
            }
        }

        // Añade la columna de botón "Add" si no existe, y engancha a un ÚNICO handler async
        private void SetupAddToCartButton()
        {
            if (dataGridView1.Columns["AddToCart"] == null)
            {
                var btnCol = new DataGridViewButtonColumn
                {
                    Name = "AddToCart",
                    HeaderText = "",
                    Text = "Add",
                    UseColumnTextForButtonValue = true,
                    Width = 60
                };
                dataGridView1.Columns.Add(btnCol);
            }

            dataGridView1.CellContentClick -= dataGridView1_CellContentClick;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }

        // EVENTO CLICK DEL BOTÓN EN EL DATAGRID (async)
        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dataGridView1.Columns[e.ColumnIndex].Name != "AddToCart") return;

            if (!(dataGridView1.Rows[e.RowIndex].DataBoundItem is DataRowView drv)) return;

            string name = drv["Name"]?.ToString() ?? "";
            int price = Convert.ToInt32(drv["Price"]);
            int stock = Convert.ToInt32(drv["Stock"]);

            if (stock <= 0)
            {
                MessageBox.Show("Product out of stock.", "Cart",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Google Sheets
            bool added = await ProductManager.AddItemToCartAsync(name, price);
            MessageBox.Show(
                added ? "Product added to cart." : "Failed to add product.",
                "Cart",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
