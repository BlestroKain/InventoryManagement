using System;
using System.IO;

namespace RapiMesa.Utility
{
    public static class ErrorLogger
    {
        private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db", "errors.log");

        public static void Log(Exception ex)
        {
            if (ex == null) return;
            try
            {
                var dir = Path.GetDirectoryName(LogPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var line = $"{DateTime.Now:O}|{ex}";
                File.AppendAllLines(LogPath, new[] { line });
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
