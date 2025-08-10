using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using RapiMesa.Data;
using RapiMesa.Utility;

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

            // 0) Tablas cache-first (rápidas); si necesitas “forzar”, has un Refresh en SheetsRepo antes
            var cartDt = await SheetsRepo.ReadTableCachedAsync("Cart");      // Id | ProductId | Uid | Name | Price | Quantity
            var prodDt = await SheetsRepo.ReadTableCachedAsync("Product");   // Id | Name | Price | Stock | Unit | Category
            var ordersDt = await SheetsRepo.ReadTableCachedAsync("Orders");    // Id | TransactionId | Name | Price | Quantity | (Date?) opcional
            var historyDt = await SheetsRepo.ReadTableCachedAsync("History");   // Id | ProductID | Added Stocks | Date

            // 1) Filtrar carrito de este usuario, y guardar el índice 1-based (row1) para borrar luego
            var myCart = new List<(int row1, DataRow row)>();
            int i = 0;
            foreach (DataRow r in cartDt.Rows)
            {
                i++;
                int.TryParse(r["Uid"]?.ToString(), out var rowUid);
                if (rowUid == uid) myCart.Add((i + 1, r)); // +1 por header 1-based
            }
            if (myCart.Count == 0) throw new InvalidOperationException("El carrito está vacío para el usuario actual.");

            // 2) Índices en memoria para evitar FindRowBy en loops
            //    Product por Id y por Name
            var prodById = prodDt.AsEnumerable().ToDictionary(r => ToInt(r["Id"]), r => r);
            var prodByName = prodDt.AsEnumerable().GroupBy(r => (r["Name"]?.ToString() ?? "").Trim().ToLowerInvariant())
                                             .ToDictionary(g => g.Key, g => g.First());
            // Product row1 (1-based) por Id (necesario para UpdateRow)
            var prodRow1ById = new Dictionary<int, int>();
            int pIndex = 0;
            foreach (DataRow r in prodDt.Rows)
            {
                pIndex++;
                prodRow1ById[ToInt(r["Id"])] = pIndex + 1; // header
            }

            // 3) Construir items (resolver ProductId si falta) y merge por producto
            var items = new List<CartItem>();
            foreach (var tpl in myCart)
            {
                var r = tpl.row;
                string name = r["Name"]?.ToString() ?? "";
                int qty = ToInt(r["Quantity"]);
                decimal price = ToDec(r["Price"]);

                int productId = ToInt(r["ProductId"]);
                if (productId == 0)
                {
                    var key = name.Trim().ToLowerInvariant();
                    if (prodByName.TryGetValue(key, out var prow))
                        productId = ToInt(prow["Id"]);
                }

                items.Add(new CartItem { ProductId = productId, Name = name, Quantity = qty, Price = price });
            }

            // merge por ProductId+Name para no duplicar descuentos de stock
            var merged = items
                .GroupBy(x => (x.ProductId != 0 ? $"ID:{x.ProductId}" : $"NAME:{(x.Name ?? "")}".ToLowerInvariant()))
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

            // 4) Actualizar Stock (enqueue), y registrar History (enqueue)
            //    (no await => no te bloquea la UI, la cola maneja reintentos/exponencial)
            //    Calculamos nextId de History localmente
            int nextHistId = historyDt.AsEnumerable().Select(r => ToInt(r["Id"])).DefaultIfEmpty(0).Max() + 1;
            string nowIso = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            foreach (var it in merged)
            {
                if (it.ProductId == 0) continue;
                if (!prodById.TryGetValue(it.ProductId, out var prow)) continue;
                if (!prodRow1ById.TryGetValue(it.ProductId, out var row1)) continue;

                int oldStock = ToInt(prow["Stock"]);
                int newStock = Math.Max(0, oldStock - it.Quantity);

                // Update Product (fila completa)
                var values = new object[]
                {
                    ToInt(prow["Id"]),
                    prow["Name"]?.ToString() ?? "",
                    ToInt(prow["Price"]),
                    newStock,
                    ToInt(prow["Unit"]),
                    prow["Category"]?.ToString() ?? ""
                };
                SyncQueue.Enqueue(new UpdateRowOp("Product", row1, values));

                // Append History: salida negativa
                var histValues = new object[] { nextHistId++, it.ProductId, -it.Quantity, nowIso };
                SyncQueue.Enqueue(new AppendOp("History", histValues));
            }
            SheetsRepo.Invalidate("Product");
            SheetsRepo.Invalidate("History");

            // 5) Cabecera Transaction (enqueue)
            var txValues = new object[]
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
            };
            SyncQueue.Enqueue(new AppendOp("Transaction", txValues));
            SheetsRepo.Invalidate("Transaction");

            // 6) Líneas Orders (enqueue). Calculamos nextId local y agregamos Date para tus gráficos.
            int nextOrderId = ordersDt.AsEnumerable().Select(r => ToInt(r["Id"])).DefaultIfEmpty(0).Max() + 1;
            string orderDateIso = currentDate.ToString("yyyy-MM-dd HH:mm:ss");

            foreach (var it in items)
            {
                var ordValues = new object[]
                {
                    nextOrderId++,
                    transactionId,
                    it.Name,
                    it.Price,
                    it.Quantity,
                    orderDateIso  // <-- agrega Date en Orders (asegurate que la hoja tenga esta 6ta columna)
                };
                SyncQueue.Enqueue(new AppendOp("Orders", ordValues));
            }
            SheetsRepo.Invalidate("Orders");

            // 7) Limpiar el Cart del usuario (enqueue, descendente)
            foreach (var (row1, _) in myCart.OrderByDescending(x => x.row1))
            {
                SyncQueue.Enqueue(new DeleteRowOp("Cart", row1 - 1)); // API usa 0-based con header
            }
            SheetsRepo.Invalidate("Cart");

            // Listo: la cola lo procesa en background.
            // Si quieres bloquear hasta terminar (no lo recomiendo), podrías esperar un “drain”,
            // pero mejor dejar que SyncQueue se encargue.
        }

        private class CartItem
        {
            public int ProductId;
            public string Name;
            public int Quantity;
            public decimal Price;
        }

        // helpers
        private static int ToInt(object v) =>
            int.TryParse(v?.ToString(), out var x) ? x : 0;

        private static decimal ToDec(object v)
        {
            if (v == null) return 0m;
            var s = v.ToString().Trim().Replace("$", "").Replace(",", "");
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                return d;
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out d))
                return d;
            return 0m;
        }
    }
}
