using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using RapiMesa.Utility;   // SyncQueue
using RapiMesa.Data;      // SheetsRepo

namespace RapiMesa.Data
{
    public class StockManager
    {
        // 1) Obtener Id por nombre (cache-first, 0 si no existe)
        public async Task<int> GetProductIdByNameAsync(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName)) return 0;

            var pdt = await SheetsRepo.ReadTableCachedAsync("Product");
            var row = pdt.AsEnumerable()
                         .FirstOrDefault(r => string.Equals(
                             r["Name"]?.ToString()?.Trim() ?? "",
                             itemName.Trim(),
                             StringComparison.OrdinalIgnoreCase));

            return row == null ? 0 : ToInt(row["Id"]);
        }

        // 2) Obtener stock actual por Id (cache-first, 0 si no existe)
        public async Task<int> GetCurrentStockByIdAsync(int productId)
        {
            if (productId <= 0) return 0;

            var pdt = await SheetsRepo.ReadTableCachedAsync("Product");
            var row = pdt.AsEnumerable()
                         .FirstOrDefault(r => ToInt(r["Id"]) == productId);

            return row == null ? 0 : ToInt(row["Stock"]);
        }

        // 3) Actualizar stock del producto (escritura en background)
        public async Task UpdateStockAsync(int productId, int newStock)
        {
            if (productId <= 0) throw new ArgumentException("Invalid product id");

            // Necesitamos row1 para UpdateRow -> una sola búsqueda puntual
            var (row1, row) = await SheetsRepo.FindRowByAsync("Product", "Id", productId.ToString());
            if (row1 == 0 || row == null) throw new Exception("Product not found");

            int oldStock = ToInt(row["Stock"]);

            // Reescribimos fila completa por consistencia
            var values = new object[]
            {
                ToInt(row["Id"]),
                row["Name"]?.ToString() ?? "",
                ToInt(row["Price"]),
                newStock,
                ToInt(row["Unit"]),
                row["Category"]?.ToString() ?? ""
            };

            SyncQueue.Enqueue(new UpdateRowOp("Product", row1, values));
            SheetsRepo.Invalidate("Product");
            if (oldStock != newStock)
                ChangeLogger.Log(UserSession.SessionUID.ToString(), "StockUpdate", "Product", $"{productId}:{oldStock}->{newStock}");
        }

        // 4) Insertar historial de stocks (Append en background, Id local)
        public async Task InsertHistoryAsync(int productId, int addedStocks)
        {
            // Saco Id local desde cache
            var hdt = await SheetsRepo.ReadTableCachedAsync("History");
            int newId = hdt.AsEnumerable()
                           .Select(r => ToInt(r["Id"]))
                           .DefaultIfEmpty(0)
                           .Max() + 1;

            string nowIso = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            SyncQueue.Enqueue(new AppendOp(
                "History",
                new object[] { newId, productId, addedStocks, nowIso }
            ));
            SheetsRepo.Invalidate("History");
        }

        // 5) Alias
        public Task<int> GetProductStockAsync(int productId) => GetCurrentStockByIdAsync(productId);

        // 6) Comodín: ajustar stock por delta (+n o -n). Puede registrar en History.
        public async Task AdjustStockAsync(int productId, int delta, bool logHistory = true)
        {
            int current = await GetCurrentStockByIdAsync(productId);
            int updated = Math.Max(0, current + delta);
            await UpdateStockAsync(productId, updated);

            if (logHistory && delta != 0)
                await InsertHistoryAsync(productId, delta);
        }

        // 7) Conveniencia: sumar/restar
        public Task IncreaseStockAsync(int productId, int add) => AdjustStockAsync(productId, Math.Abs(add), true);
        public Task DecreaseStockAsync(int productId, int sub) => AdjustStockAsync(productId, -Math.Abs(sub), true);

        // --- helpers ---
        private static int ToInt(object v) => int.TryParse(v?.ToString(), out var x) ? x : 0;
    }
}
