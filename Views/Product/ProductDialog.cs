using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa
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

            Text = "Add New Product";

            // Cargar categorías cuando el form ya está mostrado (permite await)
            this.Shown -= ProductDialog_Shown;
            this.Shown += ProductDialog_Shown;
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

            // Seteo valores primero
            NameTxt.Text = name ?? "";
            PriceTxt.Text = price.ToString();
            StockTxt.Text = stock.ToString();
            UnitTxt.Text = unit.ToString();
            CategoryCmb.Text = category ?? "";

            Text = "Edit Product";

            // Cargar categorías al mostrar y luego seleccionar la existente
            this.Shown -= ProductDialog_Shown;
            this.Shown += ProductDialog_Shown;
        }

        // === Carga de categorías (async al mostrar) ===
        private async void ProductDialog_Shown(object sender, EventArgs e)
        {
            try
            {
                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories:\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadCategoriesAsync()
        {
            CategoryCmb.Items.Clear();
            var items = await productManager.GetCategoryItemsAsync(); // <- Sheets
            if (items != null && items.Length > 0)
            {
                CategoryCmb.Items.AddRange(items);
            }

            // Si venimos en modo edición y ya había texto, intenta seleccionarlo
            var current = CategoryCmb.Text?.Trim();
            if (!string.IsNullOrEmpty(current) && CategoryCmb.Items.Contains(current))
            {
                CategoryCmb.SelectedItem = current;
            }
        }

        // === Validaciones locales ===
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

        // === Guardar (async) ===
        private async Task SaveProductAsync()
        {
            if (!ValidateAll()) return;

            var name = NameTxt.Text.Trim();
            var price = Convert.ToInt32(PriceTxt.Text);
            var stock = Convert.ToInt32(StockTxt.Text);
            var unit = Convert.ToInt32(UnitTxt.Text);
            var category = CategoryCmb.Text.Trim();

            try
            {
                // Si es categoría nueva, la inserto (evita duplicados en manager)
                if (CategoryCmb.SelectedIndex == -1 && !string.IsNullOrWhiteSpace(category))
                {
                    await productManager.InsertCategoryAsync(category); // <- Sheets
                }

                if (itemId == 0)
                {
                    await productManager.InsertProductAsync(name, price, stock, unit, category); // <- Sheets
                }
                else
                {
                    await productManager.UpdateProductAsync(itemId, name, price, stock, unit, category); // <- Sheets
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving product:\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botones
        private async void button1_Click(object sender, EventArgs e) => await SaveProductAsync();

        private void button2_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            Close();
        }

        // Enter para avanzar
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
        private async void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrWhiteSpace(CategoryCmb.Text))
            {
                e.Handled = true;
                await SaveProductAsync();
            }
        }
    }
}
