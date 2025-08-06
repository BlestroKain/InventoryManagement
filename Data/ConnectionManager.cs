using System.IO;
using Microsoft.Data.Sqlite;

public static class ConnectionManager
{
    private const string DbFile = "rapimesa.db";
    private static readonly string ConnectionString = $"Data Source={DbFile}";

    public static SqliteConnection GetConnection()
    {
        // (Opcional) Si quieres asegurarte de que el archivo exista antes de abrir:
        if (!File.Exists(DbFile))
        {
            // Basta con crearlo vacío; SQLite lo inicializará al abrir la conexión
            File.Create(DbFile).Dispose();
        }

        var conn = new SqliteConnection(ConnectionString);
        conn.Open();    // Aquí SQLite crea el esquema interno y guarda el .db
        return conn;
    }
}
