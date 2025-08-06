using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;

namespace InventoryApp.Data
{
    class AccountManager
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; // 256 bits
        private const int Iterations = 10000;

        private static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(KeySize);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(KeySize);

            return FixedTimeEquals(expectedHash, computedHash);
        }

        // Comparison en tiempo constante para evitar timing attacks
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        // Valida credenciales de usuario
        public int ValidateUserCredentials(string username, string password)
        {
            int uid = 0;
            using (var con = ConnectionManager.GetConnection())
            using (var cmd = con.CreateCommand())
            {
                // 1) Traigo el hash almacenado
                cmd.CommandText = "SELECT Uid, Password FROM Account WHERE Username = @Username";
                cmd.Parameters.AddWithValue("@Username", username);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var storedHash = reader.GetString(1);
                    // 2) Verifico contra el hash
                    if (VerifyPassword(password, storedHash))
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

            using (var con = ConnectionManager.GetConnection())
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Account (Username, Password) VALUES (@Username, @Password)";
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", HashPassword(password));

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    MessageBox.Show("Registration successful!", "Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to register. Please try again.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Verifica si ya existe el username
        private bool IsUsernameExists(string username)
        {
            bool exists = false;

            using (var con = ConnectionManager.GetConnection())
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Account WHERE Username = @Username";
                cmd.Parameters.AddWithValue("@Username", username);

                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    exists = Convert.ToInt32(result) > 0;
                }
            }

            return exists;
        }
    }
}
