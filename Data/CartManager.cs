using System;
using System.Data;
using System.Threading.Tasks;

namespace RapiMesa.Data
{
    public class CartManager
    {
        // Devuelve solo las filas del usuario actual
        public async Task<DataTable> GetCartItemsAsync()
        {
            var dt = await SheetsRepo.ReadTableAsync("Cart"); // Cart: Id | ProductId | Uid | Name | Price | Quantity
            var filtered = dt.Clone();

            foreach (DataRow r in dt.Rows)
            {
                int.TryParse(r["Uid"]?.ToString(), out var u);
                if (u == UserSession.SessionUID) filtered.Rows.Add(r.ItemArray);
            }
            return filtered;
        }

        // Actualiza cantidad usando el Id del carrito (NO productId)
        public async Task UpdateQuantityInCartAsync(int cartId, int quantity)
        {
            // buscamos la fila exacta por Id
            var tuple = await SheetsRepo.FindRowByAsync("Cart", "Id", cartId.ToString());
            int row1 = tuple.Item1;
            var row  = tuple.Item2;

            if (row1 == 0 || row == null) return;

            // Reescribimos la fila completa con la nueva cantidad
            await SheetsRepo.UpdateRowAsync("Cart", row1, new object[] {
                row["Id"],
                row["ProductId"],
                row["Uid"],
                row["Name"],
                row["Price"],
                quantity
            });
        }

        // Elimina una fila del carrito por Id
        public async Task RemoveCartItemAsync(int cartId)
        {
            var tuple = await SheetsRepo.FindRowByAsync("Cart", "Id", cartId.ToString());
            int row1 = tuple.Item1;
            if (row1 == 0) return;

            // DeleteRowAsync usa índice 0-based incluyendo header
            await SheetsRepo.DeleteRowAsync("Cart", row1 - 1);
        }

        // Total del carrito del usuario actual
        public async Task<decimal> GetTotalPriceAsync()
        {
            var dt = await GetCartItemsAsync();
            decimal total = 0;

            foreach (DataRow r in dt.Rows)
            {
                decimal price = SafeDec(r["Price"]);
                int qty       = SafeInt(r["Quantity"]);
                total += price * qty;
            }
            return total;
        }

        public async Task<int> GetCartItemCountAsync()
        {
            var dt = await GetCartItemsAsync();
            return dt.Rows.Count;
        }

        // -------- helpers --------
        private static int SafeInt(object v)
            => int.TryParse(v?.ToString(), out var x) ? x : 0;

        private static decimal SafeDec(object v)
        {
            if (v == null) return 0m;
            var s = v.ToString().Replace("$", "").Trim();
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                                 System.Globalization.CultureInfo.InvariantCulture, out var d))
                return d;
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                                 System.Globalization.CultureInfo.CurrentCulture, out d))
                return d;
            return 0m;
        }

    }
}
