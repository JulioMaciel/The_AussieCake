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
                case Model.Voc:
                    return VocabularyAttempts;
                case Model.Essay:
                    return EssayAttempts;
                case Model.SumRetell:
                    return SumRetellAttempts;
                case Model.DescImg:
                    return DescImgAttempts;
                case Model.Spell:
                    return SpellingAttempts;
                default:
                    Errors.ThrowErrorMsg(ErrorType.InvalidModelType, type);
                    return new List<AttemptVM>();
            }
        }

        public static void Insert(AttemptVM att)
        {
            InsertAttempt(att);

            if (att.Type == Model.Voc)
            {
                var idquestion = Get(att.Type).Last().IdQuestion;
                UpdateQuestionFromLastAttempt(idquestion, att.Type);
            }
        }

        public static void Remove(AttemptVM att)
        {
            RemoveQuestionAttempt(att);

            if (att.Type == Model.Voc)
                UpdateQuestionFromLastAttempt(att.IdQuestion, att.Type);
        }

        public static void RemoveLast(Model type)
        {
            var last = Get(type).Last();
            RemoveQuestionAttempt(last);

            if (type == Model.Voc)
            {
                UpdateQuestionFromLastAttempt(last.IdQuestion, type);
            }
        }

        public static void LoadDB(Model type)
        {
            if (type == Model.Voc && VocabularyAttempts == null)
                GetVocAttemptsDB();
            else if (type == Model.Essay && EssayAttempts == null)
                GetEssayAttemptsDB();
            else if (type == Model.SumRetell && SumRetellAttempts == null)
                GetSumRetellAttemptsDB();
            else if (type == Model.DescImg && DescImgAttempts == null)
                GetDescImgAttemptsDB();
            else if (type == Model.Spell && SpellingAttempts == null)
                GetSpellAttemptsDB();
        }

        private static void UpdateQuestionFromLastAttempt(int idQuestion, Model type)
        {
            var quest = QuestControl.Get(type).First(c => c.Id == idQuestion);
            quest.LoadCrossData();
        }
    }
}
