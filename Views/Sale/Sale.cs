using System;
using System.Data;
using System.Windows.Forms;
using InventoryApp.Data;

namespace InventoryApp.InventoryApp.Views
{
    public partial class Sale : Form
    {
        private readonly CartManager cartManager;

        public Sale()
        {
            InitializeComponent();
            cartManager = new CartManager();
            DisplayCartItem();
        }

        // FETCH DATA FROM CART
        private void DisplayCartItem()
        {
            DataTable dt = cartManager.GetCartItems();
            dataGridView1.DataSource = dt;
        }

        // CHECKOUT BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            decimal totalPrice = cartManager.GetTotalPrice();
            if (totalPrice > 0)
            {
                using (var dlg = new Checkout(totalPrice))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        DisplayCartItem();
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
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var qVal = dataGridView1.SelectedRows[0].Cells["Quantity"].Value;
                var idVal = dataGridView1.SelectedRows[0].Cells["ProductId"].Value;

                int quantity = Convert.ToInt32(qVal);
                int productId = Convert.ToInt32(idVal);

                using (var dlg = new Quantity(quantity, productId))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        DisplayCartItem();
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

        // REMOVE BUTTON
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var idVal = dataGridView1.SelectedRows[0].Cells["ProductId"].Value;
                int productId = Convert.ToInt32(idVal);

                if (MessageBox.Show(
                        "Are you sure you want to remove this item from your cart?",
                        "Warning!",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    ) == DialogResult.Yes)
                {
                    cartManager.RemoveCartItem(productId);
                    DisplayCartItem();
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
    }
}
