using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.Views;
using System.Windows;

namespace AussieCake
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

      DBController.LoadData();
		}

		private void btnSentences_click(object sender, RoutedEventArgs e)
		{
			frame_content.NavigationService.Navigate(new Sentences());
      btnSentences.IsEnabled = false;
      lblTopic.Content = ModelsType.Sentence.ToDescriptionString() + 's';
		}
	}
}
