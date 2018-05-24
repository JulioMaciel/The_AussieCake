using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.Util.WPF;
using AussieCake.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AussieCake.Util
{
	public static class SentenceWPF
	{
		public static void AddSentenceRow(StackPanel main_stack, SentenceVM sen, bool isGridUpdate)
		{
			var lineName = "line_" + sen.Id;

      if (sen.Id == 106)
        Console.Write("Debug");

			if (main_stack.Children.OfType<StackPanel>().Any(l => l.Name == lineName))
				return;

			var line = new StackPanel();
			line.Name = lineName;
			line.Background = isGridUpdate ? Brushes.SteelBlue : Brushes.LightSteelBlue;
			line.MouseEnter += new MouseEventHandler((source, e) => line.Background = Brushes.CornflowerBlue);
			line.MouseLeave += new MouseEventHandler((source, e) => line.Background = Brushes.LightSteelBlue);
			main_stack.Children.Add(line);

			var gridSentence = new Grid();
			gridSentence.Margin = new Thickness(8, 8, 8, 2);
			var columnSen = new ColumnDefinition();
			columnSen.Width = new GridLength(100, GridUnitType.Star);
			var columnBtnUpdate = new ColumnDefinition();
			columnBtnUpdate.Width = new GridLength(8, GridUnitType.Star);
			var columnBtnRemove = new ColumnDefinition();
			columnBtnRemove.Width = new GridLength(8, GridUnitType.Star);
			var rowSen = new RowDefinition();
			var rowPtBr = new RowDefinition();
			gridSentence.ColumnDefinitions.Add(columnSen);
			gridSentence.ColumnDefinitions.Add(columnBtnUpdate);
			gridSentence.ColumnDefinitions.Add(columnBtnRemove);
			gridSentence.RowDefinitions.Add(rowSen);
			gridSentence.RowDefinitions.Add(rowPtBr);
			line.Children.Add(gridSentence);

			var txt_sen = new TextBox();
			txt_sen.FontSize = 14;
			txt_sen.Margin = new Thickness(0, 0, 0, 4);
			txt_sen.Text = sen.Text;
			txt_sen.VerticalContentAlignment = VerticalAlignment.Center;
			//txt_sen.Name = "txt_sen_" + sen.Id;
			txt_sen.TextChanged += (source, e) => CreateEditButtonEffect(sen, source, gridSentence);
			Grid.SetRow(txt_sen, 0);
			Grid.SetColumn(txt_sen, 0);
			gridSentence.Children.Add(txt_sen);

			var txt_ptBr = new TextBox();
			txt_ptBr.FontSize = 14;
			txt_ptBr.Margin = new Thickness(0, 0, 0, 8);
			txt_ptBr.Text = sen.PtBr;
			txt_ptBr.VerticalContentAlignment = VerticalAlignment.Center;
			//txt_ptBr.Name = "txt_ptBr_" + sen.Id;
			txt_ptBr.TextChanged += (source, e) => CreateEditButtonEffect(sen, source, gridSentence);
			txt_ptBr.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#edfaeb"));
			Grid.SetRow(txt_ptBr, 1);
			Grid.SetColumn(txt_ptBr, 0);
			gridSentence.Children.Add(txt_ptBr);

			var btn_edit = new Button();
			btn_edit.Content = GetIconButton("save");
			btn_edit.Opacity = 0.5;
			//btn_edit.Name = "btn_" + sen.Id;
			btn_edit.Background = Brushes.Transparent;
      btn_edit.BorderBrush = Brushes.Transparent;
			btn_edit.VerticalAlignment = VerticalAlignment.Center;
			btn_edit.HorizontalAlignment = HorizontalAlignment.Center;
			btn_edit.Height = 38;
			btn_edit.Width = 38;
			btn_edit.IsEnabled = false;
			btn_edit.Click += (source, e) =>
			{
				Logger.StartProgress(1);
				var edited = SentenceController.Sentences.FirstOrDefault(s => s.Id == sen.Id);
				edited.Text = txt_sen.Text;
				edited.PtBr = txt_ptBr.Text;
				SentenceController.Update(edited);
				btn_edit.IsEnabled = false;
				btn_edit.Opacity = 0.5;
				var loggedItem = new LoggedItem(ModelType.Sentence, edited.Text, edited.Id);
				Logger.LogOperation(OperationType.Updated, loggedItem);
			};
			Grid.SetRow(btn_edit, 0);
			Grid.SetColumn(btn_edit, 1);
			Grid.SetRowSpan(btn_edit, 2);
			gridSentence.Children.Add(btn_edit);

			var btn_remove = new Button();
			btn_remove.Content = GetIconButton("remove");
			btn_remove.Background = Brushes.Transparent;
      btn_remove.BorderBrush = Brushes.Transparent;
      btn_remove.VerticalAlignment = VerticalAlignment.Center;
			btn_remove.HorizontalAlignment = HorizontalAlignment.Center;
			btn_remove.Height = 38;
			btn_remove.Width = 38;
			btn_remove.Opacity = 0.9;
			btn_remove.Click += (source, e) =>
			{
				Logger.StartProgress(1);
				var removed = SentenceController.Sentences.FirstOrDefault(s => s.Id == sen.Id);
				SentenceController.Remove(removed);
				line.Visibility = Visibility.Collapsed;
				var loggedItem = new LoggedItem(ModelType.Sentence, removed.Text, removed.Id);
				Logger.LogOperation(OperationType.Removed, loggedItem);
			};
			Grid.SetRow(btn_remove, 0);
			Grid.SetColumn(btn_remove, 2);
			Grid.SetRowSpan(btn_remove, 2);
			gridSentence.Children.Add(btn_remove);

		}

		private static Image GetIconButton(string iconFile)
		{
			var btn_icon = new Image();
			btn_icon.Source = new BitmapImage(new Uri(CakePaths.GetIconPath(iconFile)));
			return btn_icon;
		}

		private static void CreateEditButtonEffect(SentenceVM sen, object source, Grid gridSentence)
		{
			var btn = gridSentence.Children.OfType<Button>().FirstOrDefault();
			if (sen.Text != (source as TextBox).Text)
			{
				btn.IsEnabled = true;
				btn.Opacity = 1;
			}
			else
			{
				btn.IsEnabled = false;
				btn.Opacity = 0.5;
			}
		}
	}
}
