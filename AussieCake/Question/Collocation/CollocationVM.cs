using AussieCake.Util;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Question
{
    public class ColVM : QuestVM
    {
        public string Answer { get; private set; }

        public ColVM(int id, string text, string answer, string definition, string ptBr,
                        Importance importance, bool isActive)
            : base(id, text, definition, ptBr, importance, isActive, Model.Col)
        {
            SetProperties(answer);
        }

        public ColVM(string text, string answer, string definition, string ptBr, 
                        Importance importance, bool isActive)
            : base(text, definition, ptBr, importance, isActive, Model.Col)
        {
            SetProperties(answer);
        }

        private void SetProperties(string answer)
        {
            Answer = answer;
        }

        public ColModel ToModel()
        {
            var isActive_raw = IsActive.ToInt();

            if (IsReal)
                return new ColModel(Id, Text, Answer, PtBr, Definition, (int)Importance, isActive_raw);
            else
                return new ColModel(Text, Answer, PtBr, Definition, (int)Importance, isActive_raw);
        }

        public override void LoadCrossData()
        {
            base.LoadCrossData();
        }

        public override string ToLudwigUrl()
        {
            var result = "https://ludwig.guru/s/";

            var words = Text.Split(' ');

            result += string.Join("+", words);

            return result;
        }
    }
}
