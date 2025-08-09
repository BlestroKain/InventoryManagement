// Data/CategoryManagerSheets.cs
using System.Data;
using System.Threading.Tasks;

namespace RapiMesa.Data
{
    public class CategoryManager
    {
        public async Task<DataTable> GetCategoriesAsync() => await SheetsRepo.ReadTableAsync("Category");

        public async Task AddCategoryAsync(string categoryItem)
        {
            // valida duplicado
            var (_, row) = await SheetsRepo.FindRowByAsync("Category", "CategoryItem", categoryItem);
            if (row != null) return;
            int id = await SheetsRepo.NextIdAsync("Category");
            await SheetsRepo.AppendRowAsync("Category", new object[] { id, categoryItem });
        }

        public async Task UpdateCategoryAsync(int id, string categoryItem)
        {
            var (row1, _) = await SheetsRepo.FindRowByAsync("Category", "Id", id.ToString());
            if (row1 == 0) return;
            await SheetsRepo.UpdateRowAsync("Category", row1, new object[] { id, categoryItem });
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var (row1, _) = await SheetsRepo.FindRowByAsync("Category", "Id", id.ToString());
            if (row1 == 0) return;
            await SheetsRepo.DeleteRowAsync("Category", row1 - 1);
        }
    }
}
