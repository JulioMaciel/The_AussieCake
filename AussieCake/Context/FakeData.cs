using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.Context
{
	public static class FakeData
	{
		public static void InsertInitialFakeData()
		{
			UserController.ActiveId = 31;

			var user1 = new UserVM(new List<string>() { "Juketo" }, "batata");
			var user2 = new UserVM(new List<string>() { "Noil" }, "50k");

      UserController.Insert(user1);
      UserController.Insert(user2);

      user1 = DBController.Users.FirstOrDefault(x => x.Password == "batata");
      user2 = DBController.Users.FirstOrDefault(x => x.Password == "50k");

      var sentence1 = new SentenceVM("I was followed by someone");
      var sentence2 = new SentenceVM("My book was based on my life", "Meu livro foi baseado na minha vida");

      SentenceController.Insert(sentence1);
      SentenceController.Insert(sentence2);

      sentence1 = DBController.Sentences.FirstOrDefault(x => x.Text.Contains("followed"));
      sentence2 = DBController.Sentences.FirstOrDefault(x => x.Text.Contains("book"));
			
			// esses exemplos são de prepositions
			//var collocation1 = new CollocationVM(empty, "followed", false, empty, "by", empty, "", new List<int>() { sentence1.Id });
			//var collocation2 = new CollocationVM(empty, "based", empty, "on", empty, "", new List<int>() { sentence2.Id });

			//QuestionController.Insert(collocation1);
			//QuestionController.Insert(collocation2);

			//collocation1 = DBController.Collocations.FirstOrDefault(x => x.Component1 == "followed");
			//collocation2 = DBController.Collocations.FirstOrDefault(x => x.Component1 == "based");

			//var collocationAttempt1 = new CollocationAttemptVM(user1.Id, collocation1.Id, true, DateTime.Now.AddDays(-1));
			//var collocationAttempt2 = new CollocationAttemptVM(user1.Id, collocation2.Id, false, DateTime.Now.AddDays(-3));
			//var collocationAttempt3 = new CollocationAttemptVM(user2.Id, collocation1.Id, true, DateTime.Now.AddDays(-5));
			//var collocationAttempt4 = new CollocationAttemptVM(user2.Id, collocation2.Id, true, DateTime.Now.AddDays(-6));

			//AttemptController.Insert(collocationAttempt1);
			//AttemptController.Insert(collocationAttempt1);
			//AttemptController.Insert(collocationAttempt1);
			//AttemptController.Insert(collocationAttempt1);

			Console.WriteLine("Fake data was successfully inserted and checked.");
		}

		public static void RemoveFakeData()
		{
			// TODO
		}

	}
}
