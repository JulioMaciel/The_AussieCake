using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.Util;
using AussieCake.Util.WPF;
using AussieCake.ViewModels;
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

			txt_sentence.Focus();
		}

		private void LoadSentencesOnGrid(bool isGridUpdate)
		{
			if (!isGridUpdate)
				Logger.StartProgress(1);

			foreach (var sen in SentenceController.Sentences)
        SentenceWPF.AddSentenceRow(stk_sentences, sen, isGridUpdate);

			if (!isGridUpdate)
				Logger.LogLoading(ModelType.Sentence, SentenceController.Sentences.Count());
		}

    private async void Insert_Click(object sender, RoutedEventArgs e)
		{
			Logger.StartProgress(CollocationController.Collocations.Count());

			var sen = txt_sentence.Text;
			var ptBr = txt_ptBr.Text;
			// add isActive checkbox on interface
			var vm = new SentenceVM(sen, ptBr, false);

			if (string.IsNullOrEmpty(sen))
			{
				await SentenceController.SaveSentencesFromTxtBooks();
			}
			else if (sen == "html")
			{
				await SentenceController.SaveSentencesFromHtmlBooks();
			}
			else if (sen.StartsWith("http") || sen.StartsWith("www"))
			{
				await SentenceController.SaveSentencesFromSite(sen);
			}
			else if (sen.Length > 500)
			{
				await SentenceController.SaveSentencesFromString(sen);
			}
			else
      {
				SentenceController.Insert(vm);				
			}
			
			LoadSentencesOnGrid(true);
			txt_sentence.Text = string.Empty;
			txt_ptBr.Text = string.Empty;
			btnInsert.IsEnabled = true;

			Logger.LogOperation(OperationType.Created);
		}

	}
}
