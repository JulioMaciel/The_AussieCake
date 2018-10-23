using AussieCake.Attempt;
using AussieCake.Context;
using AussieCake.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Question
{
    public class ColControl : SqLiteHelper
    {
        protected static bool Insert(ColVM collocation)
		{
            if (Collocations.Any(s => s.Component1 == collocation.Component1 && s.Component2 == collocation.Component2))
                return Errors.ThrowErrorMsg(ErrorType.AlreadyInserted, collocation.Component1 + " or " + collocation.Component2);

            if (!ValidCompContentAndSizeValid(collocation.Component1) || !ValidCompContentAndSizeValid(collocation.Component2))
                return false;
            
			if (!InsertCollocation(collocation.ToModel()))
                return false;

            collocation.LoadCrossData();

            return true;
		}

        protected static bool Update(ColVM collocation)
		{
            if (!ValidCompContentAndSizeValid(collocation.Component1) || !ValidCompContentAndSizeValid(collocation.Component2))
                return false;

            if (!UpdateCollocation(collocation.ToModel()))
                return false;


			var oldVM = Collocations.FindIndex(x => x.Id == collocation.Id);
            Collocations.Insert(oldVM, collocation);

            return true;
		}

        protected static bool Remove(ColVM collocation)
		{
            if (!RemoveCollocation(collocation))
                return false;

            return true;
        }

        private static bool ValidCompContentAndSizeValid(string comp)
        {
            if (comp.IsEmpty() || (comp.Length < 3 && !comp.EqualsNoCase("be")))
                return Errors.ThrowErrorMsg(ErrorType.TooSmall, "Item wasn't saved. '" + comp + "' is not valid.");

            return true;
        }
    }
}
