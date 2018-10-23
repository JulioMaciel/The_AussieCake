using AussieCake.Question;
using AussieCake.Util;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AussieCake.Sentence
{
    public static class SentenceWPF
    {
        public static void AddSentenceRow(StackPanel main_stack, SenVM sen, bool isGridUpdate)
        {
            var lineName = "line_" + sen.Id;

            if (main_stack.Children.OfType<StackPanel>().Any(l => l.Name == lineName))
                return;

            var line = new StackPanel();
            line.Name = lineName;
            line.Background = isGridUpdate ? Brushes.SteelBlue : Brushes.LightSteelBlue;
            line.MouseEnter += new MouseEventHandler((source, e) => line.Background = Brushes.CornflowerBlue);
            line.MouseLeave += new MouseEventHandler((source, e) => line.Background = Brushes.LightSteelBlue);
            main_stack.Children.Insert(0, line);

            var gridSentence = new Grid();
            gridSentence.Margin = new Thickness(8, 8, 8, 2);
            var columnSen = new ColumnDefinition();
            //columnSen.Width = new GridLength(100, GridUnitType.Star);
            var columnBtnActive = new ColumnDefinition();
            columnBtnActive.Width = new GridLength(40, GridUnitType.Pixel);
            var columnBtnPtBr = new ColumnDefinition();
            columnBtnPtBr.Width = new GridLength(40, GridUnitType.Pixel);
            var columnBtnQuestions = new ColumnDefinition();
            columnBtnQuestions.Width = new GridLength(40, GridUnitType.Pixel);
            var columnBtnUpdate = new ColumnDefinition();
            columnBtnUpdate.Width = new GridLength(40, GridUnitType.Pixel);
            var columnBtnRemove = new ColumnDefinition();
            columnBtnRemove.Width = new GridLength(40, GridUnitType.Pixel);
            var rowSen = new RowDefinition();
            var rowPtBr = new RowDefinition();
            gridSentence.ColumnDefinitions.Add(columnSen);
            gridSentence.ColumnDefinitions.Add(columnBtnActive);
            gridSentence.ColumnDefinitions.Add(columnBtnPtBr);
            gridSentence.ColumnDefinitions.Add(columnBtnQuestions);
            gridSentence.ColumnDefinitions.Add(columnBtnUpdate);
            gridSentence.ColumnDefinitions.Add(columnBtnRemove);
            gridSentence.RowDefinitions.Add(rowSen);
            gridSentence.RowDefinitions.Add(rowPtBr);
            line.Children.Add(gridSentence);

            TextBox txt_sen = new TextBox();
            TextBox txt_ptBr = new TextBox();
            Button btn_active = new Button();
            Button btn_edit = new Button();
            Button btn_questions = new Button();
            Button btn_remove = new Button();

            var editedQuestions = new List<IQuestion>();

            txt_sen = new TextBox
            {
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 4),
                Text = sen.Text,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            txt_sen.TextChanged += (source, e) => CheckIfItemWasEdited(sen, txt_sen, txt_ptBr, editedQuestions, btn_edit);
            Grid.SetRow(txt_sen, 0);
            Grid.SetColumn(txt_sen, 0);
            gridSentence.Children.Add(txt_sen);

            var stackBottomLine = new StackPanel();
            stackBottomLine.Margin = new Thickness(0, 0, 0, 8);
            Grid.SetRow(stackBottomLine, 1);
            Grid.SetColumn(stackBottomLine, 0);
            Grid.SetColumnSpan(stackBottomLine, 6);
            gridSentence.Children.Add(stackBottomLine);

            txt_ptBr = new TextBox
            {
                Visibility = Visibility.Collapsed,
                FontSize = 14,
                Text = sen.PtBr,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            txt_ptBr.TextChanged += (source, e) => CheckIfItemWasEdited(sen, txt_sen, txt_ptBr, editedQuestions, btn_edit);
            txt_ptBr.Background = UtilWPF.GetBrushFromHTMLColor("#edfaeb");
            stackBottomLine.Children.Add(txt_ptBr);

            var stackCheckQuestions = new StackPanel
            {
                Visibility = Visibility.Collapsed,
                Orientation = Orientation.Horizontal
            };
            foreach (var quest in sen.Questions)
            {
                var check = new CheckBox();
                check.IsChecked = true;
                if (quest is ColVM)
                {
                    var col = quest as ColVM;
                    check.Content += col.Prefixes.Any() ? String.Join(", ", col.Prefixes.ToArray()) + "; " : string.Empty;
                    check.Content += col.Component1.Any() ? col.Component1 + "; " : string.Empty;
                    check.Content += col.LinkWords.Any() ? String.Join(", ", col.LinkWords.ToArray()) + "; " : string.Empty;
                    check.Content += col.Component2.Any() ? col.Component2 + "; " : string.Empty;
                    check.Content += col.Suffixes.Any() ? String.Join(", ", col.Suffixes.ToArray()) + "; " : string.Empty;
                    check.Content += "(Collocation)";
                }
                check.Checked += (source, e) =>
                {
                    quest.SentencesId.Add(sen.Id);

                    if (!editedQuestions.Contains(quest))
                    editedQuestions.Add(quest);

                    CheckIfItemWasEdited(sen, txt_sen, txt_ptBr, editedQuestions, btn_edit);
                };
                check.Unchecked += (source, e) =>
                {
                    quest.SentencesId.Remove(sen.Id);

                    if (!editedQuestions.Contains(quest))
                        editedQuestions.Add(quest);

                    CheckIfItemWasEdited(sen, txt_sen, txt_ptBr, editedQuestions, btn_edit);
                };
                stackCheckQuestions.Children.Add(check);
            }
            stackBottomLine.Children.Add(stackCheckQuestions);

            btn_active = new Button
            {
                Content = sen.IsActive ? UtilWPF.GetIconButton("switch_on") : UtilWPF.GetIconButton("switch_off"),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 32,
                Width = 32
            };
            btn_active.Click += (source, e) =>
            {
                sen.IsActive = !sen.IsActive;
                btn_active.Content = sen.IsActive ? UtilWPF.GetIconButton("switch_on") : UtilWPF.GetIconButton("switch_off");
            };
            Grid.SetRow(btn_active, 0);
            Grid.SetColumn(btn_active, 1);
            gridSentence.Children.Add(btn_active);

            var btn_ptBr = new Button
            {
                Content = !sen.PtBr.IsEmpty() ? UtilWPF.GetIconButton("br_gray") : UtilWPF.GetIconButton("br"),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 32,
                Width = 32,
                Opacity = !sen.PtBr.IsEmpty() ? 0.5 : 1
            };
            btn_ptBr.Click += (source, e) =>
            {
                if (txt_ptBr.Visibility == Visibility.Collapsed)
                    txt_ptBr.Visibility = Visibility.Visible;
                else
                    txt_ptBr.Visibility = Visibility.Collapsed;
            };
            Grid.SetRow(btn_ptBr, 0);
            Grid.SetColumn(btn_ptBr, 2);
            gridSentence.Children.Add(btn_ptBr);

            btn_questions = new Button
            {
                Content = sen.Questions.Any() ? UtilWPF.GetIconButton("box_full") : UtilWPF.GetIconButton("box_empty"),
                IsEnabled = sen.Questions.Any(),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 32,
                Width = 32
            };
            btn_questions.Click += (source, e) =>
            {
                if (stackCheckQuestions.Visibility == Visibility.Collapsed)
                    stackCheckQuestions.Visibility = Visibility.Visible;
                else
                    stackCheckQuestions.Visibility = Visibility.Collapsed;
            };
            Grid.SetRow(btn_questions, 0);
            Grid.SetColumn(btn_questions, 3);
            gridSentence.Children.Add(btn_questions);

            btn_edit = new Button
            {
                Content = UtilWPF.GetIconButton("save_black"),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 32,
                Width = 32,
                IsEnabled = false
            };
            btn_edit.Click += (source, e) =>
            {
                var edited = new SenVM(sen.Id);
                edited.Text = txt_sen.Text;
                edited.PtBr = txt_ptBr.Text;
                SenControl.Update(edited);

                foreach (var quest in editedQuestions)
                    QuestControl.Update(quest);

                editedQuestions.Clear();

                btn_edit.IsEnabled = false;
            };
            Grid.SetRow(btn_edit, 0);
            Grid.SetColumn(btn_edit, 4);
            gridSentence.Children.Add(btn_edit);

            btn_remove = new Button
            {
                Content = UtilWPF.GetIconButton("remove_v2"),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 28,
                Width = 28
            };
            btn_remove.Click += (source, e) =>
            {
                var removed = SenControl.Get().First(s => s.Id == sen.Id);
                SenControl.Remove(removed);
                line.Visibility = Visibility.Collapsed;
            };
            Grid.SetRow(btn_remove, 0);
            Grid.SetColumn(btn_remove, 5);
            gridSentence.Children.Add(btn_remove);

        }

        private static void CheckIfItemWasEdited(SenVM sen, TextBox txt_sen, TextBox txt_ptBr, List<IQuestion> editedQuestions, Button btn_edit)
        {
            var wasTextEdited = txt_sen.Text != sen.Text;
            var wasPtBrEdited = !txt_ptBr.Text.IsEmpty() && txt_ptBr.Text != sen.PtBr;

            if (wasTextEdited || wasPtBrEdited || editedQuestions.Any())
            {

                btn_edit.IsEnabled = true;
                btn_edit.Content = UtilWPF.GetIconButton("save");
            }
            else
            {
                btn_edit.IsEnabled = false;
                btn_edit.Content = new FormatConvertedBitmap(btn_edit.Content as BitmapSource, PixelFormats.Gray32Float, null, 50);
            }
        }

    }
}
