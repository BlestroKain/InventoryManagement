// Data/CategoryManagerSheets.cs
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using RapiMesa.Utility;

namespace RapiMesa.Data
{
    public class CategoryManager
    {
        // Lee desde caché (rápido, sin pedir a Sheets cada vez)
        public async Task<DataTable> GetCategoriesAsync()
            => await SheetsRepo.ReadTableCachedAsync("Category");

        public async Task AddCategoryAsync(string categoryItem)
        {
            if (string.IsNullOrWhiteSpace(categoryItem)) return;
            var name = categoryItem.Trim();

            // 1) Validar duplicado contra caché (case-insensitive)
            var dt = await SheetsRepo.ReadTableCachedAsync("Category");
            bool exists = dt.AsEnumerable().Any(r =>
                string.Equals(r["CategoryItem"]?.ToString()?.Trim(), name, StringComparison.OrdinalIgnoreCase)
            );
            if (exists) return;

            // 2) Generar Id local (evita otra llamada a Sheets)
            int newId = 1;
            if (dt.Rows.Count > 0)
            {
                var max = dt.AsEnumerable()
                            .Select(r => int.TryParse(r["Id"]?.ToString(), out var n) ? n : 0)
                            .DefaultIfEmpty(0)
                            .Max();
                newId = max + 1;
            }

            // 3) Encolar append (no bloquea). Invalidamos caché para próxima lectura fresca.
            SyncQueue.Enqueue(new AppendOp("Category", new object[] { newId, name }));
            SheetsRepo.Invalidate("Category");
        }

        public async Task UpdateCategoryAsync(int id, string categoryItem)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(categoryItem)) return;
            var name = categoryItem.Trim();

            // 1) Validar duplicado (permitiendo mismo Id)
            var dt = await SheetsRepo.ReadTableCachedAsync("Category");
            bool dup = dt.AsEnumerable().Any(r =>
            {
                int rid = int.TryParse(r["Id"]?.ToString(), out var n) ? n : 0;
                var rname = r["CategoryItem"]?.ToString()?.Trim();
                return rid != id && string.Equals(rname, name, StringComparison.OrdinalIgnoreCase);
            });
            if (dup) return;

            // 2) Buscar row1 (1-based) y encolar update
            var found = await SheetsRepo.FindRowByAsync("Category", "Id", id.ToString());
            int row1 = found.Item1;
            if (row1 == 0) return;

            SyncQueue.Enqueue(new UpdateRowOp("Category", row1, new object[] { id, name }));
            SheetsRepo.Invalidate("Category");
        }

        public async Task DeleteCategoryAsync(int id)
        {
            if (id <= 0) return;

            var found = await SheetsRepo.FindRowByAsync("Category", "Id", id.ToString());
            int row1 = found.Item1; // 1-based (incluye header)
            if (row1 == 0) return;

            // DeleteRowOp pide índice 0-based (incluye header)
            SyncQueue.Enqueue(new DeleteRowOp("Category", row1 - 1));
            SheetsRepo.Invalidate("Category");
        }
    }
}
