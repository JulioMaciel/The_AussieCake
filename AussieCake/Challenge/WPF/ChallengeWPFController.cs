using AussieCake.Attempt;
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

namespace AussieCake.Challenge
{
    public static class ChalWPFControl
    {
        public async static Task<ChalLine> CreateChalLine(IQuest quest, int row, Grid userControlGrid, Microsoft.Office.Interop.Word.Application wordApp)
        {
            var line = new ChalLine();
            line.Quest = quest;

            line.Chal.Grid_chal = MyGrids.GetChallenge(row, userControlGrid);

            MyStacks.Get(line.Chal.Row_1, 0, 0, line.Chal.Grid_chal);
            line.Chal.Row_1.Visibility = Visibility.Collapsed;

            MyLbls.Chal_answer(line.Chal.Answer, line.Chal.Row_1, GetQuestAnswer(quest));
            MyLbls.Get(line.Chal.PtBr, line.Chal.Row_1, line.Quest.PtBr);
            line.Chal.PtBr.Foreground = Brushes.DarkBlue;
            MyLbls.Get(line.Chal.Definition, line.Chal.Row_1, line.Quest.Definition);

            var stk_2 = await BuildSenChal(line, wordApp);
            UtilWPF.SetGridPosition(stk_2, 1, 0, line.Chal.Grid_chal);

            MyGrids.GetRow(line.Chal.Row_3, 2, 0, line.Chal.Grid_chal, new List<int>() { 1, 1, 1, 1, 1, 1 });
            line.Chal.Row_3.Visibility = Visibility.Collapsed;
            MyLbls.AvgScore(line.Chal.Avg_w, 0, 0, line.Chal.Row_3, line.Quest, 7, false);
            MyLbls.AvgScore(line.Chal.Avg_m, 0, 1, line.Chal.Row_3, line.Quest, 30, false);
            MyLbls.AvgScore(line.Chal.Avg_all, 0, 2, line.Chal.Row_3, line.Quest, 2000, false);
            MyLbls.Tries(line.Chal.Tries, 0, 3, line.Chal.Row_3, line.Quest);
            MyLbls.Get(line.Chal.Importante, 0, 4, line.Chal.Row_3, line.Quest.Importance.ToDesc());
            MyLbls.Chance(line.Chal.Chance, 0, 5, line.Chal.Row_3, line.Quest);
            line.Chal.Chance.Content.ToString().Insert(0, "was ");

            MyGrids.GetRow(line.Chal.Row_4, 3, 0, line.Chal.Grid_chal, new List<int>() { 2, 1, 2, 1, 2 });
            line.Chal.Row_4.Visibility = Visibility.Collapsed;

            MyBtns.Chal_remove_att(line);
            MyLbls.Chal_quest_id(line, 3);
            MyBtns.Chal_disable_quest(line);

            return line;
        }

        private static string GetQuestAnswer(IQuest quest)
        {
            var answer = string.Empty;

            if (quest.Type == Model.Col)
            {
                var col = quest as ColVM;
                answer = col.Prefixes.ToText() + " / " + col.Component1 + " / " +
                        col.LinkWords.ToText() + " / " + col.Component2 + " / " + col.Suffixes.ToText();
            }

            return answer;
        }

        private static ComboChallenge CreateSentence(string sentence, string choosen, bool isVerb, StackPanel parent, Microsoft.Office.Interop.Word.Application wordApp)
        {
            var cb_word = new ComboChallenge();

            foreach (var word in sentence.Split())
            {
                // check if choosen is in any way the 'word'
                var found =  Sentences.GetCompatibleWord(choosen, isVerb, word);

                if (found.Length > 0)
                {
                    MyCbBxs.BuildSynonyms(found, cb_word, parent, char.IsUpper(word[1]), wordApp);
                }
                else
                {
                    var lbl = new Label();
                    lbl.Margin = new Thickness(-3, 0, -3, 0);
                    lbl.Content = word;
                    parent.Children.Add(lbl);
                }
            }

            return cb_word;
        }

        private async static Task<StackPanel> BuildSenChal(ChalLine line, Microsoft.Office.Interop.Word.Application wordApp)
        {
            var stk_sentence = line.Chal.Row_2;
            stk_sentence.Orientation = Orientation.Horizontal;
            stk_sentence.VerticalAlignment = VerticalAlignment.Center;
            stk_sentence.HorizontalAlignment = HorizontalAlignment.Center;

            string sen_from_web = await Sentences.GetSentenceToCollocation(line.Quest);

            line.Choosen_word = ChooseWord(line.Quest, wordApp);
            var isChoosenVerb = IsPartVerb(line.Quest, line.Choosen_word);

            line.Chal.Choosen_word = CreateSentence(sen_from_web, line.Choosen_word, isChoosenVerb, stk_sentence, wordApp);

            foreach (var q_word in stk_sentence.Children.OfType<Label>())
                GetChallengeSentenceChildren(line.Quest, q_word, line);

            return stk_sentence;
        }

        private static void GetChallengeSentenceChildren(IQuest quest, Label q_word, ChalLine line)
        {
            if (quest.Type == Model.Col)
            {
                var col = quest as ColVM;
                var part = q_word.Content.ToString();

                if (col.Prefixes.Any(x => x == part) ||
                    col.LinkWords.Any(x => x == part) ||
                    col.Suffixes.Any(x => x == part) ||
                    !Sentences.GetCompatibleWord(col.Component1, col.IsComp1Verb, part).IsEmpty() ||
                    !Sentences.GetCompatibleWord(col.Component2, col.IsComp2Verb, part).IsEmpty())

                    line.Chal.Quest_words.Add(q_word);
            }
        }

        private static string ChooseWord(IQuest quest, Microsoft.Office.Interop.Word.Application wordApp)
        {
            var choosen = string.Empty;
            var alternative = string.Empty;

            if (quest.Type == Model.Col)
            {
                var col = quest as ColVM;

                if (UtilWPF.RandomNumber(0, 2).ToBool())
                {
                    choosen = col.Component1;
                    alternative = col.Component2;
                }
                else
                {
                    choosen = col.Component2;
                    alternative = col.Component1;
                }
            }

            var hasSynonyms = FileHtmlControls.GetSynonyms(choosen, wordApp).Any();

            if (!hasSynonyms)
                choosen = alternative;

            return choosen;
        }

        private static bool IsPartVerb(IQuest quest, string chosen)
        {
            if (quest.Type == Model.Col)
            {
                var col = quest as ColVM;
                if (chosen == col.Component1 && col.IsComp1Verb || chosen == col.Component2 && col.IsComp2Verb)
                    return true;
            }

            return false;
        }

        public static void Verify(ChalLine line, Button btn_verify, Button btn_next)
        {
            line.Chal.Choosen_word.IsEnabled = false;

            int score = 0;
            if (line.Chal.Choosen_word.IsCorrect())
            {
                line.Chal.Grid_chal.Background = UtilWPF.Colour_Correct;
                score = 10;
            }
            else
                line.Chal.Grid_chal.Background = UtilWPF.Colour_Incorrect;

            var att = new AttemptVM(line.Quest.Id, score, DateTime.Now, line.Quest.Type);
            AttemptsControl.Insert(att);

            var updated_quest = QuestControl.Get(line.Quest.Type).First(x => x.Id == line.Quest.Id);

            line.Chal.Avg_w.Content = updated_quest.Avg_week + "% (w)";
            line.Chal.Avg_m.Content = updated_quest.Avg_month + "% (m)";
            line.Chal.Avg_all.Content = updated_quest.Avg_all + "% (all)";
            line.Chal.Tries.Content = updated_quest.Tries.Count + " tries";

            foreach (var lbl in line.Chal.Quest_words)
                lbl.FontWeight = FontWeights.Bold;

            TurnElemsVisible(line);
            btn_next.IsEnabled = true;
            btn_verify.IsEnabled = false;
        }

        private static void TurnElemsVisible(ChalLine line)
        {
            line.Chal.Row_1.Visibility = Visibility.Visible;
            line.Chal.Row_3.Visibility = Visibility.Visible;
            line.Chal.Row_4.Visibility = Visibility.Visible;
        }

        public async static void PopulateRows(Grid parent, Model type, List<ChalLine> lines, Microsoft.Office.Interop.Word.Application wordApp)
        {
            var watcher = new Stopwatch();
            watcher.Start();

            Footer.Log("Loading...");

            lines.Clear();

            var actual_chosen = new List<int>();
            for (int row = 0; row < 4; row++)
            {
                var quest = QuestControl.GetRandomAvailableQuestion(type, actual_chosen);
                actual_chosen.Add(quest.Id);

                var item = await CreateChalLine(quest, row, parent, wordApp);
                lines.Add(item);
                Footer.Log("Loading... Challenge " + (row + 1) + " was loaded in " + watcher.Elapsed.TotalSeconds + " seconds.");
            };

            Footer.Log("4 challenges loaded in " + watcher.Elapsed.TotalSeconds + " seconds.");
        }
    }

    public class ChalLine
    {
        public ChalWpfItem Chal { get; set; }
        public IQuest Quest { get; set; }
        public string Choosen_word { get; set; }

        public ChalLine()
        {
            Chal = new ChalWpfItem();
        }
    }
}
