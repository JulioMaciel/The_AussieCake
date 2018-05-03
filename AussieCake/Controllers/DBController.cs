using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.Controllers
{
	public static class DBController
	{
		public static List<UserVM> Users { get; private set; }
		public static List<SentenceVM> Sentences { get; private set; }
		public static List<CollocationVM> Collocations { get; private set; }
		public static List<CollocationAttemptVM> CollocationAttempts { get; private set; }

		public static void LoadData()
		{
			SqLiteHelper.InitializeDB();

			LoadViewModels();
		}

		public static void LoadViewModels()
    {
      LoadUsersViewModel();
      LoadSentencesViewModel();
      LoadCollocationsViewModel();
      LoadAttemptsViewModel();
    }

    public static void LoadAttemptsViewModel()
    {
      if (CollocationAttempts == null)
        CollocationAttempts = new List<CollocationAttemptVM>();

      foreach (var col in SqLiteHelper.CollocationAttemptsDB)
      {
        var vm = new CollocationAttemptVM(col);
        if (!CollocationAttempts.Contains(vm))
          CollocationAttempts.Add(vm);
      }
    }

    public static void LoadCollocationsViewModel()
    {
      if (Collocations == null)
        Collocations = new List<CollocationVM>();

      foreach (var col in SqLiteHelper.CollocationsDB)
      {
        var vm = new CollocationVM(col);
        if (!Collocations.Contains(vm))
          Collocations.Add(vm);
      }
    }

    public static void LoadSentencesViewModel()
    {
      if (Sentences == null)
        Sentences = new List<SentenceVM>();

      foreach (var sen in SqLiteHelper.SentencesDB)
      {
        //var vm = new SentenceVM(sen);
        if (!Sentences.Any(v => v.Id == sen.Id))
					Sentences.Add(new SentenceVM(sen));
      }
    }

    public static void LoadUsersViewModel()
    {
      if (Users == null)
        Users = new List<UserVM>();

      foreach (var user in SqLiteHelper.UsersDB)
      {
        var vm = new UserVM(user);
        if (!Users.Contains(vm))
          Users.Add(vm);
      }
    }
  }
}
