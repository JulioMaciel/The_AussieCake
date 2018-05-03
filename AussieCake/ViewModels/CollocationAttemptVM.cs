using AussieCake.Models;
using System;

namespace AussieCake.ViewModels
{
	public class CollocationAttemptVM
	{
		public int Id { get; private set; }

		public int IdUser { get; set; }
		public int IdCollocation { get; set; }
		public bool IsCorrect { get; set; }
		public DateTime When { get; set; }

		public CollocationAttemptVM(int idUser, int idCollocation, bool isCorrect, DateTime when)
		{
			IdUser = idUser;
			IdCollocation = idCollocation;
			IsCorrect = isCorrect;
			When = when;
		}

		public CollocationAttemptVM(CollocationAttempt col)
		{
			Id = col.Id;
			IdUser = col.IdUser;
			IdCollocation = col.IdCollocation;
			IsCorrect = col.IsCorrect;
			When = col.When;
		}

	}
}
