using AussieCake.Question;
using AussieCake.Sentence;
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
            //stk.Background = UtilWPF.Colour_row_off;
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

        public static StackPanel Sen_quests(StackPanel reference, SenVM sen, StackPanel parent)
        {
            var stk = Get(reference, parent);
            stk.Visibility = Visibility.Collapsed;

            var stk_col = StackModel(Model.Col, stk);

            foreach (var sq in sen.Questions)
            {
                var check = new CheckQuest(sq.Quest, true);
                if (sq.Quest.Type == Model.Col)
                {
                    stk_col.Children.Add(check);
                    stk_col.Visibility = Visibility.Visible;
                }
                // and so on
            }

            var stk_search = new StackPanel();
            stk.Children.Add(stk_search);

            var stk_buttons = new StackPanel();
            stk_buttons.Orientation = Orientation.Horizontal;
            stk_search.Children.Add(stk_buttons);

            var stk_result = new StackPanel();
            stk_search.Children.Add(stk_result);

            var cb_Type = MyCbBxs.ModelOptions(new CbModelType(), false);
            cb_Type.Margin = new Thickness(0, 0, 4, 0);
            stk_buttons.Children.Add(cb_Type);

            var btn_search = new Button();
            btn_search.Content = "Search suitable quest links";
            btn_search.Width = 200;
            stk_buttons.Children.Add(btn_search);
            btn_search.Click += async (source, e) =>
            {
                QuestControl.LoadEveryCrossData();

                var watcher = new Stopwatch();
                watcher.Start();

                var suitable_quests = new List<IQuest>();

                var quests = QuestControl.Get(cb_Type.SelectedModalType);
                int actual = 1;
                var task = quests.Select(quest => Task.Factory.StartNew(() =>
                {
                    if (cb_Type.SelectedModalType == Model.Col)
                    {
                        if (AutoGetSentences.DoesSenContainsCol((ColVM)quest, sen.Text))
                        {
                            if (!QuestSenControl.Get(cb_Type.SelectedModalType).Any(qs => qs.IdQuest == quest.Id && qs.IdSen == sen.Id))
                                suitable_quests.Add(quest);
                        }
                    }

                    var log = "Analysing " + cb_Type.SelectedModalType.ToDesc() + " " + actual + " of " + quests.Count() + ". ";

                    Footer.Log(log);
                    actual = actual + 1;
                }));
                await Task.WhenAll(task);

                foreach (var quest in suitable_quests)
                {
                    if (!stk_result.Children.OfType<CheckQuest>().Any(x => x.Id == quest.Id))
                    {

                        var check_suitable = new CheckQuest(quest, false);
                        check_suitable.Content += " (" + quest.Type.ToDesc().Substring(0, 3) + ")";
                        stk_result.Children.Add(check_suitable);
                    }
                }

                Footer.Log(suitable_quests.Count + " suitable questions were found to this sentence. " +
                            "Time spent: " + Math.Round(watcher.Elapsed.TotalSeconds, 2) + " seconds.");
            };

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

        public static void LoadSentences(StackPanel reference, IQuest quest, StackPanel stackSentences)
        {
            foreach (var qs in quest.Sentences)
            {
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
                stackSentences.Children.Add(check);
            }
        }
    }
}
