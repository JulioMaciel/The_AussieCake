using AussieCake.Question;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Util.WPF
{
    public static class MyTxts
    {
        public static TextBox PtBr(TextBox reference, IQuest quest, StackPanel parent)
        {
            var txt = Get(reference, quest.PtBr, parent);
            txt.Visibility = Visibility.Collapsed;
            txt.Background = UtilWPF.GetBrushFromHTMLColor("#edfaeb");
            txt.ToolTip = "PtBr";

            return txt;
        }

        public static TextBox Definition(TextBox reference, IQuest quest, StackPanel parent)
        {
            var txt = Get(reference, quest.Definition, parent);
            txt.Visibility = Visibility.Collapsed;
            txt.ToolTip = "Definition";

            return txt;
        }

        public static TextBox Add_sentence(TextBox reference, StackPanel stk_sen)
        {
            var txt = Get(reference, string.Empty, stk_sen);
            txt.ToolTip = "Add Sentence";

            return txt;
        }

        public static TextBox Get(TextBox reference, int row, int column, Grid parent)
        {
            return Get(reference, row, column, parent, string.Empty);
        }

        public static TextBox Get(TextBox reference, StackPanel parent)
        {
            return Get(reference, string.Empty, parent);
        }

        public static TextBox Get(TextBox reference, int row, int column, Grid parent, string content)
        {
            var txt = Get(reference, content);
            txt.MouseDoubleClick += (source, e) => txt.Text = Clipboard.GetText();
            UtilWPF.SetGridPosition(txt, row, column, parent);

            return txt;
        }

        public static TextBox Get(TextBox reference, string content, StackPanel parent)
        {
            var txt = Get(reference, content);
            parent.Children.Add(txt);

            return txt;
        }

        private static TextBox Get(TextBox reference, string content)
        {
            reference.Text = content;
            reference.VerticalContentAlignment = VerticalAlignment.Center;
            reference.Height = 28;
            reference.Margin = new Thickness(1, 0, 1, 0);

            return reference;
        }

        static public bool IsEmpty(this TextBox txt)
        {
            return txt.Text.IsEmpty();
        }
    }

}
