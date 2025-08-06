using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace InventoryApp.Data
{
    internal class TransactionManager
    {
        // 1) Insertar cada ítem de la lista en la tabla Orders
        public void InsertTransactionItems(ListBox listBox, string transactionId)
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Orders (TransactionId, Name, Price, Quantity)
                VALUES (@TransactionId, @Name, @Price, @Quantity)";

            foreach (var item in listBox.Items)
            {
                // Parsear "3 x Zanahoria - $50"
                var parts = item.ToString()
                                .Split(new[] { " x ", " - $" }, StringSplitOptions.None);
                var quantity = int.Parse(parts[0]);
                var name = parts[1];
                var price = decimal.Parse(parts[2]);

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Quantity", quantity);

                cmd.ExecuteNonQuery();
            }
        }

        // 2) Guardar la transacción y actualizar stocks
        public void SaveTransactionToDatabase(
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
            var con = ConnectionManager.GetConnection();

            // 2.1) Actualizar stock usando subconsulta de Cart
            using (var updateCmd = con.CreateCommand())
            {
                updateCmd.CommandText = @"
                    UPDATE Product
                       SET Stock = Stock - (
                           SELECT Quantity
                             FROM Cart
                            WHERE Cart.ProductId = Product.Id
                              AND Cart.Uid       = @Uid
                       )
                     WHERE Id IN (
                           SELECT ProductId FROM Cart WHERE Uid = @Uid
                       )";
                updateCmd.Parameters.AddWithValue("@Uid", UserSession.SessionUID);
                updateCmd.ExecuteNonQuery();
            }

            // 2.2) Insertar cabecera de transacción
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO ""Transaction"" 
                        (TransactionId, Subtotal, Cash, DiscountPercent, DiscountAmount, ChangeAmt, Total, Date, Uid)
                    VALUES
                        (@TransactionId, @Subtotal, @Cash, @DiscountPercent, @DiscountAmount, @ChangeAmt, @Total, @Date, @Uid)";

                cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                cmd.Parameters.AddWithValue("@Cash", cash);
                cmd.Parameters.AddWithValue("@DiscountPercent", discountPercent);
                cmd.Parameters.AddWithValue("@DiscountAmount", discountAmount);
                cmd.Parameters.AddWithValue("@ChangeAmt", change);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@Date", currentDate);
                cmd.Parameters.AddWithValue("@Uid", UserSession.SessionUID);

                cmd.ExecuteNonQuery();
            }
        }

        // 3) Borrar todo el carrito del usuario tras la transacción
        public void DeleteCartData()
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Cart WHERE Uid = @Uid";
            cmd.Parameters.AddWithValue("@Uid", UserSession.SessionUID);
            cmd.ExecuteNonQuery();
        }
    }
}
