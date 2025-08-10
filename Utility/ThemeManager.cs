using System.Drawing;
using System.Windows.Forms;

namespace RapiMesa.Utility
{
    public static class ThemeManager
    {
        private class ControlColors
        {
            public ControlColors(Color back, Color fore)
            {
                Back = back;
                Fore = fore;
            }

            public Color Back { get; }
            public Color Fore { get; }
        }

        private class DataGridViewColors : ControlColors
        {
            public DataGridViewColors(
                Color back,
                Color fore,
                Color grid,
                Color headerBack,
                Color headerFore,
                Color rowBack,
                Color rowFore)
                : base(back, fore)
            {
                Grid = grid;
                HeaderBack = headerBack;
                HeaderFore = headerFore;
                RowBack = rowBack;
                RowFore = rowFore;
            }

            public Color Grid { get; }
            public Color HeaderBack { get; }
            public Color HeaderFore { get; }
            public Color RowBack { get; }
            public Color RowFore { get; }
        }

        public static bool IsDarkMode { get; private set; }

        public static void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
            foreach (Form form in Application.OpenForms)
            {
                ApplyTheme(form);
                form.Refresh();
            }
        }

        public static void ApplyTheme(Control control)
        {
            if (control is DataGridView grid)
            {
                ApplyGridTheme(grid);
                return;
            }

            if (IsDarkMode)
            {
                if (!(control.Tag is ControlColors))
                {
                    control.Tag = new ControlColors(control.BackColor, control.ForeColor);
                }
                control.BackColor = Color.FromArgb(45, 45, 48);
                control.ForeColor = Color.White;
            }
            else if (control.Tag is ControlColors colors)
            {
                control.BackColor = colors.Back;
                control.ForeColor = colors.Fore;
            }

            foreach (Control child in control.Controls)
            {
                ApplyTheme(child);
            }
        }

        private static void ApplyGridTheme(DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("News706 BT", 12F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("News706 BT", 12F, FontStyle.Bold);

            if (IsDarkMode)
            {
                if (!(grid.Tag is DataGridViewColors))
                {
                    grid.Tag = new DataGridViewColors(
                        grid.BackgroundColor,
                        grid.ForeColor,
                        grid.GridColor,
                        grid.ColumnHeadersDefaultCellStyle.BackColor,
                        grid.ColumnHeadersDefaultCellStyle.ForeColor,
                        grid.RowsDefaultCellStyle.BackColor,
                        grid.RowsDefaultCellStyle.ForeColor);
                }

                var darkBack = Color.FromArgb(45, 45, 48);
                grid.BackgroundColor = darkBack;
                grid.ForeColor = Color.White;
                grid.GridColor = Color.FromArgb(70, 70, 73);
                grid.EnableHeadersVisualStyles = false;
                grid.ColumnHeadersDefaultCellStyle.BackColor = darkBack;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                grid.RowsDefaultCellStyle.BackColor = darkBack;
                grid.RowsDefaultCellStyle.ForeColor = Color.White;
                grid.DefaultCellStyle.BackColor = darkBack;
                grid.DefaultCellStyle.ForeColor = Color.White;
            }
            else if (grid.Tag is DataGridViewColors colors)
            {
                grid.BackgroundColor = colors.Back;
                grid.ForeColor = colors.Fore;
                grid.GridColor = colors.Grid;
                grid.EnableHeadersVisualStyles = true;
                grid.ColumnHeadersDefaultCellStyle.BackColor = colors.HeaderBack;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = colors.HeaderFore;
                grid.RowsDefaultCellStyle.BackColor = colors.RowBack;
                grid.RowsDefaultCellStyle.ForeColor = colors.RowFore;
                grid.DefaultCellStyle.BackColor = colors.RowBack;
                grid.DefaultCellStyle.ForeColor = colors.RowFore;
            }

            foreach (Control child in grid.Controls)
            {
                ApplyTheme(child);
            }
        }
    }
}