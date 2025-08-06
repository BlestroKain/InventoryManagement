using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace InventoryApp.Data
{
    internal class TransactionManager
    {
        // Guarda la transacción de forma atómica junto con los items y la limpieza del carrito
        public void SaveTransactionToDatabase(
            string transactionId,
            decimal subtotal,
            decimal cash,
            double discountPercent,
            double discountAmount,
            double change,
            DateTime currentDate,
            double total,
            ListBox listBox
        )
        {
            using var con = ConnectionManager.GetConnection();
            using var transaction = con.BeginTransaction();

            try
            {
                // Actualizar stock
                using (var updateCmd = con.CreateCommand())
                {
                    updateCmd.Transaction = transaction;
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

                // Insertar cabecera de transacción
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = transaction;
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

                // Insertar items de la orden
                using (var itemCmd = con.CreateCommand())
                {
                    itemCmd.Transaction = transaction;
                    itemCmd.CommandText = @"
                    INSERT INTO Orders (TransactionId, Name, Price, Quantity)
                    VALUES (@TransactionId, @Name, @Price, @Quantity)";

                    foreach (var item in listBox.Items)
                    {
                        var parts = item.ToString().Split(new[] { " x ", " - $" }, StringSplitOptions.None);
                        var quantity = int.Parse(parts[0]);
                        var name = parts[1];
                        var price = decimal.Parse(parts[2]);

                        itemCmd.Parameters.Clear();
                        itemCmd.Parameters.AddWithValue("@TransactionId", transactionId);
                        itemCmd.Parameters.AddWithValue("@Name", name);
                        itemCmd.Parameters.AddWithValue("@Price", price);
                        itemCmd.Parameters.AddWithValue("@Quantity", quantity);

                        itemCmd.ExecuteNonQuery();
                    }
                }

                // Limpiar carrito
                using (var deleteCmd = con.CreateCommand())
                {
                    deleteCmd.Transaction = transaction;
                    deleteCmd.CommandText = "DELETE FROM Cart WHERE Uid = @Uid";
                    deleteCmd.Parameters.AddWithValue("@Uid", UserSession.SessionUID);
                    deleteCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

