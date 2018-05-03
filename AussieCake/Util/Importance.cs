using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AussieCake.ViewModels;

namespace AussieCake.Util
{
  public enum Importance
  {
    PTE_IELTS_Official = 0,
    Reliable_resource = 1,
    My_own_relevant = 2,
    Random_resource = 3,
    Just_to_be = 4, // apenas para constar
  }

  public static class ScoreHelper
  {
    private static int GetScoreFromImportance(Importance imp)
    {
      switch (imp)
      {
        case Importance.PTE_IELTS_Official:
          return 10;
        case Importance.Reliable_resource:
          return 7;
        case Importance.My_own_relevant:
          return 5;
        case Importance.Random_resource:
          return 3;
        case Importance.Just_to_be:
          return 1;
      }
      return 0;
    }
    
    // avg (inverso -> 100p) + dias last try *2 (max é 15 -> 30p) + errou a última +10 + importance*2
    internal static double LoadChanceToAppear(CollocationVM col)
    {
      double result = 0;
      var lastTry = col.Tries.Max(x => x.When);
      var daysSinceLastTry = DateTime.Now.Subtract(lastTry).Days;
      var inverseAvgScore = col.AverageScore * -1;
      var add10IfLastWasWrong = col.Tries.LastOrDefault(x => x.When == lastTry).IsCorrect == false ? 10 : 0;
      result = inverseAvgScore + daysSinceLastTry * 2 + add10IfLastWasWrong + (int)col.Importance * 2;
      return result;
    }
  }
}
