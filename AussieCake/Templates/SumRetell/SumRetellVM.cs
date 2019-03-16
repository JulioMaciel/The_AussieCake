using AussieCake.Question;
using AussieCake.Util;

namespace AussieCake.Templates
{
    public class SumRetellVM : QuestVM
    {
        public string Word { get; set; }

        public SumRetellVM(int id, string word) : base(id, word, "", "", Importance.Any, true, Model.SumRetell)
        {
        }
    }
}
