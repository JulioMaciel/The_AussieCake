using AussieCake.ViewModels;
using System;

namespace AussieCake.Models
{
	public class CollocationAttempt : UserAttempt
	{
		public int IdCollocation { get; set; }

		public CollocationAttempt(int idUser, int idCollocation, bool isCorrect, DateTime when)
		{
			IdUser = idUser;
			IdCollocation = idCollocation;
			IsCorrect = isCorrect;
			When = when;
		}

		public CollocationAttempt(int id, int idUser, int idCollocation, bool isCorrect, DateTime when) :
			this(idUser, idCollocation, isCorrect, when)
		{
			Id = id;
		}

    public CollocationAttempt(CollocationAttemptVM viewModel)
    {
      Id = viewModel.Id;
      IdUser = viewModel.IdUser;
      IdCollocation = viewModel.IdCollocation;
      IsCorrect = viewModel.IsCorrect;
      When = viewModel.When;
    }
	}
}
