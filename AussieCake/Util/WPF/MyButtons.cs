using AussieCake.Attempt;
using AussieCake.Challenge;
using AussieCake.Question;
using AussieCake.Sentence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AussieCake.Util.WPF
{
    public static class MyBtns
    {
        public static ButtonActive Is_active(ButtonActive reference, int row, int column, Grid parent, bool isActive)
        {
            UtilWPF.SetGridPosition(reference, row, column, parent);

            CreateBtnActive(reference, isActive);

            return reference;
        }

        public static ButtonActive GetIsActive(StackPanel parent, bool isActive)
        {
            var btn = new ButtonActive();
            parent.Children.Add(btn);

            CreateBtnActive(btn, isActive);

            return btn;
        }

        private static void CreateBtnActive(ButtonActive reference, bool isActive)
        {
            reference.Content = isActive ? UtilWPF.GetIconButton("switch_on") : UtilWPF.GetIconButton("switch_off");
            reference.VerticalAlignment = VerticalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);
            reference.Background = Brushes.Transparent;
            reference.BorderBrush = Brushes.Transparent;
            reference.Width = 32;
            reference.Height = 32;

            reference.IsActived = isActive;

            reference.Click += (source, e) =>
            {
                reference.IsActived = !reference.IsActived;
                reference.Content = reference.IsActived ? UtilWPF.GetIconButton("switch_on") : UtilWPF.GetIconButton("switch_off"); ;
            };
        }

        public static Button PtBr(Button reference, int row, int column, Grid parent, string ptBr, TextBox txt_ptBr)
        {
            var content = ptBr.IsEmpty() ? UtilWPF.GetIconButton("br_gray") : UtilWPF.GetIconButton("br");

            var btn = Get(reference, row, column, parent, content);
            CreateBtnLineBehavior(ptBr, txt_ptBr, btn);

            return btn;
        }

        public static Button Definition(Button reference, int row, int column, Grid parent, string def, TextBox txt_def)
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

        public static Button Edit_quest(Button reference, int row, int column, Grid parent, IQuest quest, QuestWpfItem wpf_item, StackPanel item_line)
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

        public static Button Remove_quest(Button reference, int row, int column, Grid parent, IQuest quest, StackPanel main_line)
        {
            var btn_remove = GetRemove(reference, row, column, parent, main_line);
            btn_remove.Click += (source, e) =>
            {
                var removed = QuestControl.Get(Model.Col).First(s => s.Id == quest.Id);
                QuestControl.Remove(removed);
            };

            return btn_remove;
        }

        public static Button Show_sentences(Button reference, int row, int column, Grid parent, IQuest quest, StackPanel stk_sen)
        {
            var btn = Get(reference, row, column, parent, quest.Sentences.Count + " Sens");
            btn.Foreground = quest.Sentences.Count == 0 ? Brushes.DarkRed : Brushes.Black;
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

        public static Button Chal_remove_att(ChalLine line)
        {
            var btn = Get(line.Chal.Remove_att, 0, 0, line.Chal.Row_4, "Remove attempt");
            line.Chal.Remove_att.Width = 125;
            line.Chal.Remove_att.Click += (source, e) =>
            {
                AttemptsControl.RemoveLast(line.Quest.Type);
                line.Chal.Remove_att.IsEnabled = false;
                line.Chal.Disable_sen.IsEnabled = true;
                line.Chal.Disable_quest.IsEnabled = true;
                line.Chal.Grid_chal.Background = UtilWPF.Colour_row_off;

                line.Quest.LoadCrossData();

                var updated = QuestControl.Get(line.Quest.Type).First(x => x.Id == line.Quest.Id);

                line.Chal.Avg_w.Content = updated.Avg_week + "% (w)";
                line.Chal.Avg_m.Content = updated.Avg_month + "% (m)";
                line.Chal.Avg_all.Content = updated.Avg_all + "% (all)";
                line.Chal.Tries.Content = updated.Tries.Count + " tries";
                line.Chal.Chance.Content = updated.Chance + " (" + Math.Round(updated.Chance_real, 2) + ")";
            };

            return btn;
        }

        public static Button Chal_disable_sen(ChalLine line)
        {
            var btn = Get(line.Chal.Disable_sen, 0, 2, line.Chal.Row_4, "Disable sentence");
            line.Chal.Disable_sen.Width = 125;
            line.Chal.Disable_sen.IsEnabled = false;
            line.Chal.Disable_sen.Click += (source, e) =>
            {
                QuestSenControl.Remove(line.QS.QS_id, line.Quest.Type);
                line.Chal.Disable_sen.IsEnabled = false;
            };

            return btn;
        }

        public static Button Chal_disable_quest(ChalLine line)
        {
            var btn = Get(line.Chal.Disable_quest, 0, 4, line.Chal.Row_4, "Disable quest");
            line.Chal.Disable_quest.Width = 125;
            line.Chal.Disable_quest.IsEnabled = false;
            line.Chal.Disable_quest.Click += (source, e) =>
            {
                line.Quest.Disable();
                QuestControl.Update(line.Quest);
                line.Chal.Disable_quest.IsEnabled = false;
            };

            return btn;
        }

        public static Button Chal_next(Button reference, StackPanel parent, Button btn_verify, Grid userControlGrid, List<ChalLine> lines, Model type)
        {
            parent.Children.Add(reference);
            reference.Content = "Next";
            reference.IsEnabled = false;
            reference = Set_btn_challenge(reference);
            reference.Click += (source, e) =>
            {
                ChalWPFControl.PopulateRows(userControlGrid, type, lines);
                btn_verify.IsEnabled = true;
                reference.IsEnabled = false;
            };

            return reference;
        }

        private static Button Set_btn_challenge(Button btn)
        {
            btn.Width = 100;
            btn.Height = 28;
            btn.Margin = new Thickness(0, 0, 4, 0);

            return btn;
        }

        public static Button Chal_verify(Button reference, StackPanel parent, Button btn_next)
        {
            parent.Children.Insert(0, reference);
            reference.Content = "Verify";
            reference = Set_btn_challenge(reference);
            reference.Click += (source, e) => btn_next.IsEnabled = true;

            return reference;
        }


    }

    public class ButtonActive : Button
    {
        public bool IsActived { get; set; }
    }
}
