using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Data;

namespace RapiMesa
{
    public partial class AddStock : Form
    {
        private readonly StockManager stockManager;
        private readonly string itemName;

        public AddStock(string name)
        {
            InitializeComponent();
            stockManager = new StockManager();
            itemName = name ?? "";
            label3.Text = itemName;
        }

        // INSERT STOCK BUTTON (async)
        private async void button1_Click(object sender, EventArgs e)
        {
            // Validación rápida
            if (!int.TryParse(textBox2.Text, out int addedStocks) || addedStocks <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida (> 0).", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ToggleButtons(false);

                int productId = await stockManager.GetProductIdByNameAsync(itemName);
                if (productId == 0)
                {
                    MessageBox.Show("Producto no encontrado.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int currentStock = await stockManager.GetCurrentStockByIdAsync(productId);
                int newStock = checked(currentStock + addedStocks); // por si se va de rango

                await stockManager.UpdateStockAsync(productId, newStock);
                await stockManager.InsertHistoryAsync(productId, addedStocks);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (OverflowException)
            {
                MessageBox.Show("El stock resultante es demasiado grande.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar stock:\r\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleButtons(true);
            }
        }

        // CANCEL BUTTON
        private void button2_Click(object sender, EventArgs e) => Close();

        private void ToggleButtons(bool enabled)
        {
            button1.Enabled = enabled;
            button2.Enabled = enabled;
        }
    }
}
