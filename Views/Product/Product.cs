using System;
using System.Data;
using System.Windows.Forms;
using InventoryApp.Data;
using InventoryApp.InventoryApp.dlg;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace InventoryApp
{
    public partial class Product : Form
    {
        private readonly ProductManager productManager;

        public Product()
        {
            InitializeComponent();
            productManager = new ProductManager();

            LoadProducts();
            SetupAddToCartButton();
        }

        private void LoadProducts()
        {
            dataGridView1.DataSource = productManager.GetProducts();
        }
        // Búsqueda y visualización
        private void PerformSearch()
        {
            DataTable dt = productManager.SearchProducts(textBox1.Text);
            dataGridView1.DataSource = dt;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PerformSearch();
                e.Handled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                PerformSearch();
            }
        }

        // INSERT BUTTON
        private void Add_Click(object sender, EventArgs e)
        {
            using (var dlg = new ProductDialog(productManager))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.DataSource = productManager.GetProducts();
                }
            }
        }

        // UPDATE BUTTON
        private void button2_Click(object sender, EventArgs e)
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

            // Asumimos que tu DataGridView está ligado a un DataTable/DataView
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
                string category = drv["Category"]?.ToString() ?? "";

                using (var dlg = new ProductDialog(
                    productManager,
                    id, name, price, stock, unit, category
                ))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        dataGridView1.DataSource = productManager.GetProducts();
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
        private void DeleteBtn_Click(object sender, EventArgs e)
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
                productManager.DeleteProduct(id);
                dataGridView1.DataSource = productManager.GetProducts();
            }
        }

        // ADD STOCK BUTTON
        private void AddStockBtn_Click(object sender, EventArgs e)
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
                    dataGridView1.DataSource = productManager.GetProducts();
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

        // Añade columna de botón "Add" al carrito
        // Añade la columna de botón "Add" si no existe, y engancha al CellClick
        private void SetupAddToCartButton()
        {
            // 1) Añade la columna sólo si no existe
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

            // 2) Desuscribe TODO lo anterior
            dataGridView1.CellContentClick -= DataGridView1_CellClick;
            dataGridView1.CellClick -= DataGridView1_CellClick;

            // 3) Elige UN sólo evento para manejar el click. 
            //    Yo recomiendo CellContentClick para botones:
            dataGridView1.CellContentClick += DataGridView1_CellClick;
        }



        // EVENTO CLICK DEL BOTÓN EN EL DATAGRID
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Solo filas válidas
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Solo el botón "AddToCart"
            if (dataGridView1.Columns[e.ColumnIndex].Name != "AddToCart") return;

            // Leemos directo del DataRowView ligado
            if (!(dataGridView1.Rows[e.RowIndex].DataBoundItem is DataRowView drv)) return;

            string name = drv["Name"].ToString();
            int price = Convert.ToInt32(drv["Price"]);
            int stock = Convert.ToInt32(drv["Stock"]);

            if (stock <= 0)
            {
                MessageBox.Show("Product out of stock.", "Cart",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Esto se ejecuta UNA sola vez por click
            bool added = ProductManager.AddItemToCart(name, price);
            MessageBox.Show(
                added ? "Product added to cart." : "Failed to add product.",
                "Cart",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

    }
}
