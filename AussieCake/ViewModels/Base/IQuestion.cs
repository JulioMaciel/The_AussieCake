using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
