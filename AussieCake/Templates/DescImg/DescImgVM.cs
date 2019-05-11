using AussieCake.Question;
using AussieCake.Util;

namespace AussieCake.Templates
{
    public class DescImgVM : QuestVM
    {
        public string Word { get; set; }

        public DescImgVM(int id, string word) : base(id, word, Importance.Any, true, Model.DescImg)
        {
        }
    }
}
