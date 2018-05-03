﻿using AussieCake.Models;
using AussieCake.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Controllers
{
	public class CollocationController : SqLiteHelper
	{
		public static List<CollocationVM> Collocations { get; private set; }

		public static void Insert(CollocationVM collocation)
    {
      if (Collocations.Any(s => s.Component1 == collocation.Component1 && s.Component2 == collocation.Component2))
        return;

      var model = new Collocation(collocation);
      InsertCollocation(model);
      LoadCollocationsViewModel();
    }

    public static void Update(CollocationVM collocation)
    {
      var model = new Collocation(collocation);
      UpdateCollocation(model);
      var oldVM = Collocations.FirstOrDefault(x => x.Id == collocation.Id);
      oldVM = collocation;
    }

    public static void Remove(CollocationVM collocation)
    {
      var model = new Collocation(collocation);
      RemoveCollocation(model);
      Collocations.Remove(collocation);
    }

    public static CollocationVM GetCollocation(int id)
    {
      return Collocations.FirstOrDefault(x => x.Id == id);
		}

		public static void LoadCollocationsViewModel()
		{
			if (Collocations == null)
				Collocations = new List<CollocationVM>();

			foreach (var col in CollocationsDB)
			{
				var vm = new CollocationVM(col);
				if (!Collocations.Contains(vm))
					Collocations.Add(vm);
			}
		}

	}
}
