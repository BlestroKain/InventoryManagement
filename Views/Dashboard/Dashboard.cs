using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using RapiMesa.Data; // <-- usa tu namespace real del SheetsRepo

namespace RapiMesa.Views.Dashboard
{
    public partial class Dashboard : Form
    {
        // Caché básica
        private DateTime _cacheUntil = DateTime.MinValue;
        private DataTable _tDt, _pDt, _cDt, _oDt; // Transaction, Product, Category, Orders

        // Mapa TransactionId -> Date (para no depender de Orders.Date)
        private Dictionary<string, DateTime> _transDateMap;

        private enum TimeRange { Last7, Last30, ThisMonth }
        private TimeRange _range = TimeRange.Last7;

        public Dashboard()
        {
            InitializeComponent();

            // Rango
            btn7d.Click += (s, e) => { _range = TimeRange.Last7; RefreshSalesChart(); };
            btn30d.Click += (s, e) => { _range = TimeRange.Last30; RefreshSalesChart(); };
            btnMes.Click += (s, e) => { _range = TimeRange.ThisMonth; RefreshSalesChart(); };

            // Refresh
            btnRefresh.Click += async (s, e) => await LoadStatsAsync(force: true);

            // Low stock (click)
            grpLowStock.Cursor = Cursors.Hand;
            grpLowStock.Click += (s, e) => ShowLowStockDialog();

            // Charts
            SetupCharts();

            // Cargar al mostrar
            this.Shown += async (s, e) => await LoadStatsAsync();
        }

        private void SetupCharts()
        {
            // Ventas
            chartSales7d.Series.Clear();
            chartSales7d.ChartAreas.Clear();
            chartSales7d.ChartAreas.Add(new ChartArea("ca1"));
            var s1 = new Series("Ventas")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double
            };
            chartSales7d.Series.Add(s1);
            chartSales7d.Legends.Clear();

            // Top 5
            chartTop5.Series.Clear();
            chartTop5.ChartAreas.Clear();
            chartTop5.ChartAreas.Add(new ChartArea("ca2"));
            var s2 = new Series("Top5")
            {
                ChartType = SeriesChartType.Bar,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double
            };
            chartTop5.Series.Add(s2);
            chartTop5.Legends.Clear();
        }

        private async Task LoadStatsAsync(bool force = false)
        {
            try
            {
                ToggleBusy(true);

                // caché 45s
                if (force || DateTime.Now >= _cacheUntil)
                {
                    _tDt = await SheetsRepo.ReadTableAsync("Transaction");
                    _pDt = await SheetsRepo.ReadTableAsync("Product");
                    _cDt = await SheetsRepo.ReadTableAsync("Category");
                    _oDt = await SheetsRepo.ReadTableAsync("Orders");

                    // construir mapa TransactionId -> Date
                    _transDateMap = _tDt.AsEnumerable()
                        .Where(r => r["TransactionId"] != null)
                        .ToDictionary(
                            r => r["TransactionId"].ToString(),
                            r => ParseDate(r["Date"])
                        );

                    _cacheUntil = DateTime.Now.AddSeconds(45);
                }

                // KPIs
                var today = DateTime.Today;
                decimal salesToday = SumTransactions(FilterByDate(_tDt, today, today));
                decimal salesMonth = SumTransactions(FilterByMonth(_tDt, today));
                int transTodayCount = FilterByDate(_tDt, today, today)?.Rows.Count ?? 0;
                decimal avgTicket = transTodayCount > 0 ? salesToday / transTodayCount : 0m;
                int lowStockCount = CountLowStock(_pDt, 5);
                decimal invValue = InventoryValue(_pDt);
                var (topName, topQty) = TopProductByQty(_oDt, today.AddDays(-30), today);

                lblSalesToday.Text = Money(salesToday);
                lblSalesMonth.Text = Money(salesMonth);
                lblAvgTicket.Text = Money(avgTicket);
                lblLowStock.Text = lowStockCount.ToString();
                lblInventoryValue.Text = Money(invValue);
                lblTopProduct.Text = topName == null ? "—" : $"{topName} ({topQty})";

                // Charts
                RefreshSalesChart();
                RefreshTop5Chart();

                lblLastSync.Text = $"Última sync: {DateTime.Now:dd/MM HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el panel:\r\n" + ex.Message,
                    "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleBusy(false);
            }
        }

        private void ToggleBusy(bool busy)
        {
            btnRefresh.Enabled = !busy;
            btn7d.Enabled = btn30d.Enabled = btnMes.Enabled = !busy;
            UseWaitCursor = busy;
        }

        // ---------- Charts ----------

        private void RefreshSalesChart()
        {
            var ca = chartSales7d.ChartAreas[0];
            chartSales7d.Series.Clear();
            var s = chartSales7d.Series.Add("Sales");
            s.ChartType = SeriesChartType.Column;
            s.ToolTip = "#VALX: #VAL{#,0}";
            chartSales7d.Legends.Clear();
            ca.AxisY.LabelStyle.Format = "#,0";
            ca.AxisX.Interval = 1;

            DateTime to = DateTime.Today, from;
            switch (_range)
            {
                case TimeRange.Last7: from = to.AddDays(-6); break;
                case TimeRange.Last30: from = to.AddDays(-29); break;
                case TimeRange.ThisMonth: from = new DateTime(to.Year, to.Month, 1); break;
                default: from = to.AddDays(-6); break;
            }

            // suma por día
            var perDay = new SortedDictionary<DateTime, decimal>();
            for (var d = from; d <= to; d = d.AddDays(1)) perDay[d] = 0m;

            var rangeDt = FilterByDate(_tDt, from, to);
            if (rangeDt != null)
            {
                foreach (DataRow r in rangeDt.Rows)
                {
                    var dt = ParseDate(r["Date"]);
                    var total = ParseDec(r["Total"]);
                    var day = dt.Date;
                    if (perDay.ContainsKey(day)) perDay[day] += total;
                }
            }

            foreach (var kv in perDay)
                s.Points.AddXY(kv.Key.ToString("dd/MM"), kv.Value);
        }

        private void RefreshTop5Chart()
        {
            chartTop5.Series.Clear();
            var s = chartTop5.Series.Add("Qty");
            s.ChartType = SeriesChartType.Bar;
            s.ToolTip = "#VALX: #VAL";
            chartTop5.Legends.Clear();

            var ca = chartTop5.ChartAreas[0];
            ca.AxisX.MajorGrid.Enabled = false;
            ca.AxisY.MajorGrid.Enabled = false;

            var from = DateTime.Today.AddDays(-30);
            var to = DateTime.Today;

            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (_oDt != null && _transDateMap != null)
            {
                foreach (DataRow r in _oDt.Rows)
                {
                    var tid = r["TransactionId"]?.ToString();
                    if (string.IsNullOrEmpty(tid)) continue;

                    if (!_transDateMap.TryGetValue(tid, out var dt)) continue;
                    if (dt < from || dt > to) continue;

                    var name = r["Name"]?.ToString() ?? "";
                    int qty = SafeInt(r["Quantity"]);

                    if (!dict.ContainsKey(name)) dict[name] = 0;
                    dict[name] += qty;
                }
            }

            foreach (var kv in dict.OrderByDescending(x => x.Value).Take(5).Reverse())
                s.Points.AddXY(kv.Key, kv.Value);
        }

        // ---------- Helpers de UI ----------

        private void ShowLowStockDialog()
        {
            var low = ToDataTableOrNull(
                _pDt.AsEnumerable()
                    .Where(r => SafeInt(r["Stock"]) <= 5)
                    .OrderBy(r => SafeInt(r["Stock"]))
            );

            if (low == null || low.Rows.Count == 0)
            {
                MessageBox.Show("No hay productos con stock bajo (≤5).");
                return;
            }

            using (var f = new Form())
            {
                f.Text = "Productos con stock bajo";
                f.StartPosition = FormStartPosition.CenterParent;
                f.Width = 700; f.Height = 400;

                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    DataSource = low
                };
                f.Controls.Add(grid);
                f.ShowDialog(this);
            }
        }

        // ---------- Helpers de datos ----------

        private static DataTable ToDataTableOrNull(IEnumerable<DataRow> rows)
        {
            if (rows == null) return null;
            var first = rows.FirstOrDefault();
            if (first == null) return null;

            var dt = first.Table.Clone();
            foreach (var r in rows) dt.ImportRow(r);
            return dt;
        }

        private static DataTable FilterByDate(DataTable dt, DateTime from, DateTime to)
        {
            if (dt == null) return null;
            var q = dt.AsEnumerable()
                      .Where(r =>
                      {
                          var d = ParseDate(r["Date"]);
                          return d.Date >= from.Date && d.Date <= to.Date;
                      });
            return ToDataTableOrNull(q);
        }

        private static DataTable FilterByMonth(DataTable dt, DateTime anyDay)
        {
            var start = new DateTime(anyDay.Year, anyDay.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return FilterByDate(dt, start, end);
        }

        private static decimal SumTransactions(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return 0m;
            decimal sum = 0m;
            foreach (DataRow r in dt.Rows) sum += ParseDec(r["Total"]);
            return sum;
        }

        private static int CountLowStock(DataTable pDt, int threshold)
        {
            if (pDt == null) return 0;
            int c = 0;
            foreach (DataRow r in pDt.Rows)
                if (SafeInt(r["Stock"]) <= threshold) c++;
            return c;
        }

        private static decimal InventoryValue(DataTable pDt)
        {
            if (pDt == null) return 0m;
            decimal acc = 0m;
            foreach (DataRow r in pDt.Rows)
                acc += SafeDec(r["Price"]) * SafeInt(r["Stock"]);
            return acc;
        }

        private (string name, int qty) TopProductByQty(DataTable orders, DateTime from, DateTime to)
        {
            if (orders == null || _transDateMap == null) return (null, 0);

            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow r in orders.Rows)
            {
                var tid = r["TransactionId"]?.ToString();
                if (string.IsNullOrEmpty(tid)) continue;

                if (!_transDateMap.TryGetValue(tid, out var dt)) continue;
                if (dt < from || dt > to) continue;

                var name = r["Name"]?.ToString() ?? "";
                int q = SafeInt(r["Quantity"]);
                if (!dict.ContainsKey(name)) dict[name] = 0;
                dict[name] += q;
            }

            if (dict.Count == 0) return (null, 0);
            var top = dict.OrderByDescending(x => x.Value).First();
            return (top.Key, top.Value);
        }

        // ---------- Parsers util ----------

        private static decimal ParseDec(object v)
        {
            if (v == null) return 0m;
            var s = v.ToString().Trim()
                    .Replace("$", "")
                    .Replace("€", "")
                    .Replace("£", "")
                    .Replace(",", "");
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
        }

        private static int SafeInt(object v) =>
            int.TryParse(v?.ToString(), out var x) ? x : 0;

        private static decimal SafeDec(object v)
        {
            if (v == null) return 0;
            var s = v.ToString().Replace("$", "").Replace(",", "").Trim();
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
        }

        private static DateTime ParseDate(object v)
        {
            if (v == null) return DateTime.MinValue;
            var s = v.ToString().Trim();

            // ISO/humano
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var d))
                return d;

            // Serial de Sheets (OADate)
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var serial))
                return DateTime.FromOADate(serial);

            return DateTime.MinValue;
        }

        private static string Money(decimal v) =>
            v >= 1_000_000 ? (v / 1_000_000m).ToString("0.##'M'") :
            v >= 1_000 ? (v / 1_000m).ToString("0.##'K'") :
            v.ToString("#,0");
    }
}
