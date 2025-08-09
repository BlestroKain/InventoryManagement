using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System.Threading;

namespace RapiMesa.Data
{
    public static class SheetsClient
    {
       public static SheetsService Service;
        public static string SpreadsheetId;

        public static void Init(string clientSecretPath, string spreadsheetId)
        {
            if (Service != null) return;

            string[] scopes = { SheetsService.Scope.Spreadsheets };
            UserCredential credential;

            using (var stream = new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read))
            {
                string tokenPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MiInventario-GoogleToken"
                );

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",                         // identificador del usuario local
                    CancellationToken.None,
                    new FileDataStore(tokenPath, true)
                ).Result;
            }

            Service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "MiInventario-App"
            });

            SpreadsheetId = spreadsheetId;
        }

        public static async Task<IList<IList<object>>> ReadProductsAsync()
        {
            var req = Service.Spreadsheets.Values.Get(SpreadsheetId, "Product!A2:F");
            var resp = await req.ExecuteAsync();
            return resp.Values ?? new List<IList<object>>();
        }

        public static async Task AppendCartItemAsync(int productId, int uid, string name, decimal price, int quantity)
        {
            var body = new ValueRange
            {
                Values = new[] { new List<object> { productId, uid, name, price, quantity } }
            };
            var append = Service.Spreadsheets.Values.Append(body, SpreadsheetId, "Cart!A2:E");
            append.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            await append.ExecuteAsync();
        }

        public static async Task UpdateProductStockAsync(int rowIndex1Based, int newStock)
        {
            var range = $"Product!D{rowIndex1Based}";
            var body = new ValueRange { Values = new[] { new List<object> { newStock } } };

            var upd = Service.Spreadsheets.Values.Update(body, SpreadsheetId, range);
            upd.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await upd.ExecuteAsync();
        }
    }
}
