using Microsoft.Data.Sqlite;
using System;

namespace InventoryApp.Data
{
    public class StockManager
    {
        // 1) Obtener el Id del producto por nombre
        public int GetProductIdByName(string itemName)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Id
                  FROM Product
                 WHERE Name = @itemName";
            cmd.Parameters.AddWithValue("@itemName", itemName);

            var result = cmd.ExecuteScalar();
            return (result != null && result != DBNull.Value)
                ? Convert.ToInt32(result)
                : 0;
        }

        // 2) Obtener stock actual por Id
        public int GetCurrentStockById(int productId)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT Stock
                  FROM Product
                 WHERE Id = @productId";
            cmd.Parameters.AddWithValue("@productId", productId);

            var result = cmd.ExecuteScalar();
            return (result != null && result != DBNull.Value)
                ? Convert.ToInt32(result)
                : 0;
        }

        // 3) Actualizar stock del producto
        public void UpdateStock(int productId, int newStock)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE Product
                   SET Stock = @stock
                 WHERE Id    = @productId";
            cmd.Parameters.AddWithValue("@stock", newStock);
            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.ExecuteNonQuery();
        }

        // 4) Insertar entrada en historial de stocks
        public void InsertHistory(int productId, int addedStocks)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO History (ProductID, [Added Stocks], [Date])
                VALUES (@productId, @addedStocks, CURRENT_TIMESTAMP)";
            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@addedStocks", addedStocks);
            cmd.ExecuteNonQuery();
        }

        // 5) (Opcional) Otro método para obtener stock, reutiliza GetCurrentStockById
        public int GetProductStock(int productId)
            => GetCurrentStockById(productId);
    }
}
