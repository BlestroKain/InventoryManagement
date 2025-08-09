using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RapiMesa.Data
{
    internal class TransactionManager
    {
        public async Task SaveTransactionAsync(
            string transactionId,
            decimal subtotal,
            decimal cash,
            double discountPercent,
            double discountAmount,
            double change,
            DateTime currentDate,
            double total
        )
        {
            int uid = UserSession.SessionUID;

            // 1) Leer carrito del usuario
            DataTable cart = await SheetsRepo.ReadTableAsync("Cart");
            var myCartRows = new List<Tuple<int, DataRow>>(); // (row1, row)
            int i = 0;
            foreach (DataRow r in cart.Rows)
            {
                i++;
                int rowUid;
                int.TryParse(r["Uid"]?.ToString(), out rowUid);
                if (rowUid == uid)
                {
                    // +1 por el header
                    myCartRows.Add(Tuple.Create(i + 1, r));
                }
            }

            if (myCartRows.Count == 0)
                throw new InvalidOperationException("El carrito está vacío para el usuario actual.");

            // 2) Armar items (resolver ProductId si falta)
            var items = new List<CartItem>();
            foreach (var pair in myCartRows)
            {
                var r = pair.Item2;

                string name = r["Name"]?.ToString() ?? "";
                int qty = ToInt(r["Quantity"]);
                int productId = ToInt(r["ProductId"]);
                if (productId == 0)
                {
                    var tuple = await SheetsRepo.FindRowByAsync("Product", "Name", name);
                    int prow1 = tuple.Item1;
                    DataRow prow = tuple.Item2;
                    productId = (prow != null) ? ToInt(prow["Id"]) : 0;
                }
                decimal price = ToDec(r["Price"]);
                items.Add(new CartItem { ProductId = productId, Name = name, Quantity = qty, Price = price });
            }

            // 2b) Merge items para no actualizar stock repetido
            //     Clave de agrupación simple y compatible (string)
            var merged = items
                .GroupBy(x => (x.ProductId != 0 ? ("ID:" + x.ProductId) : ("NAME:" + (x.Name ?? ""))).ToLowerInvariant())
                .Select(g =>
                {
                    var first = g.First();
                    return new CartItem
                    {
                        ProductId = first.ProductId,
                        Name = first.Name,
                        Quantity = g.Sum(x => x.Quantity),
                        Price = first.Price
                    };
                })
                .ToList();

            // 3) Descontar stock y registrar History
            foreach (var it in merged)
            {
                if (it.ProductId == 0) continue;

                var tuple = await SheetsRepo.FindRowByAsync("Product", "Id", it.ProductId.ToString());
                int row1 = tuple.Item1;
                DataRow prow = tuple.Item2;
                if (row1 == 0 || prow == null) continue;

                int oldStock = ToInt(prow["Stock"]);
                int newStock = Math.Max(0, oldStock - it.Quantity);

                await SheetsRepo.UpdateRowAsync("Product", row1, new object[]
                {
                    ToInt(prow["Id"]),
                    prow["Name"]?.ToString() ?? "",
                    ToInt(prow["Price"]),
                    newStock,
                    ToInt(prow["Unit"]),
                    prow["Category"]?.ToString() ?? ""
                });

                // History: salida negativa
                int histId = await SheetsRepo.NextIdAsync("History", "Id");
                string nowIso = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                await SheetsRepo.AppendRowAsync("History", new object[] { histId, it.ProductId, -it.Quantity, nowIso });
            }

            // 4) Cabecera Transaction
            await SheetsRepo.AppendRowAsync("Transaction", new object[]
            {
                transactionId,
                subtotal,
                cash,
                discountPercent,
                discountAmount,
                change,
                total,
                currentDate.ToString("yyyy-MM-dd HH:mm:ss"),
                uid
            });

            // 5) Líneas Orders
            foreach (var it in items)
            {
                int orderId = await SheetsRepo.NextIdAsync("Orders", "Id");
                await SheetsRepo.AppendRowAsync("Orders", new object[] { orderId, transactionId, it.Name, it.Price, it.Quantity });
            }

            // 6) Limpiar carrito (descendente)
            foreach (var pair in myCartRows.OrderByDescending(x => x.Item1))
            {
                int row1 = pair.Item1;
                await SheetsRepo.DeleteRowAsync("Cart", row1 - 1); // API es 0-based incluyendo header
            }
        }

        // helpers
        private static int ToInt(object v)
        {
            int x;
            return int.TryParse(v?.ToString(), out x) ? x : 0;
        }

        private static decimal ToDec(object v)
        {
            if (v == null) return 0m;
            var s = v.ToString().Trim().Replace("$", "").Replace(",", "");
            decimal d;
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                return d;
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out d))
                return d;
            return 0m;
        }

        private class CartItem
        {
            public int ProductId;
            public string Name;
            public int Quantity;
            public decimal Price;
        }
    }
}
