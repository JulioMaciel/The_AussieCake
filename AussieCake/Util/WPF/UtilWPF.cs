using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AussieCake.Util.WPF
{
    public static class UtilWPF
    {
        public static Image GetIconButton(string iconFile)
        {
            var btn_icon = new Image();
            btn_icon.Source = new BitmapImage(new Uri(CakePaths.GetIconPath(iconFile)));
            return btn_icon;
        }

        public static SolidColorBrush GetAvgColor(double percentage)
        {
            if (percentage == null)
                return Brushes.Black;

            var red = Color.FromRgb(255, 0, 0);
            var yellow = Color.FromRgb(255, 255, 0);
            var lime = Color.FromRgb(0, 255, 0);

            // testing better gray colors
            var darkRed = Color.FromRgb(139, 0, 0);
            var goldenrod = Color.FromRgb(218, 165, 32);
            var seaGreen = Color.FromRgb(46, 139, 87);

            if (percentage < 50)
                return Interpolate(darkRed, goldenrod, percentage / 50.0);
            return Interpolate(goldenrod, seaGreen, (percentage - 50) / 50.0);
        }

        public static Brush GetBrushFromHTMLColor(string hexadecimal)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexadecimal));
        }

        private static SolidColorBrush Interpolate(Color color1, Color color2, double fraction)
        {
            var c1 = color1.ColorContext;

            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            var dColor = System.Drawing.Color.FromArgb((int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));
            return new SolidColorBrush(Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B));
        }

        private static double Interpolate(double d1, double d2, double fraction)
        {
            return d1 + (d2 - d1) * fraction;
        }

        public static void SetGridPosition(FrameworkElement child, int row, int column, Grid parent)
        {
            Grid.SetRow(child, row);
            Grid.SetColumn(child, column);
            parent.Children.Add(child);
        }

        //public static int MaxPag
        //{
        //    get { return 30; }
        //}
    }
}
