using AussieCake.Models;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.Controllers
{
  public static class QuestionController
  {
    public static void Insert(CollocationVM collocation)
    {
      if (DBController.Collocations.Any(s => s.Component1 == collocation.Component1 && s.Component2 == collocation.Component2))
        return;

      var model = new Collocation(collocation);
      SqLiteHelper.InsertCollocation(model);
      DBController.LoadCollocationsViewModel();
    }

    public static void Update(CollocationVM collocation)
    {
      var model = new Collocation(collocation);
      SqLiteHelper.UpdateCollocation(model);
      var oldVM = DBController.Collocations.FirstOrDefault(x => x.Id == collocation.Id);
      oldVM = collocation;
    }

    public static void Remove(CollocationVM collocation)
    {
      var model = new Collocation(collocation);
      SqLiteHelper.RemoveCollocation(model);
      DBController.Collocations.Remove(collocation);
    }

    public static CollocationVM GetCollocation(int id)
    {
      return DBController.Collocations.FirstOrDefault(x => x.Id == id);
    }
  }
}
