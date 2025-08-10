// Data/CartManagerSheets.cs
using RapiMesa.Utility;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RapiMesa.Data
{
    public class CartManager
    {
        public async Task<DataTable> GetCartItemsAsync()
        {
            var dt = await SheetsRepo.ReadTableCachedAsync("Cart"); // Id | ProductId | Uid | Name | Price | Quantity
            var filtered = dt.Clone();
            foreach (DataRow r in dt.Rows)
            {
                int.TryParse(r["Uid"]?.ToString(), out var u);
                if (u == UserSession.SessionUID) filtered.Rows.Add(r.ItemArray);
            }
            return filtered;
        }

        // ⬇⬇ usa cartId y solo actualiza la celda F (Quantity) de esa fila
        public async Task UpdateQuantityInCartAsync(int cartId, int quantity)
        {
            var (row1, row) = await SheetsRepo.FindRowByAsync("Cart", "Id", cartId.ToString());
            if (row1 == 0 || row == null) return;

            // A:Id B:ProductId C:Uid D:Name E:Price F:Quantity
            var range = $"Cart!F{row1}";
            await SheetsRepo.UpdateCellAsync(range, quantity);
        }

        public async Task RemoveCartItemAsync(int cartId)
        {
            var (row1, _) = await SheetsRepo.FindRowByAsync("Cart", "Id", cartId.ToString());
            if (row1 == 0) return;
            await SheetsRepo.DeleteRowAsync("Cart", row1 - 1);
        }

        public async Task<decimal> GetTotalPriceAsync()
        {
            var dt = await GetCartItemsAsync();
            decimal total = 0;
            foreach (DataRow r in dt.Rows)
            {
                decimal.TryParse((r["Price"]?.ToString() ?? "0").Replace("$", "").Replace(",", ""), out var price);
                int.TryParse(r["Quantity"]?.ToString(), out var qty);
                total += price * qty;
            }
            return total;
        }

        public async Task<int> GetCartItemCountAsync()
        {
            var dt = await GetCartItemsAsync();
            return dt.Rows.Count;
        }
    }
}
