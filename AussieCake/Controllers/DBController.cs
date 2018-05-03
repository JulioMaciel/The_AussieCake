namespace AussieCake.Controllers
{
	public class DBController : SqLiteHelper
	{
		public static void LoadData()
		{
			InitializeDB();
			LoadViewModels();
		}

		public static void LoadViewModels()
    {
			UserController.LoadUsersViewModel();
			SentenceController.LoadSentencesViewModel();
			CollocationController.LoadCollocationsViewModel();
			AttemptController.LoadAttemptsViewModel();
			VerbController.LoadVerbsViewModel();
    }
		
  }
}
