using AussieCake.Question;
using AussieCake.Sentence;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static AussieCake.Util.WPF.MyChBxs;

namespace AussieCake.Util.WPF
{
    public static class MyStacks
    {
        public static StackPanel GetItemLine(IQuest quest, StackPanel parent, bool isGridUpdate/*, int quantItems*/)
        {
            var stack = new StackPanel();
            stack.Margin = new Thickness(2, 4, 2, 2);
            stack.Background = UtilWPF.GetColourLine(false, isGridUpdate);
            stack.MouseEnter += new MouseEventHandler((source, e) => stack.Background = UtilWPF.GetColourLine(true, isGridUpdate));
            stack.MouseLeave += new MouseEventHandler((source, e) => stack.Background = UtilWPF.GetColourLine(false, isGridUpdate));

            if (isGridUpdate)
                parent.Children.Insert(0, stack);
            else
                parent.Children.Add(stack);

            return stack;
        }

        public static StackPanel GetHeaderInsertFilter(int row, int column, Grid parent)
        {
            var stk = new StackPanel();
            stk.Background = UtilWPF.Colour_header;
            UtilWPF.SetGridPosition(stk, row, column, parent);

            return stk;
        }

        public static StackPanel GetListItems(int row, int column, Grid parent)
        {
            var stk = new StackPanel();
            stk.Background = UtilWPF.Colour_row_off;
            var viewer = new ScrollViewer();
            viewer.Content = stk;
            UtilWPF.SetGridPosition(viewer, row, column, parent);

            return stk;
        }

        public static StackPanel Sentences(StackPanel reference, IQuest quest, StackPanel parent)
        {
            var stack = Get(reference, parent);
            stack.Visibility = Visibility.Collapsed;

            LoadSentences(reference, quest, stack);

            return stack;
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

        public static void LoadSentences(StackPanel reference, IQuest quest, StackPanel stackSentences)
        {
            foreach (var qs in quest.Sentences)
            {
                var stk_sen_line = Get(stackSentences);
                stk_sen_line.Orientation = Orientation.Horizontal;

                var btn_active = MyBtns.GetIsActive(stk_sen_line, qs.IsActive);
                btn_active.Margin = new Thickness(0, 0, 2, 0);
                btn_active.VerticalAlignment = VerticalAlignment.Center;

                var check = new CheckBoxSen
                {
                    Content = qs.Sen.Text,
                    QS = qs,
                    IsChecked = true,
                };
                check.MouseRightButtonDown += (source, e) => 
                {
                    Clipboard.SetText(qs.Sen.Text);
                    check.Foreground = Brushes.DarkGreen;
                };
                check.ToolTip = "Right click to copy the sentence";
                check.MouseEnter += new MouseEventHandler((source, e) => check.Foreground = Brushes.DarkRed);
                check.MouseLeave += new MouseEventHandler((source, e) => check.Foreground = Brushes.Black);
                check.VerticalContentAlignment = VerticalAlignment.Center;
                stk_sen_line.Children.Add(check);
            }
        }
    }
}
