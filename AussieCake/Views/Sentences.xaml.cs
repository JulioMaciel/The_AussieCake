using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.Util;
using AussieCake.Util.WPF;
using AussieCake.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Views
{
    /// <summary>
    /// Interaction logic for Sentences.xaml
    /// </summary>
    public partial class Sentences : Page
    {
        public Sentences()
        {
            InitializeComponent();

            LoadSentencesOnGrid(false);

            txt_input.Focus();
        }

        private void LoadSentencesOnGrid(bool isGridUpdate)
        {
            if (!isGridUpdate)
                Logger.StartProgress(1);

            SentenceController.PopulateQuestions();

            foreach (var sen in SentenceController.Sentences)
                SentenceWPF.AddSentenceRow(stk_sentences, sen, isGridUpdate);

            if (!isGridUpdate)
                Logger.LogLoading(ModelType.Sentence, SentenceController.Sentences.Count());
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            Logger.StartProgress(CollocationController.Collocations.Count());

            var sen = txt_input.Text;
            var vm = new SentenceVM(sen, false);

            SentenceController.Insert(vm);

            LoadSentencesOnGrid(true);
            txt_input.Text = string.Empty;
            btnInsert.IsEnabled = true;

            Logger.LogOperation(OperationType.Created);
        }


        private async void GetFromText_Click(object sender, RoutedEventArgs e)
        {
            Logger.StartProgress(CollocationController.Collocations.Count());

            await SentenceController.SaveSentencesFromString(txt_input.Text);

            LoadSentencesOnGrid(true);
            Logger.LogOperation(OperationType.Created);
        }

        private async void GetFromWeb_Click(object sender, RoutedEventArgs e)
        {
            Logger.StartProgress(CollocationController.Collocations.Count());

            await SentenceController.SaveSentencesFromSite(txt_input.Text);

            LoadSentencesOnGrid(true);
            Logger.LogOperation(OperationType.Created);
        }

        private async void GetFromBooks_Click(object sender, RoutedEventArgs e)
        {
            Logger.StartProgress(CollocationController.Collocations.Count());

            await SentenceController.SaveSentencesFromTxtBooks();

            LoadSentencesOnGrid(true);
            Logger.LogOperation(OperationType.Created);
        }

        private void txt_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisableInputElements();

            if (txt_input.Text.StartsWith("http") || txt_input.Text.StartsWith("www"))
                btnGetWeb.IsEnabled = true;
            else if (txt_input.Text.Length >= 300)
                btnGetText.IsEnabled = true;
            else if (txt_input.Text.Length >= 40 && txt_input.Text.Length <= 80)
                btnInsert.IsEnabled = true;
            else if (txt_input.Text.Length > 1)
                btnFilter.IsEnabled = true;
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
            if (!cb_Active.IsLoaded || !cb_PtBr.IsLoaded || !cb_Questions.IsLoaded)
                return;

            var inputFilter = txt_input.Text.Any() ? txt_input.Text : string.Empty;

            if (stk_sentences != null)
                stk_sentences.Children.Clear();

            foreach (var sen in SentenceController.Sentences)
            {
                if (sen.Text != string.Empty && !sen.Text.ContainsInsensitive(inputFilter))
                    continue;

                if (cb_Active != null && cb_Active.SelectedIndex != 0)
                {
                    if (cb_Active.SelectedIndex == 1 && !sen.IsActive)
                        continue;
                    else if (cb_Active.SelectedIndex == 2 && sen.IsActive)
                        continue;
                }

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
            }
        }
    }
}
