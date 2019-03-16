using AussieCake.Question;
using AussieCake.Util;

namespace AussieCake.Templates
{
    public class EssayVM : QuestVM
    {
        public string Word { get; set; }

        public EssayVM(int id, string word) : base(id, word, "", "", Importance.Any, true, Model.Essay)
        {
        }
    }
}
