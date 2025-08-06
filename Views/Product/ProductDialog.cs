using System;
using System.ComponentModel;
using System.Windows.Forms;
using InventoryApp.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace InventoryApp
{
    public partial class ProductDialog : Form
    {
        private readonly ProductManager productManager;
        private readonly int itemId; // 0 = nuevo, >0 = editar

        // Constructor para crear
        public ProductDialog(ProductManager manager)
        {
            InitializeComponent();
            productManager = manager ?? throw new ArgumentNullException(nameof(manager));
            itemId = 0;

            // Primero cargo categorías
            CategoryCmb.Items.Clear();
            CategoryCmb.Items.AddRange(productManager.GetCategoryItems());

            Text = "Add New Product";
        }

        // Constructor para editar
        public ProductDialog(
            ProductManager manager,
            int id,
            string name,
            int price,
            int stock,
            int unit,
            string category
        )
        {
            InitializeComponent();
            productManager = manager ?? throw new ArgumentNullException(nameof(manager));
            itemId = id;

            // Primero cargo categorías
            CategoryCmb.Items.Clear();
            CategoryCmb.Items.AddRange(productManager.GetCategoryItems());

            // Luego asigno valores a los controles
            NameTxt.Text = name ?? "";
            PriceTxt.Text = price.ToString();
            StockTxt.Text = stock.ToString();
            UnitTxt.Text = unit.ToString();

            // Selecciono la categoría si existe, sino dejo el texto
            if (CategoryCmb.Items.Contains(category))
            {
                CategoryCmb.SelectedItem = category;
            }
            else
            {
                CategoryCmb.Text = category ?? "";
            }

            Text = "Edit Product";
        }

        // Validación sencilla de que todos los campos estén completos y numéricos
        private bool ValidateAll()
        {
            errorProvider1.Clear();
            bool ok = true;

            if (string.IsNullOrWhiteSpace(NameTxt.Text))
            {
                errorProvider1.SetError(NameTxt, "Product name is required.");
                ok = false;
            }
            if (!int.TryParse(PriceTxt.Text, out _))
            {
                errorProvider1.SetError(PriceTxt, "Price must be a number.");
                ok = false;
            }
            if (!int.TryParse(StockTxt.Text, out _))
            {
                errorProvider1.SetError(StockTxt, "Stock must be a number.");
                ok = false;
            }
            if (!int.TryParse(UnitTxt.Text, out _))
            {
                errorProvider1.SetError(UnitTxt, "Unit must be a number.");
                ok = false;
            }
            if (string.IsNullOrWhiteSpace(CategoryCmb.Text))
            {
                errorProvider1.SetError(CategoryCmb, "Category is required.");
                ok = false;
            }

            return ok;
        }

        // Guarda o actualiza el producto
        private void SaveProduct()
        {
            if (!ValidateAll()) return;

            var name = NameTxt.Text.Trim();
            var price = Convert.ToInt32(PriceTxt.Text);
            var stock = Convert.ToInt32(StockTxt.Text);
            var unit = Convert.ToInt32(UnitTxt.Text);
            var category = CategoryCmb.Text.Trim();

            // Si es categoría nueva, la inserto
            if (CategoryCmb.SelectedIndex == -1)
            {
                productManager.InsertCategory(category);
            }

            if (itemId == 0)
            {
                productManager.InsertProduct(name, price, stock, unit, category);
            }
            else
            {
                productManager.UpdateProduct(itemId, name, price, stock, unit, category);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e) => SaveProduct();

        private void button2_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            Close();
        }

        // Tecla Enter para avanzar formulario
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrWhiteSpace(NameTxt.Text))
            {
                PriceTxt.Focus();
                e.Handled = true;
            }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrWhiteSpace(PriceTxt.Text))
            {
                StockTxt.Focus();
                e.Handled = true;
            }
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrWhiteSpace(StockTxt.Text))
            {
                UnitTxt.Focus();
                e.Handled = true;
            }
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrWhiteSpace(UnitTxt.Text))
            {
                CategoryCmb.Focus();
                e.Handled = true;
            }
        }
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrWhiteSpace(CategoryCmb.Text))
            {
                SaveProduct();
                e.Handled = true;
            }
        }

       
    }
}
