using AussieCake.Question;
using AussieCake.Util;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AussieCake.Sentence
{
    public static class SentenceWpfController
    {
        public static void AddIntoItems(StackPanel stack_items, SenVM sen, bool isNew)
        {
            var item_line = MyStacks.GetItemLine(stack_items, isNew);
            item_line.Margin = new Thickness(0, 1, 2, 0);
            item_line.Name = "line_" + sen.Id;
            AddIntoThis(sen, item_line);
        }

        public static void AddIntoThis(SenVM sen, StackPanel item_line)
        {
            item_line.Children.Clear();

            var row1 = MyGrids.GetRowItem(new List<int>() {
                18, 1, 1, 1
            }, item_line);
            row1.Margin = new Thickness(2, 2, 1, 0);

            var row3 = new StackPanel();
            row3.Margin = new Thickness(1, 2, 1, 2);
            item_line.Children.Add(row3);

            var stack_quests = new StackPanel();

            var btn_edit = MyBtns.Edit_sentence(0, 2, row1, sen, item_line);

            var txt_sen = new TextBox();
            MyTxts.Get(txt_sen, 0, 0, row1, sen.Text);
            txt_sen.TextChanged += (source, e) => CheckIfItemWasEdited(sen, txt_sen, btn_edit);

            var show_quests = MyBtns.Sen_show_questions(0, 1, row1, sen, stack_quests);

            var btn_remove = MyBtns.Remove_sentence(0, 3, row1, sen, item_line);

            MyStacks.Sen_quests(stack_quests, sen, row3);
        }

        private static void CheckIfItemWasEdited(SenVM sen, TextBox txt_sen, Button btn_edit)
        {
            var wasTextEdited = txt_sen.Text != sen.Text;

            if (wasTextEdited)
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

        public async static Task<List<(int, int)>> LinkQuestType(StackPanel stk_sentences, Model type, Stopwatch watcher)
        {
            QuestControl.LoadCrossData(type);
            var tasks = LinkQuestToSentences(type, watcher);
            await Task.WhenAll(tasks);
            var links_found = tasks.Result;

            foreach (var qs in links_found)
            {
                QuestSenControl.Insert(new QuestSenVM(qs.Item1, qs.Item2, true, type));
                var item_line = stk_sentences.Children.OfType<StackPanel>().First(x => x.Name == "line_" + qs.Item2);
                var sen = SenControl.Get().First(x => x.Id == qs.Item2);
                AddIntoThis(sen, item_line);
            }

            return links_found;
        }

        public async static Task<List<(int, int)>> LinkQuestToSentences(Model type, Stopwatch watcher)
        {
            var result = new List<string>();
            var links_found = new List<(int, int)>();
            var quests = QuestControl.Get(type).Where(x => x.Sentences.Count <= 5);
            int actual = 1;
            var task = quests.Select(quest => Task.Factory.StartNew(() =>
            {
                foreach (var sen in SenControl.Get())
                {
                    if (type == Model.Col)
                    {
                        if (AutoGetSentences.DoesSenContainsCol((ColVM)quest, sen.Text))
                        {
                            if (!QuestSenControl.Get(type).Any(qs => qs.IdQuest == quest.Id && qs.IdSen == sen.Id))
                                links_found.Add((quest.Id, sen.Id));
                        }
                    }
                }

                var log = "Analysing " + type.ToDesc()  + " " + actual + " of " + quests.Count() + ". ";
                log += result.Count + " sentences added in " + Math.Round(watcher.Elapsed.TotalSeconds, 1) + " seconds. ";

                if (actual > 5)
                {
                    var quant_missing = quests.Count() - actual;
                    var time_to_finish = (watcher.Elapsed.TotalSeconds * quant_missing) / actual;
                    log += "It must finish in " + Math.Round(time_to_finish, 1) + " seconds.";
                }

                Footer.Log(log);
                actual = actual + 1;
            }));
            await Task.WhenAll(task);

            return links_found;
        }
    }
}
