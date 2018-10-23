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
        public static StackPanel GetItemLine(IQuestion quest, StackPanel parent, bool isGridUpdate/*, int quantItems*/)
        {
            var stack = new StackPanel();
            stack.Margin = new Thickness(2, 4, 2, 2);
            stack.Background = isGridUpdate ? UtilWPF.GetBrushFromHTMLColor("#cce4ff") : Brushes.LightSteelBlue;
            stack.MouseEnter += new MouseEventHandler((source, e) => stack.Background = UtilWPF.GetBrushFromHTMLColor("#81a1ca"));
            stack.MouseLeave += new MouseEventHandler((source, e) => stack.Background = Brushes.LightSteelBlue);

            if (isGridUpdate)
                parent.Children.Insert(0, stack);
            else
                parent.Children.Add(stack);

            return stack;
        }

        public static StackPanel GetHeaderInsertFilter(int row, int column, Grid parent)
        {
            var stk = new StackPanel();
            stk.Background = UtilWPF.GetBrushFromHTMLColor("#6f93c3");
            UtilWPF.SetGridPosition(stk, row, column, parent);

            return stk;
        }

        public static StackPanel GetListItems(int row, int column, Grid parent)
        {
            var stk = new StackPanel();
            stk.Background = UtilWPF.GetBrushFromHTMLColor("#ebf0fa");
            var viewer = new ScrollViewer();
            viewer.Content = stk;
            UtilWPF.SetGridPosition(viewer, row, column, parent);

            return stk;
        }

        public static StackPanel GetSentences(StackPanel reference, IQuestion quest, StackPanel parent)
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

        public static StackPanel Get(StackPanel reference, int row, int column, Grid parent)
        {
            var stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            UtilWPF.SetGridPosition(stack, row, column, parent);

            return stack;
        }

        public static void LoadSentences(StackPanel reference, IQuestion quest, StackPanel stackSentences)
        {
            foreach (var senId in quest.SentencesId)
            {
                var check = new CheckBoxSen
                {
                    Content = SenControl.Get().First(s => s.Id == senId).Text,
                    SenId = senId,
                    IsChecked = true,
                };
                stackSentences.Children.Add(check);
            }
        }
    }
}
