using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace RapiMesa.Utility
{
    public static class ThemeManager
    {
        // Paleta
        private static readonly Color DarkBack = Color.FromArgb(45, 45, 48);
        private static readonly Color DarkPanel = Color.FromArgb(37, 37, 38);
        private static readonly Color DarkFore = Color.White;
        private static readonly Color DarkGrid = Color.FromArgb(70, 70, 73);
        private static readonly Color DarkSelBack = Color.FromArgb(63, 63, 70);
        private static readonly Color DarkSelFore = Color.White;
        private static readonly Color DarkAltRow = Color.FromArgb(50, 50, 53);

        public static bool IsDarkMode { get; private set; }

        // Guardamos colores originales sin usar Tag (evita colisiones)
        private class ControlColors
        {
            public ControlColors(Color back, Color fore) { Back = back; Fore = fore; }
            public Color Back { get; }
            public Color Fore { get; }
        }
        private static readonly ConditionalWeakTable<Control, ControlColors> _orig =
          new ConditionalWeakTable<Control, ControlColors>();


        public static void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
            foreach (Form f in Application.OpenForms)
                ApplyTheme(f);
        }

        public static void ApplyTheme(Form form)
        {
            if (form == null) return;
            form.SuspendLayout();
            try
            {
                ApplyTheme((Control)form);
                form.Refresh();
            }
            finally { form.ResumeLayout(true); }
        }

        public static void ApplyTheme(Control control)
        {
            if (control == null) return;

            // Opt-out fácil
            if (control.Tag is string tag && tag.Equals("NoTheme", StringComparison.OrdinalIgnoreCase))
                return;

            // Guarda originales una sola vez
            if (!_orig.TryGetValue(control, out _))
                _orig.Add(control, new ControlColors(control.BackColor, control.ForeColor));

            // Tipos especiales primero
            if (control is DataGridView dgv)
            {
                ApplyGridTheme(dgv);
            }
            else if (control is Button btn)
            {
                if (IsDarkMode)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = DarkGrid;
                    btn.BackColor = DarkPanel;
                    btn.ForeColor = DarkFore;
                }
                else RestoreColors(btn);
            }
            else if (control is TextBoxBase tb)
            {
                if (IsDarkMode)
                {
                    tb.BackColor = DarkPanel;
                    tb.ForeColor = DarkFore;
                    if (tb is RichTextBox rtb) { /* nada extra */ }
                }
                else RestoreColors(tb);
            }
            else if (control is ComboBox cb)
            {
                if (IsDarkMode)
                {
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.BackColor = DarkPanel;
                    cb.ForeColor = DarkFore;
                }
                else RestoreColors(cb);
            }
            else if (control is ListBox lb)
            {
                if (IsDarkMode)
                {
                    lb.BackColor = DarkPanel;
                    lb.ForeColor = DarkFore;
                }
                else RestoreColors(lb);
            }
            else if (control is TabControl tc)
            {
                if (IsDarkMode)
                {
                    tc.BackColor = DarkBack;
                    tc.ForeColor = DarkFore;
                    foreach (TabPage p in tc.TabPages)
                    {
                        if (!_orig.TryGetValue(p, out _))
                            _orig.Add(p, new ControlColors(p.BackColor, p.ForeColor));
                        p.BackColor = DarkBack;
                        p.ForeColor = DarkFore;
                    }
                }
                else
                {
                    RestoreColors(tc);
                    foreach (TabPage p in tc.TabPages) RestoreColors(p);
                }
            }
            else
            {
                // Default
                if (IsDarkMode)
                {
                    control.BackColor = (control is Panel || control is GroupBox) ? DarkPanel : DarkBack;
                    control.ForeColor = DarkFore;
                }
                else RestoreColors(control);
            }

            // Recursión
            foreach (Control child in control.Controls)
                ApplyTheme(child);
        }

        private static void ApplyGridTheme(DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("News706 BT", 12F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("News706 BT", 12F, FontStyle.Bold);

            if (IsDarkMode)
            {
                grid.EnableHeadersVisualStyles = false;

                grid.BackgroundColor = DarkBack;
                grid.GridColor = DarkGrid;

                // Header
                var hdr = grid.ColumnHeadersDefaultCellStyle;
                hdr.BackColor = DarkBack;
                hdr.ForeColor = DarkFore;
                hdr.SelectionBackColor = DarkBack;
                hdr.SelectionForeColor = DarkFore;

                // Cells
                var cell = grid.DefaultCellStyle;
                cell.BackColor = DarkBack;
                cell.ForeColor = DarkFore;
                cell.SelectionBackColor = DarkSelBack;
                cell.SelectionForeColor = DarkSelFore;

                // Rows
                var rows = grid.RowsDefaultCellStyle;
                rows.BackColor = DarkBack;
                rows.ForeColor = DarkFore;
                rows.SelectionBackColor = DarkSelBack;
                rows.SelectionForeColor = DarkSelFore;

                // Alternas
                grid.AlternatingRowsDefaultCellStyle.BackColor = DarkAltRow;
                grid.AlternatingRowsDefaultCellStyle.ForeColor = DarkFore;
                grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = DarkSelBack;
                grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = DarkSelFore;
            }
            else
            {
                grid.EnableHeadersVisualStyles = true;
                // Restaurar lo que el diseñador/tema del SO traiga
                RestoreColors(grid);
                grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle(); // reset
            }
        }

        private static void RestoreColors(Control c)
        {
            if (_orig.TryGetValue(c, out var o))
            {
                c.BackColor = o.Back;
                c.ForeColor = o.Fore;
            }
        }
    }
}
