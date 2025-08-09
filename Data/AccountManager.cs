using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RapiMesa.Data
{
    class AccountManager
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; // 256 bits
        private const int Iter = 10000;

        // ======= API PÚBLICA (async) =======

        // Valida credenciales; devuelve Uid (>0 si OK, 0 si falla)
        public async Task<int> ValidateUserCredentialsAsync(string username, string password)
        {
            var dt = await SheetsRepo.ReadTableAsync("Account");
            foreach (DataRow r in dt.Rows)
            {
                var user = r["Username"]?.ToString();
                if (!string.Equals(user, username, StringComparison.OrdinalIgnoreCase)) continue;

                var stored = r["Password"]?.ToString() ?? "";
                if (VerifyPassword(password, stored))
                {
                    // OK
                    return ToInt(r["Uid"]);
                }

                // (opcional) compatibilidad: si era texto plano, acepta una vez y re-hashea
                if (!stored.Contains(":") && stored == password)
                {
                    int uid = ToInt(r["Uid"]);
                    int row1 = FindRow1(dt, r); // fila 1-based (incluye header)
                    await SheetsRepo.UpdateRowAsync("Account", row1,
                        new object[] { uid, user, HashPassword(password) });
                    return uid;
                }

                return 0; // usuario encontrado pero password no coincide
            }
            return 0; // no existe
        }

        // Registra usuario (con hash); muestra mensajes como antes
        public async Task RegisterUserAsync(string username, string password)
        {
            if (await IsUsernameExistsAsync(username))
            {
                MessageBox.Show(
                    "Username already exists. Please choose a different username.",
                    "Registration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int uid = await SheetsRepo.NextIdAsync("Account", "Uid");
            await SheetsRepo.AppendRowAsync("Account",
                new object[] { uid, username, HashPassword(password) });

            MessageBox.Show(
                "Registration successful!",
                "Registration",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Comprueba si ya existe el username
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            var dt = await SheetsRepo.ReadTableAsync("Account");
            foreach (DataRow r in dt.Rows)
            {
                if (string.Equals(r["Username"]?.ToString(), username, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        // ======= UTILIDADES =======

        private static int ToInt(object v) =>
            v == null ? 0 : int.TryParse(v.ToString(), out var x) ? x : 0;

        private static int FindRow1(DataTable dt, DataRow row)
        {
            // Fila 1 es encabezado; DataRow index 0 ? fila 2
            int idx = dt.Rows.IndexOf(row);
            return idx + 2;
        }

        // Hash PBKDF2
        private static string HashPassword(string password)
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var salt = new byte[SaltSize];
                rng.GetBytes(salt);
                using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
                    password, salt, Iter, System.Security.Cryptography.HashAlgorithmName.SHA256))
                {
                    var hash = pbkdf2.GetBytes(KeySize);
                    return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
                }
            }
        }

        private static bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split(':');
            if (parts.Length != 2) return false;
            var salt = Convert.FromBase64String(parts[0]);
            var expected = Convert.FromBase64String(parts[1]);

            using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
                password, salt, Iter, System.Security.Cryptography.HashAlgorithmName.SHA256))
            {
                var computed = pbkdf2.GetBytes(KeySize);
                return FixedTimeEquals(expected, computed);
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
