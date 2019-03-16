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
            if (Collocations.Any(s => s.Text == collocation.Text))
            {
                var old_col = Collocations.First(s => s.Text == collocation.Text);
                var old_score = ScoreHelper.GetScoreFromImportance(old_col.Importance);
                var new_score = ScoreHelper.GetScoreFromImportance(collocation.Importance);

                if (new_score > old_score)
                {
                    var to_update = new ColVM(old_col.Id, old_col.Text, old_col.Answer, old_col.Definition, 
                                              old_col.PtBr, collocation.Importance, old_col.IsActive);
                    return Update(to_update);
                }
                else
                return Errors.ThrowErrorMsg(ErrorType.AlreadyInserted, collocation.Text);
            }

            if (!ValidWordsAndAnswerSize(collocation.Text, collocation.Answer))
                return false;

            if (!InsertCollocation(collocation.ToModel()))
                return false;

            collocation.LoadCrossData();

            return true;
		}

        protected static bool Update(ColVM collocation)
		{
            if (!ValidWordsAndAnswerSize(collocation.Text, collocation.Answer))
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

        private static bool ValidWordsAndAnswerSize(string words, string answer)
        {
            if (!words.Contains(answer))
                return Errors.ThrowErrorMsg(ErrorType.AnswerNotFound, answer);

            if (answer.Length < 3 && !answer.EqualsNoCase("be"))
                return Errors.ThrowErrorMsg(ErrorType.TooSmall, "Item wasn't saved. '" + answer + "' is not valid.");

            if (words.Length < 6)
                return Errors.ThrowErrorMsg(ErrorType.TooSmall, "Item wasn't saved. '" + words + "' is not valid.");

            if (!words.Contains(' '))
                return Errors.ThrowErrorMsg(ErrorType.InvalidCharacters, "Item wasn't saved. There's just one word.");

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
