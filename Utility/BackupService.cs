using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RapiMesa.Data;

namespace RapiMesa.Utility
{
    public static class BackupService
    {
        private static Timer _timer;

        public static void Start(TimeSpan? interval = null)
        {
            if (_timer != null) return;
            var span = interval ?? TimeSpan.FromHours(1);
            _timer = new Timer(async _ => await RunBackupAsync(), null, span, span);
        }

        private static async Task RunBackupAsync()
        {
            try
            {
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db", "backups");
                Directory.CreateDirectory(folder);
                await SheetsRepo.BackupAllAsync(folder);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
    }
}
