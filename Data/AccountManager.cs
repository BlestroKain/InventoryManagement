using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;

namespace InventoryApp.Data
{
    class AccountManager
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32;  // 256 bits
        private const int Iterations = 10000;

        private static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hashToCompare = pbkdf2.GetBytes(KeySize);
            return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
        }

        // Valida credenciales de usuario
        public int ValidateUserCredentials(string username, string password)
        {
            int uid = 0;

            using (SqliteConnection con = ConnectionManager.GetConnection())
            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT Uid, Password FROM Account WHERE Username = @Username";
                cmd.Parameters.AddWithValue("@Username", username);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string stored = reader.GetString(1);
                    if (VerifyPassword(password, stored))
                    {
                        uid = reader.GetInt32(0);
                    }
                }
            }

            return uid;
        }

        // Registra un nuevo usuario
        public void RegisterUser(string username, string password)
        {
            if (IsUsernameExists(username))
            {
                MessageBox.Show(
                    "Username already exists. Please choose a different username.",
                    "Registration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            using (SqliteConnection con = ConnectionManager.GetConnection())
            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO Account (Username, Password) VALUES (@Username, @Password)";
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", HashPassword(password));

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    MessageBox.Show(
                        "Registration successful!",
                        "Registration",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Failed to register. Please try again.",
                        "Registration Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        // Verifica si ya existe el username
        private bool IsUsernameExists(string username)
        {
            bool exists = false;

            using (SqliteConnection con = ConnectionManager.GetConnection())
            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT COUNT(*) FROM Account WHERE Username = @Username";
                cmd.Parameters.AddWithValue("@Username", username);

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    exists = Convert.ToInt32(result) > 0;
                }
            }

            return exists;
        }
    }
}

