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

namespace AussieCake.Sentence
{
    /// <summary>
    /// Interaction logic for Sentences.xaml
    /// </summary>
    public partial class Sentences : UserControl
    {
        bool btn_filter_was;
        bool btn_insert_was;
        bool btn_get_books_was;
        bool btn_get_web_was;
        bool btn_get_text_was;
        bool btn_link_was;

        public Sentences()
        {
            InitializeComponent();

            LoadSentencesOnGrid(false);

            PopulateCbQuestionType();

            txt_input.Focus();
        }

        private void PopulateCbQuestionType()
        {
            var source = new List<ModalTypeCb>();
            source.Add(new ModalTypeCb("Any"));
            
            var quests_types = Enum.GetValues(typeof(Model)).Cast<Model>()
                                                       .Where(x => x != Model.Sen && x != Model.Verb);
            foreach(var type in quests_types)
                source.Add(new ModalTypeCb(type, type.ToDesc()));

            cb_QuestType.ItemsSource = source;
            cb_QuestType.DisplayMemberPath = "Text";
            cb_QuestType.SelectedIndex = 0;
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
                LoadSentencesOnGrid(true);
                txt_input.Text = string.Empty;
                //btnInsert.IsEnabled = true;

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

        private async void GetFromBooks_Click(object sender, RoutedEventArgs e)
        {
            SaveBtnUIStatus();
            var sentencesFound = await FileHtmlControls.SaveSentencesFromTxtBooks();
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
            btnGetBooks.IsEnabled = false;
            btnInsert.IsEnabled = false;
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
            {
                var selected_type = ((ModalTypeCb)cb_QuestType.SelectedValue).Type;
                links_found = await SentenceWpfController.LinkQuestType(stk_sentences, selected_type, watcher);
            }
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
                    var selected_type = ((ModalTypeCb)cb_QuestType.SelectedValue).Type;
                    if (!sen.Questions.Any(x => x.Quest.Type == selected_type))
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
            btn_filter_was = btnFilter.IsEnabled;
            btn_insert_was = btnInsert.IsEnabled;
            btn_get_books_was = btnGetBooks.IsEnabled;
            btn_get_web_was = btnGetWeb.IsEnabled;
            btn_get_text_was = btnGetText.IsEnabled;
            btn_link_was = btnLink.IsEnabled;
        }

        private void RestoreBtnUIStatus()
        {
            btnFilter.IsEnabled = btn_filter_was;
            btnInsert.IsEnabled = btn_insert_was;
            btnGetBooks.IsEnabled = btn_get_books_was;
            btnGetWeb.IsEnabled = btn_get_web_was;
            btnGetText.IsEnabled = btn_get_text_was;
            btnLink.IsEnabled = btn_link_was;
        }

        internal class ModalTypeCb
        {
            public Model Type { get; set; }
            public string Text { get; set; }

            public ModalTypeCb(Model type, string text)
            {
                Type = type;
                Text = text;
            }

            public ModalTypeCb(string text)
            {
                Text = text;
            }
        }
    }
}
