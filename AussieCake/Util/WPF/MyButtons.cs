using AussieCake.Question;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AussieCake.Util.WPF
{
    public static class MyBtns
    {
        public static ButtonActive GetIsActive(ButtonActive reference, int row, int column, Grid parent, bool isActive)
        {
            reference.Content = isActive ? UtilWPF.GetIconButton("switch_on") : UtilWPF.GetIconButton("switch_off");
            reference.VerticalAlignment = VerticalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);
            reference.Background = Brushes.Transparent;
            reference.BorderBrush = Brushes.Transparent;
            reference.Width = 32;
            reference.Height = 32;

            UtilWPF.SetGridPosition(reference, row, column, parent);

            reference.IsActived = isActive;

            reference.Click += (source, e) =>
            {
                reference.IsActived = !reference.IsActived;
                reference.Content = reference.IsActived ? UtilWPF.GetIconButton("switch_on") : UtilWPF.GetIconButton("switch_off"); ;
            };

            return reference;
        }

        public static Button GetPtBr(Button reference, int row, int column, Grid parent, string ptBr, TextBox txt_ptBr)
        {
            var content = ptBr.IsEmpty() ? UtilWPF.GetIconButton("br_gray") : UtilWPF.GetIconButton("br");

            var btn = Get(reference, row, column, parent, content);
            CreateBtnLineBehavior(ptBr, txt_ptBr, btn);

            return btn;
        }

        public static Button GetDefinition(Button reference, int row, int column, Grid parent, string def, TextBox txt_def)
        {
            var btn = Get(reference, row, column, parent, UtilWPF.GetIconButton("definition"));
            CreateBtnLineBehavior(def, txt_def, btn);

            return btn;
        }

        private static void CreateBtnLineBehavior(string content, TextBox txt, Button btn)
        {
            btn.Opacity = content.IsEmpty() ? 0.5 : 1;

            btn.Click += (source, e) =>
            {
                if (txt.Visibility == Visibility.Collapsed)
                    txt.Visibility = Visibility.Visible;
                else
                    txt.Visibility = Visibility.Collapsed;
            };
        }

        public static Button GetEdit(Button reference, int row, int column, Grid parent, IQuestion quest, QuestWpfItem wpf_item, StackPanel item_line)
        {
            var btn = Get(reference, row, column, parent, UtilWPF.GetIconButton("save_black"));
            btn.Click += async (source, e) =>
            {
                btn.Content = UtilWPF.GetIconButton("save");
                await System.Threading.Tasks.Task.Delay(2000);
                btn.Content = UtilWPF.GetIconButton("save_black");

                if (quest is ColVM)
                    QuestWpfUtil.EditColClick(quest as ColVM, wpf_item as ColWpfItem, item_line);
            };

            return btn;
        }

        public static Button GetRemove(Button reference, int row, int column, Grid parent, StackPanel item_line)
        {
            var btn = Get(reference, row, column, parent, UtilWPF.GetIconButton("remove_v2"));
            btn.Height = 28;
            btn.Width = 28;

            btn.Click += (source, e) => item_line.Children.Clear();

            return btn;
        }

        public static Button GetFilter(Button reference, int row, int column, Grid parent, QuestWpfHeader wpf_header, IFilter filter)
        {
            var btn = Get(reference, row, column, parent, "Filter");

            if (wpf_header is ColWpfHeader)
                btn.Click += (source, e) => filter.Filter(wpf_header as ColWpfHeader);

            return btn;
        }

        public static Button GetInsert(Button reference, int row, int column, Grid parent, StackPanel stk_items, QuestWpfHeader wpf_header)
        {
            var btn = Get(reference, row, column, parent, "Insert");

            btn.Click += (source, e) =>
            {
                if (wpf_header is ColWpfHeader)
                    QuestWpfUtil.InsertColClick(stk_items, wpf_header as ColWpfHeader);
            };

            return btn;
        }

        public static Button Get(Button reference, int row, int column, Grid parent, object content)
        {
            var btn = Get(reference, content);
            UtilWPF.SetGridPosition(btn, row, column, parent);

            return btn;
        }

        public static Button Get(Button reference, object content, StackPanel parent)
        {
            var btn = Get(reference, content);
            parent.Children.Add(btn);

            return btn;
        }

        private static Button Get(Button reference, object content)
        {
            reference.Content = content;
            reference.VerticalAlignment = VerticalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);

            if (content.GetType() != typeof(String))
            {
                reference.Background = Brushes.Transparent;
                reference.BorderBrush = Brushes.Transparent;
                reference.Width = 32;
                reference.Height = 32;
            }
            else
                reference.Height = 28;

            return reference;
        }

        public static Button GetRemoveQuestion(Button reference, int row, int column, Grid parent, IQuestion quest, StackPanel main_line)
        {
            var btn_remove = GetRemove(reference, row, column, parent, main_line);
            btn_remove.Click += (source, e) =>
            {
                var removed = QuestControl.Get(Model.Col).First(s => s.Id == quest.Id);
                QuestControl.Remove(removed);
            };

            return btn_remove;
        }

        public static Button GetSentences(Button reference, int row, int column, Grid parent, IQuestion quest, StackPanel stk_sen)
        {
            var btn = Get(reference, row, column, parent, quest.SentencesId.Count + " Sens");
            btn.Foreground = quest.SentencesId.Count == 0 ? Brushes.DarkRed : Brushes.Black;
            btn.Background = Brushes.Transparent;
            btn.BorderBrush = Brushes.Transparent;

            btn.Click += (source, e) =>
            {
                if (stk_sen.Visibility == Visibility.Collapsed)
                    stk_sen.Visibility = Visibility.Visible;
                else
                    stk_sen.Visibility = Visibility.Collapsed;
            };

            return btn;
        }
    }

    public class ButtonActive : Button
    {
        public bool IsActived { get; set; }
    }
}
