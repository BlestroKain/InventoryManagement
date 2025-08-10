// InventoryApp/Utility/SyncQueue.cs
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using RapiMesa.Data;              // <-- ajusta si tu SheetsRepo está en otro namespace
// using Google; // opcional: si quieres atrapar GoogleApiException

namespace RapiMesa.Utility
{
    // ----- Operaciones -----
    public abstract class SyncOp { }

    public sealed class AppendOp : SyncOp
    {
        public string Sheet { get; private set; }
        public object[] Values { get; private set; }
        public AppendOp(string sheet, object[] values)
        {
            Sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
            Values = values ?? Array.Empty<object>();
        }
    }

    public sealed class UpdateRowOp : SyncOp
    {
        public string Sheet { get; private set; }
        public int Row1 { get; private set; }              // índice 1-based (incluye header como 1)
        public object[] Values { get; private set; }
        public UpdateRowOp(string sheet, int row1, object[] values)
        {
            Sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
            Row1 = row1;
            Values = values ?? Array.Empty<object>();
        }
    }

    public sealed class DeleteRowOp : SyncOp
    {
        public string Sheet { get; private set; }
        public int ZeroBasedIndex { get; private set; }    // índice 0-based (header = 0)
        public DeleteRowOp(string sheet, int zeroBasedIndex)
        {
            Sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
            ZeroBasedIndex = zeroBasedIndex;
        }
    }

    // ----- Cola de sincronización -----
    public static class SyncQueue
    {
        private static readonly BlockingCollection<SyncOp> _queue =
            new BlockingCollection<SyncOp>();

        private static bool _started;

        public static void Start()
        {
            if (_started) return;
            _started = true;
            Task.Run(Worker);
        }

        public static void Enqueue(SyncOp op)
        {
            if (op == null) return;
            _queue.Add(op);
        }

        private static async Task Worker()
        {
            while (true)
            {
                var op = _queue.Take(); // bloquea hasta que haya algo

                // reintentos con backoff simple (1s, 2s, 4s)
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    try
                    {
                        if (op is AppendOp a)
                        {
                            await SheetsRepo.AppendRowAsync(a.Sheet, a.Values);
                            SheetsRepo.Invalidate(a.Sheet);
                        }
                        else if (op is UpdateRowOp u)
                        {
                            await SheetsRepo.UpdateRowAsync(u.Sheet, u.Row1, u.Values);
                            SheetsRepo.Invalidate(u.Sheet);
                        }
                        else if (op is DeleteRowOp d)
                        {
                            await SheetsRepo.DeleteRowAsync(d.Sheet, d.ZeroBasedIndex);
                            SheetsRepo.Invalidate(d.Sheet);
                        }

                        break; // OK, salir del bucle de reintentos
                    }
                    catch (Exception)
                    {
                        // Si fue 429/5xx, esperamos y reintentamos
                        await Task.Delay((int)Math.Pow(2, attempt) * 1000);
                        // Si quieres, aquí puedes inspeccionar el tipo de excepción
                        // (Google.GoogleApiException) y sólo reintentar en 429/5xx.
                    }
                }
            }
        }
    }
}
