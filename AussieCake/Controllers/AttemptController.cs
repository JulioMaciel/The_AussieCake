using AussieCake.Models;
using AussieCake.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace AussieCake.Controllers
{
	public class AttemptController : SqLiteHelper
	{
		public static List<CollocationAttemptVM> CollocationAttempts { get; private set; }

		public static void Insert(CollocationAttemptVM collocation)
		{
			var model = new CollocationAttempt(collocation);

			InsertCollocationAttempt(model);
			LoadAttemptsViewModel();
			CollocationController.LoadCollocationsViewModel();
		}

		public static void LoadAttemptsViewModel()
		{
			if (CollocationAttempts == null)
				CollocationAttempts = new List<CollocationAttemptVM>();

			foreach (var col in CollocationAttemptsDB)
			{
				var vm = new CollocationAttemptVM(col);
				if (!CollocationAttempts.Contains(vm))
					CollocationAttempts.Add(vm);
			}
		}
	}
}
