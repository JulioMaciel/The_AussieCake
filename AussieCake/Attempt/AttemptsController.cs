using AussieCake.Context;
using AussieCake.Question;
using AussieCake.Util;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Attempt
{
    public class AttemptsControl : SqLiteHelper
    {
        public static IEnumerable<AttemptVM> Get(Model type)
        {
            LoadDB(type);

            switch (type)
            {
                case Model.Col:
                    return CollocationAttempts;
                default:
                    Errors.ThrowErrorMsg(ErrorType.InvalidModelType, type);
                    return new List<AttemptVM>();
            }
        }

        public static void Insert(AttemptVM att)
        {
            InsertQuestionAttempt(att);
            UpdateQuestionFromLastAttempt();
        }

        public static void Remove(AttemptVM att)
        {
            RemoveQuestionAttempt(att);
            UpdateQuestionFromLastAttempt();
        }

        public static void RemoveLast(Model type)
        {
            RemoveQuestionAttempt(Get(type).Last());
            UpdateQuestionFromLastAttempt();
        }

        public static void LoadDB(Model type)
        {
            if (type == Model.Col && CollocationAttempts == null)
                GetColAttemptsDB();
        }

        private static void UpdateQuestionFromLastAttempt()
        {
            var attemptAdded = CollocationAttempts.Last();

            var col = QuestControl.Get(attemptAdded.Type).First(c => c.Id == attemptAdded.IdQuestion);
            col.LoadCrossData();
        }
    }
}
