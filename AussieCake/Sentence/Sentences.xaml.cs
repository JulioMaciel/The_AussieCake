using AussieCake.Util;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Sentence
{
    /// <summary>
    /// Interaction logic for Sentences.xaml
    /// </summary>
    public partial class Sentences : UserControl
    {
        bool btn_insert_was;
        bool btn_get_web_was;
        bool btn_get_text_was;
        bool btn_import_text_was;


        public Sentences()
        {
            InitializeComponent();

            LoadSentencesOnGrid(false);

            MyCbBxs.ModelOptions(cb_QuestType, true);

            txt_input.Focus();
        }

        private void LoadSentencesOnGrid(bool isGridUpdate)
        {
            var watcher = new Stopwatch();
            watcher.Start();

            var sens = SenControl.Get();
            SenControl.PopulateQuestions();

            sens = sens.Take(60);

            foreach (var sen in sens)
                SentenceWpfController.AddIntoItems(stk_sentences, sen, false);

            Footer.Log(sens.Count() + " sentences loaded in " + Math.Round(watcher.Elapsed.TotalSeconds, 2) + " seconds.");
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            var sen = txt_input.Text;
            var vm = new SenVM(sen);

            if (SenControl.Insert(vm, true))
            {
                //LoadSentencesOnGrid(true);
                txt_input.Text = string.Empty;
                var added = SenControl.Get().Last();
                SentenceWpfController.AddIntoItems(stk_sentences, added, true);

                Footer.Log("The sentence has been inserted.");
            }
        }

        private async void GetFromText_Click(object sender, RoutedEventArgs e)
        {
            SaveBtnUIStatus();
            var sentencesFound = await AutoGetSentences.GetSentencesFromString(txt_input.Text);
            InsertLinkLoad(sentencesFound);
            RestoreBtnUIStatus();
        }

        private async void GetFromWeb_Click(object sender, RoutedEventArgs e)
        {
            SaveBtnUIStatus();
            var sentencesFound = await FileHtmlControls.GetSentencesFromSite(txt_input.Text);
            InsertLinkLoad(sentencesFound);
            RestoreBtnUIStatus();
        }

        private async void ImportFromBooks_Click(object sender, RoutedEventArgs e)
        {
            SaveBtnUIStatus();
            var sentencesFound = await FileHtmlControls.SaveSentencesFromTxtBooks();
            InsertLinkLoad(sentencesFound);
            RestoreBtnUIStatus();
        }

        private async void btnImportFromWeb_Click(object sender, RoutedEventArgs e)
        {
            SaveBtnUIStatus();
            var sentencesFound = await FileHtmlControls.ImportSentencesFromLudwig(cb_QuestType.SelectedModalType);
            InsertLinkLoad(sentencesFound);
            RestoreBtnUIStatus();
        }

        private void InsertLinkLoad(List<string> sentencesFound)
        {
            int auto_inserted = 0;
            foreach (var found in sentencesFound)
            {
                var vm = new SenVM(found);
                if (SenControl.Insert(vm, false))
                {
                    var added_sen = SenControl.Get().Last();
                    SentenceWpfController.AddIntoItems(stk_sentences, added_sen, true);
                    auto_inserted = auto_inserted + 1;
                }
            }

            Footer.Log("Auto insert has finished. " + auto_inserted + " sentences were inserted.");
        }

        private void txt_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisableInputElements();

            if (txt_input.Text.StartsWith("http") || txt_input.Text.StartsWith("www"))
                btnGetWeb.IsEnabled = true;
            else if (!Errors.IsNullSmallerOrBigger(txt_input.Text, SenVM.MinSize, SenVM.MaxSize, false))
                btnInsert.IsEnabled = true;
            else
                btnGetBooks.IsEnabled = true;
        }

        private void DisableInputElements()
        {
            btnGetWeb.IsEnabled = false;
            btnGetText.IsEnabled = false;
            btnInsert.IsEnabled = false;
            btnImportWeb.IsEnabled = false;
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
            SaveBtnUIStatus();

            var result = new List<string>();

            var watcher = new Stopwatch();
            watcher.Start();

            List<(int, int)> links_found = new List<(int, int)>();

            if (cb_QuestType.SelectedIndex != 0)
                links_found = await SentenceWpfController.LinkQuestType(stk_sentences, cb_QuestType.SelectedModalType, watcher);
            else
            {
                links_found.AddRange(await SentenceWpfController.LinkQuestType(stk_sentences, Model.Col, watcher));
                // and so on;
            }

            RestoreBtnUIStatus();
            Footer.Log(links_found.Count + " sentences were linked to collocations. Time spent: " +
                        Math.Round(watcher.Elapsed.TotalMinutes, 2) + " minutes.");
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            var watcher = new Stopwatch();
            watcher.Start();

            var inputFilter = txt_input.Text.Any() ? txt_input.Text : string.Empty;

            if (stk_sentences != null)
                stk_sentences.Children.Clear();

            if (!inputFilter.IsEmpty())
            {
                if (inputFilter.IsDigitsOnly() && SenControl.Get().Any(x => x.Id == Convert.ToInt16(inputFilter)))
                {
                    var sen = SenControl.Get().First(x => x.Id == Convert.ToInt16(inputFilter));
                    SentenceWpfController.AddIntoItems(stk_sentences, sen, false);
                    return;
                }
            }

            int count = 0;
            foreach (var sen in SenControl.Get())
            {
                if (sen.Text != string.Empty && !sen.Text.ContainsInsensitive(inputFilter))
                    continue;

                if (!txt_quests.IsEmpty() && txt_quests.Text.IsDigitsOnly())
                {
                    var quant = Convert.ToInt16(txt_quests.Text);
                    if (sen.Questions.Count < quant)
                        continue;
                }

                if (cb_QuestType.SelectedIndex != 0)
                {
                    if (!sen.Questions.Any(x => x.Quest.Type == cb_QuestType.SelectedModalType))
                        continue;
                }

                SentenceWpfController.AddIntoItems(stk_sentences, sen, false);
                count = count + 1;

                if (count == 60)
                    break;
            }

            Footer.Log(count + " sentences loaded in " + watcher.Elapsed.TotalSeconds + " seconds.");
        }

        private void SaveBtnUIStatus()
        {
            btn_insert_was = btnInsert.IsEnabled;
            btn_get_web_was = btnGetWeb.IsEnabled;
            btn_get_text_was = btnGetText.IsEnabled;
            btn_import_text_was = btnImportWeb.IsEnabled;
            btnFilter.IsEnabled = false;
            btnGetBooks.IsEnabled = false;
            btnLink.IsEnabled = false;

        }

        private void RestoreBtnUIStatus()
        {
            btnInsert.IsEnabled = btn_insert_was;
            btnGetWeb.IsEnabled = btn_get_web_was;
            btnGetText.IsEnabled = btn_get_text_was;
            btnImportWeb.IsEnabled = btn_import_text_was;
            btnFilter.IsEnabled = true;
            btnGetBooks.IsEnabled = true;
            btnLink.IsEnabled = true;
        }

        private void txt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Filter();
        }

        private void cb_QuestType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_QuestType.SelectedIndex != 0)
                btnImportWeb.IsEnabled = true;
            else
                btnImportWeb.IsEnabled = false;
        }
    }
}
