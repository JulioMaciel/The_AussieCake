using AussieCake.Sentence;
using AussieCake.Util;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Question
{
    public class QuestControl : ColControl
    {
        public static IEnumerable<IQuestion> Get(Model type)
        {
            switch (type)
            {
                case Model.Col:
                    return Collocations.Cast<IQuestion>();
                default:
                    Errors.ThrowErrorMsg(ErrorType.InvalidModelType, type);
                    return new List<IQuestion>();
            }
        }

        public static bool Insert(IQuestion quest)
        {
            if (quest is ColVM col)
                return ColControl.Insert(col);

            return false;
        }

        public static bool Update(IQuestion quest)
        {
            if (quest is ColVM col)
                return ColControl.Update(col);

            return false;
        }

        public static bool Update(IQuestion quest, string newSen)
        {
            if (SenControl.Insert(new SenVM(newSen, true)))
            {
                quest.AddLastSentence();

                return Update(quest);
            }

            return false;
        }

        public static void Remove(IQuestion quest)
        {
            if (quest is ColVM col)
                ColControl.Remove(col);

            quest.RemoveAllAttempts();
        }

        public static void LoadCrossData()
        {
            foreach (var quest in Get(Model.Col))
                quest.LoadCrossData();

            LoadRealChances();
        }

        private static void LoadRealChances()
        {
            var summed_chances = Get(Model.Col).Sum(x => x.Chance);

            foreach (var quest in Get(Model.Col))
                quest.Chance_real = (quest.Chance * 100) / summed_chances;
        }


    }
}
