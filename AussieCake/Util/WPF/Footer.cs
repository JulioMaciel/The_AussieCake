using AussieCake.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Util.WPF
{
	public static class Footer
	{
		private static Label LabelFooter = ((MainWindow)Application.Current.MainWindow).lblFooter;
		private static ProgressBar ProgBar = ((MainWindow)Application.Current.MainWindow).progressBar;
		private static Stopwatch Watcher;
		private static ModelsType ExtraModel;
		private static int Quantity = 0;
		private static int ExtraQuantity = 0;
		private static IProgress<string> Reporter;

		public static void LogFooterOperation(ModelsType type, OperationType operation, int quantity = 0)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				LabelFooter.Visibility = Visibility.Visible;
				ProgBar.Visibility = Visibility.Collapsed;

				if (quantity != 0)
					Quantity = quantity;

				var finalInfo = Quantity + " ";
				finalInfo += type.ToDescString();
				finalInfo += Quantity <= 1 ? " was " : "s were ";

				finalInfo += operation == OperationType.Created ? "created" :
										 operation == OperationType.Removed ? "removed" :
										 operation == OperationType.Updated ? "updated" :
										 operation == OperationType.Load ? "loaded" : string.Empty;

				finalInfo += ExtraQuantity != 0 ? (" and " + ExtraQuantity + " " + ExtraModel.ToDescString()) : string.Empty;
				finalInfo += ExtraQuantity == 1 ? " was " : (ExtraQuantity > 1 ? "s were " : string.Empty);

				finalInfo += " in ";
				finalInfo += Watcher.Elapsed.TotalSeconds;
				finalInfo += " seconds.";

				LabelFooter.Content = finalInfo;
				Quantity = 0;
				ExtraQuantity = 0;
				ProgBar.Value = 0;
				Watcher.Stop();
			});
		}

		public static void StartProgress(int max)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				LabelFooter.Visibility = Visibility.Collapsed;
				ProgBar.Visibility = Visibility.Visible;

				Reporter = new Progress<String>(str => ProgBar.Value++);

				ProgBar.Maximum = max;

				Watcher = new Stopwatch();
				Watcher.Start();
			});
		}

		public static void IncreaseProgress()
		{
			Application.Current.Dispatcher.Invoke(() => Reporter.Report("1"));
		}

		public static void AddInfo()
		{
			Application.Current.Dispatcher.Invoke(() => Quantity++);
		}

		public static void AddExtraInfo(ModelsType extraType)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				ExtraModel = extraType;
				ExtraQuantity++;
			});
		}
	}

	public enum OperationType
	{
		Created,
		Removed,
		Updated,
		Load,
	}
}
