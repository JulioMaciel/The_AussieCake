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
            var idquestion = Get(att.Type).Last().IdQuestion;
            UpdateQuestionFromLastAttempt(idquestion, att.Type);
        }

        public static void Remove(AttemptVM att)
        {
            RemoveQuestionAttempt(att);
            UpdateQuestionFromLastAttempt(att.IdQuestion, att.Type);
        }

        public static void RemoveLast(Model type)
        {
            var last = Get(type).Last();
            RemoveQuestionAttempt(last);
            UpdateQuestionFromLastAttempt(last.IdQuestion, type);
        }

        public static void LoadDB(Model type)
        {
            if (type == Model.Col && CollocationAttempts == null)
                GetColAttemptsDB();
        }

        private static void UpdateQuestionFromLastAttempt(int idQuestion, Model type)
        {
            var col = QuestControl.Get(type).First(c => c.Id == idQuestion);
            col.LoadCrossData();
        }
    }
}
