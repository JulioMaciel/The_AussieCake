using System.Collections.Generic;

namespace AussieCake.ViewModels.Base
{
	public interface IQuestion
  {
    int Id { get; }

    string PtBr { get; set; }
    List<int> SentencesId { get; set; }

    List<DateTry> Tries { get; }
    int Corrects { get; }
    int Incorrects { get; }
    double AverageScore { get; }
    double ChanceToAppear { get; }
  }
}
