using AussieCake.Context;
using AussieCake.Question;
using AussieCake.Util;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Sentence
{
    public class QuestSenControl_Deprecated : SqLiteHelper
    {
        public static IEnumerable<QuestSenVM_Deprecated> Get(Model type)
        {
            LoadDB(type);

            switch (type)
            {
                case Model.Voc:
                    //return  Vocabularyentences;
                default:
                    Errors.ThrowErrorMsg(ErrorType.InvalidModelType, type);
                    return new List<QuestSenVM_Deprecated>();
            }
        }

        public static void Insert(QuestSenVM_Deprecated vm)
        {
            //InsertQuestionSentence(vm);
            var editedQuest = QuestControl.Get(vm.Type).First(x => x.Id == vm.IdQuest);
            editedQuest.LoadCrossData();
        }

        public static void Remove(QuestSenVM_Deprecated vm)
        {
            //RemoveQuestionSentence(vm);
            var editedQuest = QuestControl.Get(vm.Type).First(x => x.Id == vm.IdQuest);
            editedQuest.LoadCrossData();
        }

        public static void Remove(int qs_id, Model type)
        {
            var qs = Get(type).First(x => x.Id == qs_id);
            Remove(qs);
        }

        //public static void Update(QuestSenVM vm)
        //{
        //    UpdateQuestSentence(vm);
        //}

        public static void LoadDB(Model type)
        {
            //if (type == Model.Voc && Vocabularyentences == null)
            //    GetVocSentencesDB();
        }

        public static void LoadEveryQuestSenDB()
        {
            LoadDB(Model.Voc);
        }
    }
}
