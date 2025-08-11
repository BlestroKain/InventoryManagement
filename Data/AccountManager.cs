using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using RapiMesa.Utility;

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
            var dt = await SheetsRepo.ReadTableCachedAsync("Account");
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
                    var roleVal = dt.Columns.Contains("Role") ? r["Role"]?.ToString() : "";
                    await SheetsRepo.UpdateRowAsync("Account", row1,
                        new object[] { uid, user, HashPassword(password), roleVal });
                    return uid;
                }

                return 0; // usuario encontrado pero password no coincide
            }
            return 0; // no existe
        }

        // Obtiene todas las cuentas
        public async Task<DataTable> GetAccountsAsync()
        {
            return await SheetsRepo.ReadTableCachedAsync("Account");
        }

        // Registra usuario (con hash) y rol
        public async Task RegisterUserAsync(string username, string password, UserRole role = UserRole.Cashier)
        {
            if (await IsUsernameExistsAsync(username))
            {
                MessageBox.Show(
                    "El nombre de usuario ya existe. Por favor elige otro.",
                    "Error de registro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int uid = await SheetsRepo.NextIdAsync("Account", "Uid");
            await SheetsRepo.AppendRowAsync("Account",
                new object[] { uid, username, HashPassword(password), role.ToString() });
            SheetsRepo.Invalidate("Account");

            MessageBox.Show(
                "Registro exitoso",
                "Registro",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Comprueba si ya existe el username
        public async Task<bool> IsUsernameExistsAsync(string username, int excludeUid = 0)
        {
            var dt = await SheetsRepo.ReadTableCachedAsync("Account");
            foreach (DataRow r in dt.Rows)
            {
                if (string.Equals(r["Username"]?.ToString(), username, StringComparison.OrdinalIgnoreCase)
                    && ToInt(r["Uid"]) != excludeUid)
                    return true;
            }
            return false;
        }

        // Actualiza usuario existente
        public async Task UpdateUserAsync(int uid, string username, string password, UserRole role)
        {
            if (await IsUsernameExistsAsync(username, uid))
            {
                MessageBox.Show(
                    "El nombre de usuario ya existe.",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var (row1, row) = await SheetsRepo.FindRowByAsync("Account", "Uid", uid.ToString());
            if (row1 <= 0) return;

            string passHash = string.IsNullOrWhiteSpace(password)
                ? row["Password"]?.ToString() ?? ""
                : HashPassword(password);

            await SheetsRepo.UpdateRowAsync("Account", row1,
                new object[] { uid, username, passHash, role.ToString() });
            SheetsRepo.Invalidate("Account");
        }

        // Elimina usuario
        public async Task DeleteUserAsync(int uid)
        {
            var (row1, _) = await SheetsRepo.FindRowByAsync("Account", "Uid", uid.ToString());
            if (row1 <= 0) return;

            await SheetsRepo.DeleteRowAsync("Account", row1 - 1);
            SheetsRepo.Invalidate("Account");
        }

        public async Task<UserRole> GetUserRoleAsync(int uid)
        {
            var dt = await SheetsRepo.ReadTableCachedAsync("Account");
            foreach (DataRow r in dt.Rows)
            {
                if (ToInt(r["Uid"]) == uid)
                {
                    if (dt.Columns.Contains("Role"))
                    {
                        var roleStr = r["Role"]?.ToString();
                        if (Enum.TryParse<UserRole>(roleStr, true, out var role))
                            return role;
                    }
                    break;
                }
            }
            return UserRole.Administrator;
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
