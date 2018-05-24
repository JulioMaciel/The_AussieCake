using AussieCake.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Util.WPF
{
	public static class Logger
	{
		private static Label LabelFooter = ((MainWindow)Application.Current.MainWindow).lblFooter;
		private static ProgressBar ProgBar = ((MainWindow)Application.Current.MainWindow).progressBar;
		private static Stopwatch Watcher;
		private static IProgress<string> Reporter;
		private static List<LoggedItem> Items;

		public static void LogOperation(OperationType operation)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				LabelFooter.Visibility = Visibility.Visible;
				ProgBar.Visibility = Visibility.Collapsed;

				var logInfo = string.Empty;
				var logPattern = "\n{0} \"{1}\"{2} was {3}.";

				foreach (var item in Items)
				{
					logInfo += string.Format(logPattern, item.Model.ToDescString(), item.Name,
						(item.Id != null ? " (Id " + item.Id + ")" : string.Empty),
						operation.ToDescString());
				}

				var footerInfo = string.Empty;

				var itemsGroup = Items.GroupBy(i => i.Model);
				foreach (var group in itemsGroup)
				{
					if (!string.IsNullOrEmpty(footerInfo))
						footerInfo += ", ";

					footerInfo += group.Count() + " " + group.FirstOrDefault().Model;
					footerInfo += group.Count() > 1 ? "s" : string.Empty;
				}

				if (itemsGroup.Count() == 0)
					footerInfo += "Nothing";

				footerInfo += itemsGroup.Count() > 1 ? " were " : " was ";

				footerInfo += operation.ToDescString() + " in " + Watcher.Elapsed.TotalSeconds + " seconds.";
				LabelFooter.Content = footerInfo;

				logInfo = logInfo.Insert(0, "*************\n" + DateTime.Now.ToString("dd/MM/yy, HH:mm") + ": " + footerInfo);

				using (StreamWriter writetext = new StreamWriter(CakePaths.Log, true))
					writetext.WriteLine(logInfo);

				ProgBar.Value = 0;

			});

			Watcher.Stop();
		}

		public static void LogOperation(OperationType operation, LoggedItem item)
		{
			LogItem(item);
			LogOperation(operation);
		}

		public static void LogLoading(ModelType model, int quantity)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				LabelFooter.Visibility = Visibility.Visible;
				ProgBar.Visibility = Visibility.Collapsed;

				LabelFooter.Content = quantity + " " + model.ToDescString() + "s were loaded in " + Watcher.Elapsed.TotalSeconds + " seconds.";

				ProgBar.Value = 0;
			});

			Watcher.Stop();
		}

		public static void StartProgress(int max)
		{
			Items = new List<LoggedItem>();

			Application.Current.Dispatcher.Invoke(() =>
			{
				LabelFooter.Visibility = Visibility.Collapsed;
				ProgBar.Visibility = Visibility.Visible;

				Reporter = new Progress<String>(str => ProgBar.Value++);

				ProgBar.Maximum = max;
			});

			Watcher = new Stopwatch();
			Watcher.Start();
		}

		public static void IncreaseProgress()
		{
			Application.Current.Dispatcher.Invoke(() => Reporter.Report("1"));
		}

		public static void LogItem(LoggedItem item)
		{
			Items.Add(item);
		}
	}

	public class LoggedItem
	{
		public int? Id { get; private set; }
		public string Name { get; private set; }
		public ModelType Model { get; private set; }

		public LoggedItem(ModelType model, string name)
		{
			Model = model;
			Name = name;
		}

		public LoggedItem(ModelType model, string name, int id) : this(model, name)
		{
			Id = id;
		}
	}

	public enum OperationType
	{
		[Description("Created")]
		Created,

		[Description("Removed")]
		Removed,

		[Description("Updated")]
		Updated,
	}
}
