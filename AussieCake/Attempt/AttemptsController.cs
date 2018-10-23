using AussieCake.Context;
using AussieCake.Question;
using AussieCake.Util;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Attempt
{
    public class AttemptsControl : SqLiteHelper
    {
        public static IEnumerable<IAttempt> Get(Model type)
        {
            switch (type)
            {
                case Model.Col:
                    return CollocationAttempts;
                default:
                    Errors.ThrowErrorMsg(ErrorType.InvalidModelType, type);
                    return new List<IAttempt>();
            }
        }

        public static void Insert(IAttempt att)
        {
            InsertQuestionAttempt(att);
            UpdateQuestionFromLastAttempt();
        }

        public static void Remove(IAttempt att)
        {
            RemoveQuestionAttempt(att);
            UpdateQuestionFromLastAttempt();
        }

        private static void UpdateQuestionFromLastAttempt()
        {
            var attemptAdded = CollocationAttempts.Last();

            var col = QuestControl.Get(attemptAdded.Type).First(c => c.Id == attemptAdded.IdQuestion);
            col.LoadCrossData();
        }
    }
}
