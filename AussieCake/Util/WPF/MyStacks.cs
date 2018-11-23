using AussieCake.Question;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static AussieCake.Util.WPF.MyChBxs;

namespace AussieCake.Util.WPF
{
    public static class MyStacks
    {
        public static StackPanel GetItemLine(StackPanel parent, bool isGridUpdate)
        {
            var stack = new StackPanel();
            stack.Margin = new Thickness(0, 2, 2, 2);
            stack.Background = UtilWPF.GetColourLine(false, isGridUpdate);
            stack.MouseEnter += new MouseEventHandler((source, e) => stack.Background = UtilWPF.GetColourLine(true, isGridUpdate));
            stack.MouseLeave += new MouseEventHandler((source, e) => stack.Background = UtilWPF.GetColourLine(false, isGridUpdate));

            if (isGridUpdate)
                parent.Children.Insert(0, stack);
            else
                parent.Children.Add(stack);

            return stack;
        }

        public static StackPanel GetHeaderInsertFilter(StackPanel stk, int row, int column, Grid parent)
        {
            stk.Background = UtilWPF.Colour_header;
            UtilWPF.SetGridPosition(stk, row, column, parent);

            return stk;
        }

        public static StackPanel GetListItems(int row, int column, Grid parent)
        {
            var stk = new StackPanel();
            //stk.Background = UtilWPF.Colour_row_off;
            var viewer = new ScrollViewer();
            viewer.Content = stk;
            UtilWPF.SetGridPosition(viewer, row, column, parent);

            return stk;
        }

        private static StackPanel StackModel(Model type, StackPanel parent)
        {
            var stack_type = new StackPanel();
            stack_type.Orientation = Orientation.Horizontal;
            stack_type.Visibility = Visibility.Collapsed;
            parent.Children.Add(stack_type);
            var lbl_col = new Label();
            lbl_col.Content = type.ToDesc() + "s:";
            stack_type.Children.Add(lbl_col);

            return stack_type;
        }

        public static StackPanel Get(StackPanel reference, StackPanel parent)
        {
            parent.Children.Add(reference);

            return reference;
        }

        public static StackPanel Get(StackPanel parent)
        {
            return Get(new StackPanel(), parent);
        }

        public static StackPanel Get(StackPanel reference, int row, int column, Grid parent)
        {
            reference.Orientation = Orientation.Horizontal;
            UtilWPF.SetGridPosition(reference, row, column, parent);

            return reference;
        }
    }
}
