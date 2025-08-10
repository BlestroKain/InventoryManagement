using System.Drawing;

namespace RapiMesa.Utility
{
    public static class PlaceholderIcons
    {
        private static Bitmap Draw(System.Action<Graphics, Pen> drawAction, Color color)
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen pen = new Pen(color, 2))
            {
                g.Clear(Color.Transparent);
                drawAction(g, pen);
            }
            return bmp;
        }

        public static Bitmap Add => Draw((g, p) =>
        {
            g.DrawLine(p, 8, 2, 8, 14);
            g.DrawLine(p, 2, 8, 14, 8);
        }, Color.DodgerBlue);

        public static Bitmap Edit => Draw((g, p) =>
        {
            g.DrawRectangle(p, 3, 3, 10, 10);
            g.DrawLine(p, 3, 3, 13, 13);
        }, Color.SeaGreen);

        public static Bitmap Delete => Draw((g, p) =>
        {
            g.DrawLine(p, 2, 2, 14, 14);
            g.DrawLine(p, 14, 2, 2, 14);
        }, Color.IndianRed);
    }
}
