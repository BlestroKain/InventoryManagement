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

        public async Task<DataTable> SearchProductsAsync(
            string term,
            int? minPrice = null,
            int? maxPrice = null,
            string category = null)
        {
            var dt = await GetProductsAsync();

            var rows = dt.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim().ToLowerInvariant();
                rows = rows.Where(r =>
                    (r["Name"]?.ToString()?.ToLowerInvariant().Contains(term) ?? false) ||
                    (r["Category"]?.ToString()?.ToLowerInvariant().Contains(term) ?? false));
            }

            if (minPrice.HasValue)
            {
                rows = rows.Where(r =>
                    int.TryParse(r["Price"]?.ToString(), out var p) && p >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                rows = rows.Where(r =>
                    int.TryParse(r["Price"]?.ToString(), out var p) && p <= maxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim();
                rows = rows.Where(r =>
                    string.Equals(r["Category"]?.ToString()?.Trim(), cat, StringComparison.OrdinalIgnoreCase));
            }

            var filtered = dt.Clone();
            foreach (var r in rows)
            {
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
            ValidateProductParams(name, price, stock, unit, category);

            var dt = await SheetsRepo.ReadTableCachedAsync("Product");

            bool exists = dt.AsEnumerable()
                            .Any(r => string.Equals(r["Name"]?.ToString()?.Trim(),
                                                    name, StringComparison.OrdinalIgnoreCase));
            if (exists) throw new Exception("Product already exists");

            // Generar Id localmente
            int newId = dt.AsEnumerable()
                          .Select(r => int.TryParse(r["Id"]?.ToString(), out var n) ? n : 0)
                          .DefaultIfEmpty(0)
                          .Max() + 1;

            SyncQueue.Enqueue(new AppendOp("Product", new object[] { newId, name, price, stock, unit, category }));
            SheetsRepo.Invalidate("Product");
            ChangeLogger.Log(UserSession.SessionUID.ToString(), "Insert", "Product", name);
        }

        public async Task UpdateProductAsync(int id, string name, int price, int stock, int unit, string category)
        {
            if (id <= 0) throw new ArgumentException("Invalid product id");
            name = (name ?? "").Trim();
            category = (category ?? "").Trim();
            ValidateProductParams(name, price, stock, unit, category);

            var dt = await SheetsRepo.ReadTableCachedAsync("Product");
            bool exists = dt.AsEnumerable()
                            .Any(r => string.Equals(r["Name"]?.ToString()?.Trim(), name, StringComparison.OrdinalIgnoreCase)
                                       && SafeInt(r["Id"]) != id);
            if (exists) throw new Exception("Product already exists");

            var (row1, _) = await SheetsRepo.FindRowByAsync("Product", "Id", id.ToString());
            if (row1 == 0) throw new Exception("Product not found");

            SyncQueue.Enqueue(new UpdateRowOp("Product", row1, new object[] { id, name, price, stock, unit, category }));
            SheetsRepo.Invalidate("Product");
            ChangeLogger.Log(UserSession.SessionUID.ToString(), "Update", "Product", $"{id}:{name}");
        }

        public async Task DeleteProductAsync(int id)
        {
            if (id <= 0) return;
            var (row1, _) = await SheetsRepo.FindRowByAsync("Product", "Id", id.ToString());
            if (row1 == 0) return;

            SyncQueue.Enqueue(new DeleteRowOp("Product", row1 - 1)); // 0-based
            SheetsRepo.Invalidate("Product");
            ChangeLogger.Log(UserSession.SessionUID.ToString(), "Delete", "Product", id.ToString());
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
            if (string.IsNullOrWhiteSpace(name) || price <= 0) return false;

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

        private static void ValidateProductParams(string name, int price, int stock, int unit, string category)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
            if (price <= 0) throw new ArgumentException("Price must be positive", nameof(price));
            if (stock < 0) throw new ArgumentException("Stock cannot be negative", nameof(stock));
            if (unit <= 0) throw new ArgumentException("Unit must be positive", nameof(unit));
            if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required", nameof(category));
        }
    }
}
