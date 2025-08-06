using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using InventoryApp.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace InventoryApp
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

        // Validates input and duplicates
        private bool ValidateAll()
        {
            errorProvider1.Clear();
            var name = textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider1.SetError(textBox2, "Category name is required.");
                return false;
            }

            // Check duplicates
            var dt = categoryManager.GetCategories();
            foreach (DataRow row in dt.Rows)
            {
                var existingName = row["CategoryItem"]?.ToString();
                var existingId = Convert.ToInt32(row["Id"]);
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
            // simple non-empty check on leave
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

        // SAVE or UPDATE
        private void SaveCategory()
        {
            if (!ValidateAll()) return;

            var name = textBox2.Text.Trim();
            if (itemId == 0)
            {
                categoryManager.AddCategory(name);
            }
            else
            {
                categoryManager.UpdateCategory(itemId, name);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e) => SaveCategory();

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SaveCategory();
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            Close();
        }
    }
}
