// Data/SheetsRepo.cs
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RapiMesa.Data
{
    public static class SheetsRepo
    {
        private static SheetsService Svc => SheetsClient.Service;
        private static string Sid => SheetsClient.SpreadsheetId;

        // Lee toda la hoja (con encabezados) a DataTable
        public static async Task<DataTable> ReadTableAsync(string sheet, string rangeA1 = null)
        {
            var range = rangeA1 ?? $"{sheet}!A1:Z";
            var resp = await Svc.Spreadsheets.Values.Get(Sid, range).ExecuteAsync();

            var dt = new DataTable(sheet);
            if (resp.Values == null || resp.Values.Count == 0) return dt;

            var headers = resp.Values[0].Select(c => c?.ToString() ?? "").ToArray();
            foreach (var h in headers) dt.Columns.Add(h);

            foreach (var row in resp.Values.Skip(1))
            {
                var arr = headers.Select((_, i) => i < row.Count ? row[i] : null).ToArray();
                dt.Rows.Add(arr);
            }
            return dt;
        }

        // Append una fila (RAW)
        public static async Task AppendRowAsync(string sheet, IList<object> values)
        {
            var body = new ValueRange { Values = new[] { values } };
            var req = Svc.Spreadsheets.Values.Append(body, Sid, $"{sheet}!A1:Z");
            req.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            await req.ExecuteAsync();
        }

        public static async Task UpdateCellAsync(string a1, object value)
        {
            var body = new ValueRange { Values = new[] { new List<object> { value } } };
            var req = SheetsClient.Service.Spreadsheets.Values.Update(body, SheetsClient.SpreadsheetId, a1);
            req.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await req.ExecuteAsync();
        }

        // Update fila completa en rango A#:Z# con valores
        public static async Task UpdateRowAsync(string sheet, int rowIndex1, IList<object> values)
        {
            var range = $"{sheet}!A{rowIndex1}";
            var body = new ValueRange { Values = new[] { values } };
            var req = Svc.Spreadsheets.Values.Update(body, Sid, range);
            req.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await req.ExecuteAsync();
        }

        // Borra una fila (delete dimension)
        public static async Task DeleteRowAsync(string sheet, int rowIndex0) // 0-based (incluye header)
        {
            var req = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
                {
                    new Request {
                      DeleteDimension = new DeleteDimensionRequest {
                        Range = new DimensionRange {
                          SheetId = await GetSheetIdAsync(sheet),
                          Dimension = "ROWS",
                          StartIndex = rowIndex0,
                          EndIndex = rowIndex0 + 1
                        }
                      }
                    }
                }
            };
            await Svc.Spreadsheets.BatchUpdate(req, Sid).ExecuteAsync();
        }

        // Helpers
        public static async Task<int> GetSheetIdAsync(string sheet)
        {
            var ss = await Svc.Spreadsheets.Get(Sid).ExecuteAsync();
            return ss.Sheets.First(s => s.Properties.Title == sheet).Properties.SheetId ?? 0;
        }

        // Busca la fila (1-based) por igualdad exacta en una columna (por nombre de encabezado)
        public static async Task<(int row1, DataRow row)> FindRowByAsync(string sheet, string columnName, string equalsValue)
        {
            var dt = await ReadTableAsync(sheet);
            if (!dt.Columns.Contains(columnName)) return (0, null);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (string.Equals(dt.Rows[i][columnName]?.ToString(), equalsValue, StringComparison.OrdinalIgnoreCase))
                {
                    // row1 = i + 2 (porque fila 1 es encabezado)
                    return (i + 2, dt.Rows[i]);
                }
            }
            return (0, null);
        }

        // Siguiente Id incremental (max + 1) en columna "Id"
        public static async Task<int> NextIdAsync(string sheet, string idColumn = "Id")
        {
            var dt = await ReadTableAsync(sheet);
            int max = 0;
            foreach (DataRow r in dt.Rows)
                if (int.TryParse(r[idColumn]?.ToString(), out var v) && v > max) max = v;
            return max + 1;
        }
    }
}
