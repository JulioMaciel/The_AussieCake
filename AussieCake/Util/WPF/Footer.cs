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
		private static int ExtraQuantity = 0;
		private static TaskScheduler Ts_scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		private static IProgress<string> Reporter;

		public static async Task LogFooterOperation(ModelsType type, OperationType operation, int quantity)
		{
			await Task.Factory.StartNew(() =>
			{
				LabelFooter.Visibility = Visibility.Visible;
				ProgBar.Visibility = Visibility.Collapsed;

				var finalInfo = quantity + " ";
				finalInfo += type.ToDescriptionString();
				finalInfo += quantity <= 1 ? " was " : "s were ";

				finalInfo += operation == OperationType.Created ? "created" :
								 operation == OperationType.Removed ? "removed" :
								 operation == OperationType.Updated ? "updated" : string.Empty;

				finalInfo += ExtraQuantity != 0 ? (" and " + ExtraQuantity + " " + ExtraModel.ToDescriptionString()) : string.Empty;
				finalInfo += ExtraQuantity == 1 ? " was " : (ExtraQuantity > 1 ? "s were " : string.Empty);

				finalInfo += " in ";
				finalInfo += Watcher.Elapsed.TotalSeconds;
				finalInfo += " seconds.";

				LabelFooter.Content = finalInfo;
				ExtraQuantity = 0;
			}, System.Threading.CancellationToken.None,
				 TaskCreationOptions.None,
				 Ts_scheduler);
		}

		public static async Task StartProgress(int max)
		{
			await Task.Factory.StartNew(() =>
			{
				LabelFooter.Visibility = Visibility.Collapsed;
				ProgBar.Visibility = Visibility.Visible;

				Reporter = new Progress<String>(str => ProgBar.Value++);

				ProgBar.Maximum = max;

				Watcher = new Stopwatch();
				Watcher.Start();
			}, System.Threading.CancellationToken.None,
				 TaskCreationOptions.None,
				 Ts_scheduler);
		}

		public static async Task IncreaseProgress()
		{
			await Task.Factory.StartNew(() =>
			{
				Reporter.Report("1");
				//if (ProgBar.Value == 1400)
				//	ProgBar.Value = 1;

				//ProgBar.Value++;
			}, System.Threading.CancellationToken.None,
				 TaskCreationOptions.None,
				 Ts_scheduler);
		}

		public static async Task AddExtraInfo(ModelsType extraType, int extraQuantity)
		{
			await Task.Factory.StartNew(() =>
			{
				if (ExtraQuantity == 0)
				{
					ExtraModel = extraType;
					ExtraQuantity = extraQuantity;
				}
				else
					ExtraModel++;
			}, System.Threading.CancellationToken.None,
				 TaskCreationOptions.None,
				 Ts_scheduler);
		}
	}

	public enum OperationType
	{
		Created,
		Removed,
		Updated
	}
}
