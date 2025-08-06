using System.Data;
using System;
using System.Windows.Forms;
using InventoryApp.Data;

namespace InventoryApp.InventoryApp.Views
{
    public partial class Category : Form
    {
        private readonly CategoryManager categoryManager;

        public Category()
        {
            InitializeComponent();
            categoryManager = new CategoryManager();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dataGridView1.DataSource = categoryManager.GetCategories();
        }

        // ADD BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            using (var dlg = new CatDialog(categoryManager))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    RefreshGrid();
                }
            }
        }

        // UPDATE BUTTON
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "No category is available for editing.",
                    "Empty Category",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Leemos directamente del DataRowView ligado
            var drv = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("Unable to read selected category.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int id = Convert.ToInt32(drv["Id"]);
                string categoryItem = drv["CategoryItem"]?.ToString() ?? "";

                using (var dlg = new CatDialog(categoryManager, id, categoryItem))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        RefreshGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error reading category data:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // DELETE BUTTON
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Please select a category to delete.",
                    "Empty Category",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var drv = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("Unable to read selected category.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(drv["Id"]);

            if (MessageBox.Show(
                    "Are you sure you want to delete this category?",
                    "Warning!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) == DialogResult.Yes)
            {
                categoryManager.DeleteCategory(id);
                RefreshGrid();
            }
        }
    }
}
