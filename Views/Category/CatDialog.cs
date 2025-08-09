using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa
{
    public partial class CatDialog : Form
    {
        private readonly CategoryManager categoryManager;
        private readonly int itemId; // 0 = create, >0 = edit

        public CatDialog(CategoryManager manager)
        {
            InitializeComponent();
            categoryManager = manager ?? throw new ArgumentNullException(nameof(manager));
            itemId = 0;
            Text = "Add New Category";

            textBox2.Validating += textBox2_Validating;
        }

        // Edit mode
        public CatDialog(CategoryManager manager, int id, string categoryItem)
        {
            InitializeComponent();
            categoryManager = manager ?? throw new ArgumentNullException(nameof(manager));
            itemId = id;
            Text = "Edit Category";

            textBox2.Text = categoryItem ?? "";
            textBox2.Validating += textBox2_Validating;
        }

        // === Validación async (incluye duplicados en Google Sheets) ===
        private async Task<bool> ValidateAllAsync()
        {
            errorProvider1.Clear();
            var name = (textBox2.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider1.SetError(textBox2, "Category name is required.");
                return false;
            }

            // Verificar duplicados en la hoja "Category"
            var dt = await categoryManager.GetCategoriesAsync();
            foreach (DataRow row in dt.Rows)
            {
                var existingName = row["CategoryItem"]?.ToString();
                int existingId = 0;
                int.TryParse(row["Id"]?.ToString(), out existingId);

                if (string.Equals(existingName, name, StringComparison.OrdinalIgnoreCase)
                    && (itemId == 0 || existingId != itemId))
                {
                    errorProvider1.SetError(textBox2, "This category already exists.");
                    return false;
                }
            }

            return true;
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                errorProvider1.SetError(textBox2, "Category name is required.");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(textBox2, "");
                e.Cancel = false;
            }
        }

        // === Guardar / Actualizar (async) ===
        private async Task SaveCategoryAsync()
        {
            if (!await ValidateAllAsync()) return;

            var name = textBox2.Text.Trim();

            try
            {
                if (itemId == 0)
                {
                    await categoryManager.AddCategoryAsync(name);
                }
                else
                {
                    await categoryManager.UpdateCategoryAsync(itemId, name);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error saving category:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // SAVE or UPDATE
        private async void button1_Click(object sender, EventArgs e)
        {
            await SaveCategoryAsync();
        }

        private async void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                await SaveCategoryAsync();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            Close();
        }
    }
}
