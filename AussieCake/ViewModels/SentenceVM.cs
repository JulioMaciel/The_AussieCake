using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.ViewModels.Base;
using System.Collections.Generic;

namespace AussieCake.ViewModels
{
    public class SentenceVM
    {
        public int Id { get; private set; }

        public string Text { get; set; }
        public string PtBr { get; set; }
        public bool IsActive { get; set; }

        public List<IQuestion> Questions { get; set; }

        public SentenceVM(string text, bool isActive)
        {
            Text = text;
            IsActive = isActive;

            PtBr = string.Empty;
        }

        public SentenceVM(string text, string ptBr, bool isActive) : this(text, isActive)
        {
            PtBr = ptBr;
        }

        public SentenceVM(Sentence sentence)
        {
            Id = sentence.Id;
            Text = sentence.Text;
            PtBr = sentence.PtBr == null ? string.Empty : sentence.PtBr;
            IsActive = sentence.IsActive;

            Questions = new List<IQuestion>();
        }

        public void GetQuestions()
        {
            foreach (var col in CollocationController.Collocations)
            {
                if (col.SentencesId.Contains(Id))
                    Questions.Add(col);
            }
        }
    }
}
