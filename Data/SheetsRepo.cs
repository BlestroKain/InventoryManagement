// Data/SheetsRepo.cs
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
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
        private static readonly ConcurrentDictionary<string, (DateTime until, DataTable dt)> _cache
      = new ConcurrentDictionary<string, (DateTime, DataTable)>();

        public static async Task<DataTable> ReadTableCachedAsync(string sheet, int ttlSeconds = 45)
        {
            if (_cache.TryGetValue(sheet, out var hit) && DateTime.UtcNow < hit.until)
                return hit.dt.Copy(); // devolver copia para no mutar en UI

            var fresh = await ReadTableAsync(sheet);   // tu método actual que lee de Sheets
            _cache[sheet] = (DateTime.UtcNow.AddSeconds(ttlSeconds), fresh);
            return fresh.Copy();
        }

        // Si necesitas invalidar manualmente tras escribir:
        public static void Invalidate(string sheet)
        {
            _cache.TryRemove(sheet, out _);
        }
        // Append una fila (RAW)
        public static async Task AppendRowAsync(string sheet, IList<object> values)
        {
            var body = new ValueRange { Values = new[] { values } };
            var req = Svc.Spreadsheets.Values.Append(body, Sid, $"{sheet}!A1:Z");
            req.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            await req.ExecuteAsync();
        }

        public static async Task UpdateCellAsync(string a1Range, object value)
        {
            // a1Range ejemplo: "Cart!F12"
            var excl = a1Range.IndexOf('!');
            if (excl <= 0) throw new ArgumentException("Rango A1 inválido", nameof(a1Range));

            var sheetName = a1Range.Substring(0, excl);

            var body = new ValueRange
            {
                Values = new[] { new System.Collections.Generic.List<object> { value ?? "" } }
            };
            var req = SheetsClient.Service.Spreadsheets.Values.Update(SheetsClient.ValueRange(body), SheetsClient.SpreadsheetId, a1Range);
            req.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await req.ExecuteAsync();

            Invalidate(sheetName); // para que la próxima lectura venga fresca
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
            var dt = await ReadTableCachedAsync(sheet);
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
            var dt = await ReadTableCachedAsync(sheet);
            int max = 0;
            foreach (DataRow r in dt.Rows)
                if (int.TryParse(r[idColumn]?.ToString(), out var v) && v > max) max = v;
            return max + 1;
        }

        public static async Task BackupAllAsync(string folder)
        {
            var ss = await Svc.Spreadsheets.Get(Sid).ExecuteAsync();
            foreach (var sh in ss.Sheets)
            {
                var name = sh.Properties.Title;
                var dt = await ReadTableAsync(name);
                var path = Path.Combine(folder, $"{DateTime.Now:yyyyMMdd_HHmmss}_{name}.csv");
                WriteCsv(dt, path);
            }
        }

        private static void WriteCsv(DataTable dt, string path)
        {
            using var sw = new StreamWriter(path);
            var headers = dt.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName));
            sw.WriteLine(string.Join(",", headers));
            foreach (DataRow row in dt.Rows)
            {
                var fields = dt.Columns.Cast<DataColumn>()
                    .Select(c => EscapeCsv(row[c]?.ToString() ?? string.Empty));
                sw.WriteLine(string.Join(",", fields));
            }
        }

        private static string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            if (input.Contains(",") || input.Contains("\""))
                return $"\"{input.Replace("\"", "\"\"")}\"";
            return input;
        }
    }
}
