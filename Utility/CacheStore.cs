using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapiMesa.Utility
{
    // Infra/CacheStore.cs
    using System.Data;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class CacheStore
    {
        private static readonly object _lock = new object();
        public static DataTable Product { get; private set; }
        public static DataTable Category { get; private set; }
        public static DataTable Cart { get; private set; }
        public static DataTable Transaction { get; private set; }
        public static DataTable Orders { get; private set; }
        public static DataTable History { get; private set; }

        private static string SnapshotPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RapiMesa", "snapshot.bin");

        public static void LoadFromSheets(DataTable product, DataTable category, DataTable cart,
                                          DataTable transaction, DataTable orders, DataTable history)
        {
            lock (_lock)
            {
                Product = product;
                Category = category;
                Cart = cart;
                Transaction = transaction;
                Orders = orders;
                History = history;
            }
            SaveSnapshotSafe();
        }

        public static bool TryLoadSnapshot()
        {
            try
            {
                if (!File.Exists(SnapshotPath)) return false;
                Directory.CreateDirectory(Path.GetDirectoryName(SnapshotPath));
                using (var fs = File.OpenRead(SnapshotPath))
                {
                    var bf = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                    var ds = (DataSet)bf.Deserialize(fs);
#pragma warning restore SYSLIB0011
                    lock (_lock)
                    {
                        Product = ds.Tables["Product"];
                        Category = ds.Tables["Category"];
                        Cart = ds.Tables["Cart"];
                        Transaction = ds.Tables["Transaction"];
                        Orders = ds.Tables["Orders"];
                        History = ds.Tables["History"];
                    }
                    return true;
                }
            }
            catch { return false; }
        }

        public static void SaveSnapshotSafe()
        {
            try
            {
                var ds = new DataSet();
                lock (_lock)
                {
                    if (Product != null) ds.Tables.Add(Product.Copy());
                    if (Category != null) ds.Tables.Add(Category.Copy());
                    if (Cart != null) ds.Tables.Add(Cart.Copy());
                    if (Transaction != null) ds.Tables.Add(Transaction.Copy());
                    if (Orders != null) ds.Tables.Add(Orders.Copy());
                    if (History != null) ds.Tables.Add(History.Copy());
                }
                Directory.CreateDirectory(Path.GetDirectoryName(SnapshotPath));
                using (var fs = File.Create(SnapshotPath))
                {
                    var bf = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                    bf.Serialize(fs, ds);
#pragma warning restore SYSLIB0011
                }
            }
            catch { /* noop */ }
        }

        public static DataTable Copy(string name)
        {
            lock (_lock)
            {
                var t = name switch
                {
                    "Product" => Product,
                    "Category" => Category,
                    "Cart" => Cart,
                    "Transaction" => Transaction,
                    "Orders" => Orders,
                    "History" => History,
                    _ => null
                };
                return t?.Copy();
            }
        }
    }

}
