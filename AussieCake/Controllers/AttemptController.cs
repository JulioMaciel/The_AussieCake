using AussieCake.Models;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.Controllers
{
  public static class AttemptController
  {
    public static void Insert(CollocationAttemptVM collocation)
    {
      var model = new CollocationAttempt(collocation);
      SqLiteHelper.InsertCollocationAttempt(model);
      DBController.LoadAttemptsViewModel();
      DBController.LoadCollocationsViewModel();
    }
  }
}
