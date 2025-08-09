// Data/ProductManagerSheets.cs
using System;
using System.Data;
using System.Threading.Tasks;

namespace RapiMesa.Data
{
    public class ProductManager
    {
        public async Task<DataTable> GetProductsAsync() => await SheetsRepo.ReadTableAsync("Product");

        public async Task<DataTable> SearchProductsAsync(string term)
        {
            var dt = await GetProductsAsync();
            if (string.IsNullOrWhiteSpace(term)) return dt;
            term = term.ToLowerInvariant();
            var filtered = dt.Clone();
            foreach (DataRow r in dt.Rows)
            {
                var name = r["Name"]?.ToString()?.ToLowerInvariant() ?? "";
                var cat  = r["Category"]?.ToString()?.ToLowerInvariant() ?? "";
                if (name.Contains(term) || cat.Contains(term))
                    filtered.Rows.Add(r.ItemArray);
            }
            return filtered;
        }

        public async Task<string[]> GetCategoryItemsAsync()
        {
            var dt = await SheetsRepo.ReadTableAsync("Category");
            var list = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow r in dt.Rows)
                if (!string.IsNullOrWhiteSpace(r["CategoryItem"]?.ToString()))
                    list.Add(r["CategoryItem"].ToString());
            return new System.Collections.Generic.List<string>(list).ToArray();
        }

        public async Task InsertProductAsync(string name, int price, int stock, int unit, string category)
        {
            int id = await SheetsRepo.NextIdAsync("Product");
            await SheetsRepo.AppendRowAsync("Product", new object[] { id, name, price, stock, unit, category });
        }

        public async Task UpdateProductAsync(int id, string name, int price, int stock, int unit, string category)
        {
            var (row1, _) = await SheetsRepo.FindRowByAsync("Product", "Id", id.ToString());
            if (row1 == 0) throw new Exception("Product not found");
            await SheetsRepo.UpdateRowAsync("Product", row1, new object[] { id, name, price, stock, unit, category });
        }

        public async Task DeleteProductAsync(int id)
        {
            // row0 = header(0) + (row1-1)
            var (row1, _) = await SheetsRepo.FindRowByAsync("Product", "Id", id.ToString());
            if (row1 == 0) return;
            await SheetsRepo.DeleteRowAsync("Product", row1 - 1);
        }

        public async Task InsertCategoryAsync(string categoryItem)
        {
            // Evitar duplicados
            var (_, row) = await SheetsRepo.FindRowByAsync("Category", "CategoryItem", categoryItem);
            if (row != null) return;
            int id = await SheetsRepo.NextIdAsync("Category");
            await SheetsRepo.AppendRowAsync("Category", new object[] { id, categoryItem });
        }

        // Add al carrito (Uid + Name)
        public static async Task<bool> AddItemToCartAsync(string name, int price)
        {
            int uid = UserSession.SessionUID;
            // ¿ya existe?
            var dt = await SheetsRepo.ReadTableAsync("Cart");
            DataRow found = null; int foundRow1 = 0; int idx = 0;
            foreach (DataRow r in dt.Rows)
            {
                idx++;
                var n = r["Name"]?.ToString();
                int.TryParse(r["Uid"]?.ToString(), out var u);
                if (string.Equals(n, name, StringComparison.OrdinalIgnoreCase) && u == uid)
                {
                    found = r;
                    foundRow1 = idx + 1; // header=1
                    break;
                }
            }

            if (found != null)
            {
                int q = Convert.ToInt32(found["Quantity"]);
                int p = Convert.ToInt32(found["Price"]); // mantenemos precio
                await SheetsRepo.UpdateRowAsync("Cart", foundRow1,
                    new object[] { found["Id"], found["ProductId"], uid, name, p, q + 1 });
            }
            else
            {
                // Buscar ProductId
                var (prow1, prow) = await SheetsRepo.FindRowByAsync("Product", "Name", name);
                int pid = prow != null ? Convert.ToInt32(prow["Id"]) : 0;
                int id = await SheetsRepo.NextIdAsync("Cart");
                await SheetsRepo.AppendRowAsync("Cart", new object[] { id, pid, uid, name, price, 1 });
            }
            return true;
        }
    }
}
