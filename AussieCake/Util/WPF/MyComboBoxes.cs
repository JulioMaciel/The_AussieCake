using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Util.WPF
{
    public static class MyCbBxs
    {
        public static ComboBox GetImportance(ComboBox reference, int row, int column, Grid parent, Importance imp, bool isFilter)
        {
            var cb = Get(reference, row, column, parent);
            var source = Enum.GetValues(typeof(Importance)).Cast<Importance>();

            if (!isFilter) // remove Any when edit line
                source = source.Where(i => i != Importance.Any);

            cb.ItemsSource = source;
            cb.SelectedValue = imp;

            return cb;
        }

        public static ComboBox Get(ComboBox reference, int row, int column, Grid parent)
        {
            var cb = Get(reference);
            UtilWPF.SetGridPosition(cb, row, column, parent);

            return cb;
        }

        public static ComboBox Get(ComboBox reference, StackPanel parent)
        {
            var cb = Get(reference);
            parent.Children.Add(cb);

            return cb;
        }

        private static ComboBox Get(ComboBox reference)
        {
            reference.VerticalContentAlignment = VerticalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);

            return reference;
        }
    }
}
