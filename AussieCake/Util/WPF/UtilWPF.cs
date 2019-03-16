using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AussieCake.Util.WPF
{
    public static class UtilWPF
    {
        public static Brush Colour_header { get; } = GetBrushFromHTMLColor("#6f93c3"); // azul forte
        public static Brush Colour_row_on { get; } = GetBrushFromHTMLColor("#cad7e8"); // azul claro
        public static Brush Colour_row_off { get; } = GetBrushFromHTMLColor("#a5bcd9"); // azul médio
        public static Brush Colour_new_row_off { get; } = GetBrushFromHTMLColor("#95CAE4"); // azul claro médio
        public static Brush Colour_Incorrect { get; } = GetBrushFromHTMLColor("#e6b3b3");
        public static Brush Colour_Correct { get; } = GetBrushFromHTMLColor("#2fb673");

        public static Brush GetColourLine(bool is_on, bool isGridUpdate)
        {
            if (is_on)
                return Colour_row_on;
            else
                return isGridUpdate ? Colour_new_row_off : Colour_row_off;
        }

        public static Image GetIconButton(string iconFile)
        {
            var btn_icon = new Image();
            btn_icon.Source = new BitmapImage(new Uri(CakePaths.GetIconPath(iconFile)));
            return btn_icon;
        }

        public static SolidColorBrush GetAvgColor(double percentage)
        {
            var red = Color.FromRgb(255, 0, 0);
            var yellow = Color.FromRgb(255, 255, 0);
            var lime = Color.FromRgb(0, 255, 0);

            var darkRed = Color.FromRgb(139, 0, 0);
            var goldenrod = Color.FromRgb(179, 143, 0);
            var seaGreen = Color.FromRgb(26, 101, 64);

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
            if (parent.Children.Contains(child))
                return;

            Grid.SetRow(child, row);
            Grid.SetColumn(child, column);
            parent.Children.Add(child);
        }

        static int seed = Environment.TickCount;
        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
        public static int RandomNumber(int min, int max) => random.Value.Next(min, max);        
    }
}
