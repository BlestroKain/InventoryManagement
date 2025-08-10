// Data/ProductManagerSheets.cs
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using RapiMesa.Utility; // SyncQueue

namespace RapiMesa.Data
{
    public class ProductManager
    {
        public async Task<DataTable> GetProductsAsync()
            => await SheetsRepo.ReadTableCachedAsync("Product");

        public async Task<DataTable> SearchProductsAsync(string term)
        {
            var dt = await GetProductsAsync();
            if (string.IsNullOrWhiteSpace(term)) return dt;

            term = term.Trim().ToLowerInvariant();
            var filtered = dt.Clone();

            foreach (DataRow r in dt.Rows)
            {
                var name = r["Name"]?.ToString()?.ToLowerInvariant() ?? "";
                var cat = r["Category"]?.ToString()?.ToLowerInvariant() ?? "";
                if (name.Contains(term) || cat.Contains(term))
                    filtered.Rows.Add(r.ItemArray);
            }
            return filtered;
        }

        public async Task<string[]> GetCategoryItemsAsync()
        {
            var dt = await SheetsRepo.ReadTableCachedAsync("Category");
            return dt.AsEnumerable()
                     .Select(r => (r["CategoryItem"]?.ToString() ?? "").Trim())
                     .Where(s => !string.IsNullOrWhiteSpace(s))
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .ToArray();
        }

        public async Task InsertProductAsync(string name, int price, int stock, int unit, string category)
        {
            name = (name ?? "").Trim();
            category = (category ?? "").Trim();

            var dt = await SheetsRepo.ReadTableCachedAsync("Product");

            // Generar Id localmente
            int newId = dt.AsEnumerable()
                          .Select(r => int.TryParse(r["Id"]?.ToString(), out var n) ? n : 0)
                          .DefaultIfEmpty(0)
                          .Max() + 1;

            SyncQueue.Enqueue(new AppendOp("Product", new object[] { newId, name, price, stock, unit, category }));
            SheetsRepo.Invalidate("Product");
        }

        public async Task UpdateProductAsync(int id, string name, int price, int stock, int unit, string category)
        {
            if (id <= 0) throw new ArgumentException("Invalid product id");
            name = (name ?? "").Trim();
            category = (category ?? "").Trim();

            var (row1, _) = await SheetsRepo.FindRowByAsync("Product", "Id", id.ToString());
            if (row1 == 0) throw new Exception("Product not found");

            SyncQueue.Enqueue(new UpdateRowOp("Product", row1, new object[] { id, name, price, stock, unit, category }));
            SheetsRepo.Invalidate("Product");
        }

        public async Task DeleteProductAsync(int id)
        {
            if (id <= 0) return;
            var (row1, _) = await SheetsRepo.FindRowByAsync("Product", "Id", id.ToString());
            if (row1 == 0) return;

            SyncQueue.Enqueue(new DeleteRowOp("Product", row1 - 1)); // 0-based
            SheetsRepo.Invalidate("Product");
        }

        public async Task InsertCategoryAsync(string categoryItem)
        {
            var name = (categoryItem ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) return;

            // Evitar duplicados desde caché
            var cdt = await SheetsRepo.ReadTableCachedAsync("Category");
            bool exists = cdt.AsEnumerable()
                             .Any(r => string.Equals(r["CategoryItem"]?.ToString()?.Trim(),
                                                     name, StringComparison.OrdinalIgnoreCase));
            if (exists) return;

            int newId = cdt.AsEnumerable()
                           .Select(r => int.TryParse(r["Id"]?.ToString(), out var n) ? n : 0)
                           .DefaultIfEmpty(0)
                           .Max() + 1;

            SyncQueue.Enqueue(new AppendOp("Category", new object[] { newId, name }));
            SheetsRepo.Invalidate("Category");
        }

        // Add al carrito (Uid + Name) con caché y cola
        public static async Task<bool> AddItemToCartAsync(string name, int price)
        {
            name = (name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) return false;

            int uid = UserSession.SessionUID;

            // 1) Cart desde caché
            var cartDt = await SheetsRepo.ReadTableCachedAsync("Cart");

            // Buscar si ya existe para el mismo usuario
            DataRow found = null;
            int foundRow1 = 0;      // 1-based (incl header)
            int visualIdx = 0;      // índice de cuerpo (para calcular row1)

            foreach (DataRow r in cartDt.Rows)
            {
                visualIdx++;
                var n = r["Name"]?.ToString() ?? "";
                int.TryParse(r["Uid"]?.ToString(), out var u);
                if (u == uid && string.Equals(n, name, StringComparison.OrdinalIgnoreCase))
                {
                    found = r;
                    foundRow1 = visualIdx + 1; // header ocupa la fila 1
                    break;
                }
            }

            if (found != null)
            {
                // Ya existe: solo subimos Quantity
                int q = SafeInt(found["Quantity"]);
                int p = SafeInt(found["Price"]); // mantenemos precio unitario guardado

                SyncQueue.Enqueue(new UpdateRowOp("Cart", foundRow1,
                    new object[] { found["Id"], found["ProductId"], uid, name, p, q + 1 }));

                SheetsRepo.Invalidate("Cart");
                return true;
            }

            // 2) Si no existe, buscamos ProductId desde caché de Product (sin golpear API)
            var pdt = await SheetsRepo.ReadTableCachedAsync("Product");
            int pid = pdt.AsEnumerable()
                         .Where(r => string.Equals(r["Name"]?.ToString()?.Trim(), name, StringComparison.OrdinalIgnoreCase))
                         .Select(r => SafeInt(r["Id"]))
                         .FirstOrDefault();

            // 3) Nuevo Id para Cart (local)
            int newCartId = cartDt.AsEnumerable()
                                  .Select(r => SafeInt(r["Id"]))
                                  .DefaultIfEmpty(0)
                                  .Max() + 1;

            SyncQueue.Enqueue(new AppendOp("Cart", new object[] { newCartId, pid, uid, name, price, 1 }));
            SheetsRepo.Invalidate("Cart");
            return true;
        }

        // ---- helpers ----
        private static int SafeInt(object v) => int.TryParse(v?.ToString(), out var n) ? n : 0;
    }
}
