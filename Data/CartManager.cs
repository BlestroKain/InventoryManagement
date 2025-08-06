using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Windows.Forms;

namespace InventoryApp.Data
{
    public class CartManager
    {
        // 1) Leer carrito en un DataTable
        public DataTable GetCartItems()
        {
            int currentUID = UserSession.SessionUID;

            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Name, Price, Quantity, ProductId
                  FROM Cart
                 WHERE Uid = @Uid";
            cmd.Parameters.AddWithValue("@Uid", currentUID);

            var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);  // carga columnas+filas
            return dt;
        }

        // 2) Actualizar cantidad
        public void UpdateQuantityInCart(int itemId, string quantity)
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE Cart
                   SET Quantity = @quantity
                 WHERE ProductId = @productId";
            cmd.Parameters.AddWithValue("@quantity", quantity);
            cmd.Parameters.AddWithValue("@productId", itemId);
            cmd.ExecuteNonQuery();
        }

        // 3) Total del carrito
        public decimal GetTotalPrice()
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT SUM(Price * Quantity) FROM Cart";
            var result = cmd.ExecuteScalar();

            return (result != null && result != DBNull.Value)
                ? Convert.ToDecimal(result)
                : 0m;
        }

        // 4) Eliminar item
        public void RemoveCartItem(int productId)
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Cart WHERE ProductId = @ProductId";
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.ExecuteNonQuery();
        }

        // 5) Contar items
        public int GetCartItemCount()
        {
            int currentUID = UserSession.SessionUID;

            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Cart WHERE Uid = @Uid";
            cmd.Parameters.AddWithValue("@Uid", currentUID);

            var result = cmd.ExecuteScalar();
            return (result != null && result != DBNull.Value)
                ? Convert.ToInt32(result)
                : 0;
        }

        // 6) Cargar lista en ListBox
        public void LoadCartItems(ListBox listBox)
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Name, Price, Quantity FROM Cart";

            var reader = cmd.ExecuteReader();
            listBox.Items.Clear();

            while (reader.Read())
            {
                var name = reader.GetString(0);
                var price = reader.GetDecimal(1);
                var quantity = reader.GetInt32(2);

                listBox.Items.Add($"{quantity} x {name} - ${price}");
            }
        }
    }
}
