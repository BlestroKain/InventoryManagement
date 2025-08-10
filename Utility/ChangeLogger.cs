using System;
using System.IO;

namespace RapiMesa.Utility
{
    public static class ChangeLogger
    {
        private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db", "changes.log");

        public static void Log(string user, string action, string entity, string details)
        {
            try
            {
                var dir = Path.GetDirectoryName(LogPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var line = $"{DateTime.Now:O}|{user}|{action}|{entity}|{details}";
                File.AppendAllLines(LogPath, new[] { line });
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
