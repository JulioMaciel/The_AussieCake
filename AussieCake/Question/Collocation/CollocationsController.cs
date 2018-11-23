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
            {
                var old_col = Collocations.First(s => s.Component1 == collocation.Component1 && s.Component2 == collocation.Component2);
                var old_score = ScoreHelper.GetScoreFromImportance(old_col.Importance);
                var new_score = ScoreHelper.GetScoreFromImportance(collocation.Importance);

                if (new_score > old_score)
                {
                    var to_update = new ColVM(old_col.Id, old_col.Prefixes, old_col.Component1, old_col.IsComp1Verb, old_col.LinkWords, 
                                              old_col.Component2, old_col.IsComp2Verb, old_col.Suffixes, old_col.Definition, 
                                              old_col.PtBr, collocation.Importance, old_col.IsActive);
                    return Update(to_update);
                }
                else
                return Errors.ThrowErrorMsg(ErrorType.AlreadyInserted, collocation.Component1 + " or " + collocation.Component2);
            }

            if (!ValidCompContentAndSizeValid(collocation.Component1) || !ValidCompContentAndSizeValid(collocation.Component2))
                return false;

            collocation.Prefixes = RemoveUselessPart(collocation.Prefixes);
            collocation.LinkWords = RemoveUselessPart(collocation.LinkWords);
            collocation.Suffixes = RemoveUselessPart(collocation.Suffixes);

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

        private static List<string> RemoveUselessPart(List<string> parts)
        {
            if (parts.Any(x => x == "a"))
                parts.Remove(parts.First(y => y == "a"));

            if (parts.Any(x => x == "an"))
                parts.Remove(parts.First(y => y == "an"));

            if (parts.Any(x => x == "the"))
                parts.Remove(parts.First(y => y == "the"));

            return parts;
        }
    }
}
