﻿using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Util.WPF
{
    public static class MyChBxs
    {
        public static CheckBox GetIsVerb(CheckBox reference, int row, int column, Grid parent, bool isCompVerb)
        {
            return Get(reference, row, column, parent, "isVerb", isCompVerb);
        }

        public static CheckBox Get(CheckBox reference, int row, int column, Grid parent, string content, bool isChecked)
        {
            var cb = Get(reference, content, isChecked);
            UtilWPF.SetGridPosition(cb, row, column, parent);

            return cb;
        }

        public static CheckBox Get(CheckBox reference, string content, bool isChecked, StackPanel parent)
        {
            var cb = Get(reference, content, isChecked);
            parent.Children.Add(cb);

            return cb;
        }

        private static CheckBox Get(CheckBox reference, string content, bool isChecked)
        {
            reference.Content = content;
            reference.IsChecked = isChecked;
            reference.VerticalAlignment = VerticalAlignment.Center;
            reference.HorizontalAlignment = HorizontalAlignment.Center;
            reference.HorizontalContentAlignment = HorizontalAlignment.Center;
            reference.VerticalContentAlignment = VerticalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);

            return reference;
        }

        public class CheckBoxSen : CheckBox
        {
            public int SenId { get; set; }
        }
    }
}