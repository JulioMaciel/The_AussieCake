using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.Util;
using AussieCake.Util.WPF;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
      foreach (var sen in DBController.Sentences)
      {
        SentenceWPF.AddSentenceRow(stk_sentences, sen, isGridUpdate);
      }
		}

    private async void Insert_Click(object sender, RoutedEventArgs e)
    {
			var sen = txt_sentence.Text;
			var ptBr = txt_ptBr.Text;

      if (string.IsNullOrEmpty(sen))
        return;
						
			Footer.StartProgress(DBController.Collocations.Count());

			if (sen.StartsWith("http") || sen.StartsWith("www"))
			{
				await SentenceInText.SaveSentencesFromSite(sen);
			}
			else if (sen.Length > 500)
			{
				await SentenceInText.SaveSentencesFromString(sen);
			}
			else
      {
				var vm = new SentenceVM(sen, ptBr);
				SentenceController.Insert(vm);
				Footer.LogFooterOperation(ModelsType.Sentence, OperationType.Created, 1);
			}
			
			LoadSentencesOnGrid(true);
			txt_sentence.Text = string.Empty;
			txt_ptBr.Text = string.Empty;
			btnInsert.IsEnabled = true;			
		}
	}
}
