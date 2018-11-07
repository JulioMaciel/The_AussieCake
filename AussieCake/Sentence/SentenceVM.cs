using AussieCake.Question;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Sentence
{
    public class SenVM
    {
        public int Id { get; private set; }

        public string Text { get; set; }
        public string PtBr { get; set; }

        public List<SenQuest> Questions { get; set; }

        public static int MaxSize = 120;
        public static int MinSize = 40;

        public SenVM(int id)
        {
            Id = id;
        }

        public SenVM(string text)
        {
            Text = text;
        }

        public SenVM(string text, string ptBr) : this(text)
        {
            PtBr = ptBr;
        }

        public SenVM(int id, string text, string ptBr) : this(text, ptBr)
        {
            Id = id;

            Questions = new List<SenQuest>();
        }

        public void GetQuestions()
        {
            foreach (var qs in QuestSenControl.Get(Model.Col))
            {
                if (qs.IdSen == Id)
                {
                    var quest = QuestControl.Get(Model.Col).First(x => x.Id == qs.IdQuest);
                    Questions.Add(new SenQuest(quest, qs.IsActive, qs.Id));
                }
            }

            // and so on [types]
        }

        public class SenQuest
        {
            public IQuest Quest { get; set; }
            public bool IsActive { get; set; }
            public int QS_id { get; set; }

            public SenQuest(IQuest quest, bool isActive, int qs_id)
            {
                Quest = quest;
                IsActive = isActive;
                QS_id = qs_id;
            }
        }
    }
}
