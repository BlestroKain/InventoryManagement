using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace RapiMesa.Utility
{
    public static class DataGridViewFader
    {
        private class FadeInfo
        {
            public Timer Timer { get; }
            public Color Start { get; }
            public Color End { get; }
            public int Steps { get; }
            public int Current { get; set; }
            public DataGridViewRow Row { get; }
            public FadeInfo(DataGridViewRow row, Color start, Color end)
            {
                Row = row;
                Start = start;
                End = end;
                Steps = 10;
                Timer = new Timer { Interval = 15 };
            }
        }

        private static readonly ConditionalWeakTable<DataGridView, object> _attached =
            new ConditionalWeakTable<DataGridView, object>();

        public static void Attach(DataGridView grid)
        {
            if (_attached.TryGetValue(grid, out _)) return;
            _attached.Add(grid, null);
            grid.SelectionChanged += Grid_SelectionChanged;
        }

        private static void Grid_SelectionChanged(object sender, EventArgs e)
        {
            var grid = (DataGridView)sender;
            foreach (DataGridViewRow row in grid.Rows)
            {
                Color baseColor = (row.Index % 2 == 1 && grid.AlternatingRowsDefaultCellStyle.BackColor != Color.Empty)
                    ? grid.AlternatingRowsDefaultCellStyle.BackColor
                    : grid.DefaultCellStyle.BackColor;
                row.DefaultCellStyle.BackColor = baseColor;
            }

            foreach (DataGridViewRow row in grid.SelectedRows)
            {
                Color start = row.DefaultCellStyle.BackColor;
                Color end = ThemeManager.IsDarkMode ? Color.FromArgb(63, 63, 70) : Color.LightGoldenrodYellow;
                StartFade(new FadeInfo(row, start, end));
            }
        }

        private static void StartFade(FadeInfo info)
        {
            info.Timer.Tick += (s, e) =>
            {
                info.Current++;
                float t = info.Current / (float)info.Steps;
                info.Row.DefaultCellStyle.BackColor = Lerp(info.Start, info.End, t);
                if (info.Current >= info.Steps)
                {
                    info.Timer.Stop();
                    info.Timer.Dispose();
                }
            };
            info.Timer.Start();
        }

        private static Color Lerp(Color a, Color b, float t)
        {
            int r = (int)(a.R + (b.R - a.R) * t);
            int g = (int)(a.G + (b.G - a.G) * t);
            int bVal = (int)(a.B + (b.B - a.B) * t);
            return Color.FromArgb(r, g, bVal);
        }
    }
}
