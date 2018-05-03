using AussieCake.Context;
using AussieCake.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using AussieCake.Helper;
using System.Data;
using System.Collections;
using System.IO;
using AussieCake.Controllers;
using AussieCake.Views;

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

      //SqLiteHelper.InitializeDB();      
      //FakeData.InsertInitialFakeData();
		}

		private void btnSentences_click(object sender, RoutedEventArgs e)
		{
			frame_content.NavigationService.Navigate(new Sentences());
      btnSentences.IsEnabled = false;
      lblTopic.Content = ModelsType.Sentence.ToDescriptionString() + 's';
		}
	}
}
