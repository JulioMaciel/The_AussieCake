using AussieCake.Sentence;
using AussieCake.Util;
using AussieCake.Util.WPF;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AussieCake.Question
{
    public class QuestControl : ColControl
    {
        public static IEnumerable<IQuest> Get(Model type)
        {
            LoadDB(type);

            switch (type)
            {
                case Model.Col:
                    return Collocations.Cast<IQuest>();
                default:
                    Errors.ThrowErrorMsg(ErrorType.InvalidModelType, type);
                    return new List<IQuest>();
            }
        }

        public static bool Insert(IQuest quest)
        {
            if (quest is ColVM col)
                return ColControl.Insert(col);

            return false;
        }

        public static bool Update(IQuest quest)
        {
            if (quest is ColVM col)
                return ColControl.Update(col);

            return false;
        }

        public static bool Update(IQuest quest, string newSen)
        {
            if (SenControl.Insert(new SenVM(newSen), true))
            {
                var added_sen = SenControl.Get().Last();
                QuestSenControl.Insert(new QuestSenVM(quest.Id, added_sen.Id, true, quest.Type));

                quest.AddLastSentence();

                return Update(quest);
            }

            return false;
        }

        public static void Remove(IQuest quest)
        {
            if (quest is ColVM col)
                ColControl.Remove(col);

            quest.RemoveAllAttempts();
        }

        public static void LoadCrossData(Model type)
        {
            if (Get(type).First().Tries != null)
                return;

            foreach (var quest in Get(type))
                quest.LoadCrossData();

            LoadRealChances(type);
        }

        public static void LoadDB(Model type)
        {
            if (type == Model.Col && Collocations == null)
                GetCollocationsDB();
        }

        public static void LoadEveryQuestionDB()
        {
            LoadDB(Model.Col);
        }

        private static void LoadRealChances(Model type)
        {
            var summed_chances = Get(type).Sum(x => x.Chance);

            foreach (var quest in Get(type))
                quest.Chance_real = (quest.Chance * 100) / summed_chances;
        }

        public static IQuest GetRandomAvailableQuestion(Model type, List<int> actual_chosen)
        {
            double actual_index = 0;

            var quests = Get(type).Where(x => x.IsActive &&
                                              x.Sentences.Where(s => s.IsActive).Any() &&
                                              !actual_chosen.Contains(x.Id));

            foreach (var quest in quests)
            {
                actual_index += quest.Chance;
                quest.Index_show = actual_index;
            }

            var totalChances =  quests.Select(x => x.Chance).Sum();
            int pickBasedOnChance = UtilWPF.RandomNumber(0, (int)totalChances);

            IQuest selected = quests.First();

            double cumulative = 0.0;
            for (int i = 0; i < quests.Count(); i++)
            {
                cumulative += quests.ElementAt(i).Chance;
                if (pickBasedOnChance < cumulative)
                {
                    selected = quests.ElementAt(i);
                    break;
                }
            }

            return selected;
        }


    }
}
