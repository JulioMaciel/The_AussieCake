using AussieCake.Context;
using AussieCake.Question;
using AussieCake.Util;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Sentence
{
    public class QuestSenControl : SqLiteHelper
    {
        public static IEnumerable<QuestSenVM> Get(Model type)
        {
            LoadDB(type);

            switch (type)
            {
                case Model.Col:
                    return  CollocationSentences;
                default:
                    Errors.ThrowErrorMsg(ErrorType.InvalidModelType, type);
                    return new List<QuestSenVM>();
            }
        }

        public static void Insert(QuestSenVM vm)
        {
            InsertQuestionSentence(vm);
        }

        public static void Remove(QuestSenVM vm)
        {
            RemoveQuestionSentence(vm);
        }

        public static void Remove(int qs_id, Model type)
        {
            var qs = Get(type).First(x => x.Id == qs_id);
            Remove(qs);
        }

        public static void Update(QuestSenVM vm)
        {
            UpdateQuestSentence(vm);
        }

        public static void LoadDB(Model type)
        {
            if (type == Model.Col && CollocationSentences == null)
                GetColSentencesDB();
        }

        public static void LoadEveryQuestSenDB()
        {
            LoadDB(Model.Col);
        }
    }
}
