using System.Linq;
using System.Threading.Tasks;

namespace AussieCake.Controllers
{
	public class DBController : SqLiteHelper
	{
		public static void LoadData()
		{
			InitializeDB();
			LoadViewModels();

			if (WasDataBaseEmpty)
				CreateStaticAssossiations();
		}

		public static void LoadViewModels()
		{
			UserController.LoadUsersViewModel();
			SentenceController.LoadSentencesViewModel();
			CollocationController.LoadCollocationsViewModel();
			AttemptController.LoadAttemptsViewModel();
			VerbController.LoadVerbsViewModel();
		}

		private async static void CreateStaticAssossiations()
		{
			//var tasks = SentenceController.Sentences.Select(async sen =>
			//{
			//	await Task.Run(() =>
			//	{
			//		foreach (var col in CollocationController.Collocations)
			//			SentenceController.DoesSentenceContainsCollection(col, sen);
			//	});
			//}).ToList();
			//await Task.WhenAll(tasks);

			//fazer async
			foreach (var sen in SentenceController.Sentences)
			{
				foreach (var col in CollocationController.Collocations)
					SentenceController.DoesSentenceContainsCollection(col, sen);
			}
		}

	}
}
