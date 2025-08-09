using System;
using System.Data;
using System.Threading.Tasks;

namespace RapiMesa.Data
{
    public class StockManager
    {
        // 1) Obtener el Id del producto por nombre
        public async Task<int> GetProductIdByNameAsync(string itemName)
        {
            var (row1, row) = await SheetsRepo.FindRowByAsync("Product", "Name", itemName);
            if (row1 == 0 || row == null) return 0;
            return ToInt(row["Id"]);
        }

        // 2) Obtener stock actual por Id
        public async Task<int> GetCurrentStockByIdAsync(int productId)
        {
            var (row1, row) = await SheetsRepo.FindRowByAsync("Product", "Id", productId.ToString());
            if (row1 == 0 || row == null) return 0;
            return ToInt(row["Stock"]);
        }

        // 3) Actualizar stock del producto
        public async Task UpdateStockAsync(int productId, int newStock)
        {
            var (row1, row) = await SheetsRepo.FindRowByAsync("Product", "Id", productId.ToString());
            if (row1 == 0 || row == null) throw new Exception("Product not found");

            // Reescribimos toda la fila para mantener consistencia
            await SheetsRepo.UpdateRowAsync(
                "Product",
                row1,
                new object[] {
                    ToInt(row["Id"]),
                    row["Name"]?.ToString() ?? "",
                    ToInt(row["Price"]),
                    newStock,                                   // <-- Stock actualizado
                    ToInt(row["Unit"]),
                    row["Category"]?.ToString() ?? ""
                }
            );
        }

        // 4) Insertar entrada en historial de stocks
        public async Task InsertHistoryAsync(int productId, int addedStocks)
        {
            int id = await SheetsRepo.NextIdAsync("History", "Id");
            // Fecha ISO para ordenar bien en Sheets
            string nowIso = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            await SheetsRepo.AppendRowAsync(
                "History",
                new object[] { id, productId, addedStocks, nowIso }
            );
        }

        // 5) Alias
        public Task<int> GetProductStockAsync(int productId) => GetCurrentStockByIdAsync(productId);

        // --- helpers ---
        private static int ToInt(object v) => int.TryParse(v?.ToString(), out var x) ? x : 0;
    }
}
