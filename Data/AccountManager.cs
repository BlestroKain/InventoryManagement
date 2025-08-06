using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace InventoryApp.Data
{
    class AccountManager
    {
        // Valida credenciales de usuario
        public int ValidateUserCredentials(string username, string password)
        {
            int uid = 0;

            using (SqliteConnection con = ConnectionManager.GetConnection())
            {
                using (SqliteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT Uid FROM Account WHERE Username = @Username AND Password = @Password";
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        uid = Convert.ToInt32(result);
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
            {
                using (SqliteCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO Account (Username, Password) VALUES (@Username, @Password)";
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

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
        }

        // Verifica si ya existe el username
        private bool IsUsernameExists(string username)
        {
            bool exists = false;

            using (SqliteConnection con = ConnectionManager.GetConnection())
            {
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
            }

            return exists;
        }
    }
}
