using System.Data;
using Microsoft.Data.Sqlite;

namespace InventoryApp.Data
{
    public class CategoryManager
    {
        // Fetch data from Category
        public DataTable GetCategories()
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Category";

            var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);  // Carga columnas y filas desde el reader

            return dt;
        }

        // Add new Category
        public void AddCategory(string categoryItem)
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO Category (CategoryItem) VALUES (@categoryitem)";
            cmd.Parameters.AddWithValue("@categoryitem", categoryItem);
            cmd.ExecuteNonQuery();
        }

        // Update Category
        public void UpdateCategory(int id, string categoryItem)
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Category SET CategoryItem = @categoryitem WHERE ID = @id";
            cmd.Parameters.AddWithValue("@categoryitem", categoryItem);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // Delete Category
        public void DeleteCategory(int id)
        {
            var con = ConnectionManager.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Category WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
