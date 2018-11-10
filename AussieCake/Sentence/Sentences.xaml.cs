using AussieCake.Question;
using AussieCake.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Sentence
{
    /// <summary>
    /// Interaction logic for Sentences.xaml
    /// </summary>
    public partial class Sentences : UserControl
    {
        public Sentences()
        {
            InitializeComponent();

            LoadSentencesOnGrid(false);

            txt_input.Focus();
        }

        private void LoadSentencesOnGrid(bool isGridUpdate)
        {
            var watcher = new Stopwatch();
            watcher.Start();

            var sens = SenControl.Get();
            SenControl.PopulateQuestions();

            foreach (var sen in sens)
                SentenceWPF.AddSentenceRow(stk_sentences, sen, isGridUpdate);

            Footer.Log(sens.Count() + " sentences loaded in " + Math.Round(watcher.Elapsed.TotalSeconds, 2) + " seconds.");
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            var sen = txt_input.Text;
            var vm = new SenVM(sen);

            if (SenControl.Insert(vm, true))
            {
                LoadSentencesOnGrid(true);
                txt_input.Text = string.Empty;
                //btnInsert.IsEnabled = true;
            }
        }

        private async void GetFromText_Click(object sender, RoutedEventArgs e)
        {
            var sentencesFound = await AutoGetSentences.GetSentencesFromString(txt_input.Text);
            InsertLinkLoad(sentencesFound);
        }

        private async void GetFromWeb_Click(object sender, RoutedEventArgs e)
        {
            var sentencesFound = await FileHtmlControls.GetSentencesFromSite(txt_input.Text);
            InsertLinkLoad(sentencesFound);
        }

        private async void GetFromBooks_Click(object sender, RoutedEventArgs e)
        {
            var sentencesFound = await FileHtmlControls.SaveSentencesFromTxtBooks();
            InsertLinkLoad(sentencesFound);
        }

        private void InsertLinkLoad(List<string> sentencesFound)
        {
            foreach (var found in sentencesFound)
            {
                var vm = new SenVM(found);
                SenControl.Insert(vm, false);
            }

            SenControl.PopulateQuestions();

            LoadSentencesOnGrid(true);

            Footer.Log("Auto insert has finished. " + sentencesFound.Count + " sentences were inserted.");
        }

        private void txt_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisableInputElements();

            if (txt_input.Text.StartsWith("http") || txt_input.Text.StartsWith("www"))
                btnGetWeb.IsEnabled = true;
            else if (txt_input.Text.IsDigitsOnly())
                btnFilter.IsEnabled = true;
            else if (!Errors.IsNullSmallerOrBigger(txt_input.Text, SenVM.MinSize, SenVM.MaxSize, false))
                btnInsert.IsEnabled = true;
            else
                btnGetBooks.IsEnabled = true;
        }

        private void DisableInputElements()
        {
            btnGetWeb.IsEnabled = false;
            btnGetText.IsEnabled = false;
            btnGetBooks.IsEnabled = false;
            btnInsert.IsEnabled = false;
            btnFilter.IsEnabled = false;
        }

        private void lblInput_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txt_input.Text = string.Empty;
        }

        private void lblInput_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            txt_input.Text = Clipboard.GetText();
        }

        private async void btnLink_Click(object sender, RoutedEventArgs e)
        {
            var result = new List<string>();

            var watcher = new Stopwatch();
            watcher.Start();

            var links_found = new List<(int, int)>();
            Task tasks = Task.Run(() => Parallel.ForEach(QuestControl.Get(Model.Col), col =>
            {
                foreach (var sen in SenControl.Get())
                {
                    if (AutoGetSentences.DoesSenContainsCol((ColVM)col, sen.Text))
                    {
                        if (!QuestSenControl.Get(Model.Col).Any(qs => qs.IdQuest == col.Id && qs.IdSen == sen.Id))
                            links_found.Add((col.Id, sen.Id));
                    }

                    Footer.Log("Analysing " + SenControl.Get().Count() + " sentences for collocation id " + col.Id + " of " +
                        QuestControl.Get(Model.Col).Count() + ". " + result.Count + " sentences added in " +
                        Math.Round(watcher.Elapsed.TotalMinutes, 2) + " minutes.");
                }
            }));
            await Task.WhenAll(tasks);

            foreach (var qs in links_found)
                QuestSenControl.Insert(new QuestSenVM(qs.Item1, qs.Item2, true, Model.Col));

            //SenControl.PopulateQuestions();
            LoadSentencesOnGrid(true);

            Footer.Log(links_found.Count + " sentences were linked to collocations. Time spent: " +
                        Math.Round(watcher.Elapsed.TotalMinutes, 2) + " minutes.");
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void cb_Active_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void cb_PtBr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void cb_Questions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            var watcher = new Stopwatch();
            watcher.Start();

            if (!cb_PtBr.IsLoaded || !cb_Questions.IsLoaded)
                return;

            var inputFilter = txt_input.Text.Any() ? txt_input.Text : string.Empty;

            if (stk_sentences != null)
                stk_sentences.Children.Clear();

            if (inputFilter.IsDigitsOnly() && SenControl.Get().Any(x => x.Id == Convert.ToInt16(inputFilter)))
            {
                var sen = SenControl.Get().First(x => x.Id == Convert.ToInt16(inputFilter));
                SentenceWPF.AddSentenceRow(stk_sentences, sen, false);
                return;
            }

            int count = 0;
            foreach (var sen in SenControl.Get())
            {
                if (sen.Text != string.Empty && !sen.Text.ContainsInsensitive(inputFilter))
                    continue;

                if (cb_PtBr != null && cb_PtBr.SelectedIndex != 0)
                {
                    if (cb_PtBr.SelectedIndex == 1 && !sen.PtBr.Any())
                        continue;
                    else if (cb_PtBr.SelectedIndex == 2 && sen.PtBr.Any())
                        continue;
                }

                if (cb_Questions != null && cb_Questions.SelectedIndex != 0)
                {
                    if (cb_Questions.SelectedIndex == 1 && !sen.Questions.Any())
                        continue;
                    else if (cb_Questions.SelectedIndex == 2 && sen.Questions.Any())
                        continue;
                }

                SentenceWPF.AddSentenceRow(stk_sentences, sen, false);
                count = count++;
            }

            Footer.Log(count + " sentences loaded in " + watcher.Elapsed.TotalSeconds + " seconds.");
        }
    }
}
