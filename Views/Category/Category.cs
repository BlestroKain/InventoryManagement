using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa.InventoryApp.Views
{
    public partial class Category : Form
    {
        private readonly CategoryManager categoryManager;

        public Category()
        {
            InitializeComponent();
            categoryManager = new CategoryManager();

            // Cargar asíncrono cuando el form se muestra
            this.Shown -= Category_Shown;
            this.Shown += Category_Shown;
        }

        private async void Category_Shown(object sender, EventArgs e)
        {
            await RefreshGridAsync();
        }

        private async Task RefreshGridAsync()
        {
            dataGridView1.DataSource = await categoryManager.GetCategoriesAsync();
        }

        // ADD BUTTON
        private async void button1_Click(object sender, EventArgs e)
        {
            using (var dlg = new CatDialog(categoryManager))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    await RefreshGridAsync();
                }
            }
        }

        // UPDATE BUTTON
        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "No hay categorías disponibles para editar.",
                    "Categoría vacía",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var drv = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("No se puede leer la categoría seleccionada.", "Error",
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
                        await RefreshGridAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al leer los datos de la categoría:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // DELETE BUTTON
        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una categoría para eliminar.",
                    "Categoría vacía",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var drv = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("No se puede leer la categoría seleccionada.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(drv["Id"]);

            if (MessageBox.Show(
                    "¿Está seguro de que desea eliminar esta categoría?",
                    "Advertencia",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) == DialogResult.Yes)
            {
                await categoryManager.DeleteCategoryAsync(id);
                await RefreshGridAsync();
            }
        }
    }
}
