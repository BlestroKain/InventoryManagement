using System;
using Google.Apis.Sheets.v4;

namespace RapiMesa.Data
{
    public static class ConnectionManager
    {
        private static bool _initialized;

        public static void Init(string clientSecretPath, string spreadsheetId)
        {
            if (_initialized) return;
            SheetsClient.Init(clientSecretPath, spreadsheetId);
            _initialized = true;
        }

        public static SheetsService GetService()
        {
            if (!_initialized)
                throw new InvalidOperationException("Sheets no inicializado. Llama a ConnectionManager.Init(...) en Program.Main.");
            return SheetsClient.Service;
        }

        public static string SpreadsheetId
        {
            get
            {
                if (!_initialized)
                    throw new InvalidOperationException("Sheets no inicializado. Llama a ConnectionManager.Init(...) primero.");
                return SheetsClient.SpreadsheetId;
            }
        }
    }
}
