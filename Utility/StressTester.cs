using System.Diagnostics;
using System.Threading.Tasks;
using RapiMesa.Data;

namespace RapiMesa.Utility
{
    public static class StressTester
    {
        public static async Task RunProductInsertTestAsync(int count)
        {
            var pm = new ProductManager();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                try
                {
                    await pm.InsertProductAsync($"Stress{i}", i + 1, i, 1, "Stress");
                }
                catch (System.Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
            }
            sw.Stop();
            ErrorLogger.Log(new System.Exception($"Stress test inserted {count} products in {sw.ElapsedMilliseconds}ms"));
        }
    }
}
