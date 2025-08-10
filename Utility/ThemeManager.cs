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
    }
}
