using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Collections.Generic;

namespace InventoryApp.Data
{
    public class ProductManager
    {
        // 1) Obtener todos los productos
        public DataTable GetProducts()
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Product";

            using var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        // 2) Buscar productos por nombre o categoría
        public DataTable SearchProducts(string searchTerm)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT *
                  FROM Product
                 WHERE Name     LIKE @term
                    OR Category LIKE @term";
            cmd.Parameters.AddWithValue("@term", $"%{searchTerm}%");

            using var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        // 3) Obtener categorías para ComboBox
        public string[] GetCategoryItems()
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT CategoryItem FROM Category";

            using var reader = cmd.ExecuteReader();
            var list = new List<string>();
            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            return list.ToArray();
        }

        // 4) Insertar un nuevo producto
        public void InsertProduct(string name, int price, int stock, int unit, string category)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Product (Name, Price, Stock, Unit, Category)
                VALUES (@name, @price, @stock, @unit, @category)";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@stock", stock);
            cmd.Parameters.AddWithValue("@unit", unit);
            cmd.Parameters.AddWithValue("@category", category);
            cmd.ExecuteNonQuery();
        }

        // 5) Actualizar un producto existente
        public void UpdateProduct(int id, string name, int price, int stock, int unit, string category)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE Product
                   SET Name     = @name,
                       Price    = @price,
                       Stock    = @stock,
                       Unit     = @unit,
                       Category = @category
                 WHERE Id       = @id";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@stock", stock);
            cmd.Parameters.AddWithValue("@unit", unit);
            cmd.Parameters.AddWithValue("@category", category);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // 6) Eliminar un producto
        public void DeleteProduct(int id)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Product WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // 7) Insertar una nueva categoría
        public void InsertCategory(string categoryItem)
        {
            using var con = ConnectionManager.GetConnection();
            using var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO Category (CategoryItem) VALUES (@categoryitem)";
            cmd.Parameters.AddWithValue("@categoryitem", categoryItem);
            cmd.ExecuteNonQuery();
        }

        // 8) Agregar un ítem al carrito (o actualizar cantidad si ya existe)
        public static bool AddItemToCart(string name, int price)
        {
            using var con = ConnectionManager.GetConnection();

            // 8.1) Verificar si ya está en el carrito para el usuario actual
            using var selectCmd = con.CreateCommand();
            selectCmd.CommandText = @"
                SELECT Quantity, Price
                  FROM Cart
                 WHERE Name = @name
                   AND Uid  = @uid";
            selectCmd.Parameters.AddWithValue("@name", name);
            selectCmd.Parameters.AddWithValue("@uid", UserSession.SessionUID);

            using var reader = selectCmd.ExecuteReader();
            if (reader.Read())
            {
                var existingQuantity = reader.GetInt32(0);
                var existingPrice = reader.GetInt32(1);
                var newQuantity = existingQuantity + 1;
                reader.Close();

                using var updateCmd = con.CreateCommand();
                updateCmd.CommandText = @"
                    UPDATE Cart
                       SET Quantity = @quantity,
                           Price    = @price
                     WHERE Name     = @name
                       AND Uid      = @uid";
                updateCmd.Parameters.AddWithValue("@quantity", newQuantity);
                updateCmd.Parameters.AddWithValue("@price", existingPrice);
                updateCmd.Parameters.AddWithValue("@name", name);
                updateCmd.Parameters.AddWithValue("@uid", UserSession.SessionUID);
                updateCmd.ExecuteNonQuery();
            }
            else
            {
                reader.Close();

                using var insertCmd = con.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO Cart (ProductId, Uid, Name, Price, Quantity)
                    VALUES (
                        (SELECT Id FROM Product WHERE Name = @name),
                        @uid,
                        @name,
                        @price,
                        1
                    )";
                insertCmd.Parameters.AddWithValue("@name", name);
                insertCmd.Parameters.AddWithValue("@uid", UserSession.SessionUID);
                insertCmd.Parameters.AddWithValue("@price", price);
                insertCmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}
