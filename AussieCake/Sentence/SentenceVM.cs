using AussieCake.Question;
using System.Collections.Generic;

namespace AussieCake.Sentence
{
    public class SenVM
    {
        public int Id { get; private set; }

        public string Text { get; set; }
        public string PtBr { get; set; }
        public bool IsActive { get; set; }

        public List<IQuestion> Questions { get; set; }

        public SenVM(int id)
        {
            Id = id;
        }

        public SenVM(string text, bool isActive)
        {
            Text = text;
            IsActive = isActive;
        }

        public SenVM(string text, string ptBr, bool isActive) : this(text, isActive)
        {
            PtBr = ptBr;
        }

        public SenVM(int id, string text, string ptBr, bool isActive) : this(text, ptBr, isActive)
        {
            Id = id;

            if (PtBr == null)
                PtBr = string.Empty;
        }

        public void GetQuestions()
        {
            Questions = new List<IQuestion>();

            foreach (var col in QuestControl.Get(Model.Col))
            {
                if (col.SentencesId.Contains(Id))
                    Questions.Add(col);
            }
        }
    }
}
