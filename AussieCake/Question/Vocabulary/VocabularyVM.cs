using AussieCake.Util;

namespace AussieCake.Question
{
    public class VocVM : QuestVM
    {
        public string Answer { get; private set; }
        public string PtBr { get; protected set; }
        public string Definition { get; protected set; }

        public VocVM(int id, string text, string answer, string definition, string ptBr,
                        Importance importance, bool isActive)
            : base(id, text, importance, isActive, Model.Voc)
        {
            SetProperties(answer);
        }

        public VocVM(string text, string answer, string definition, string ptBr,
                        Importance importance, bool isActive)
            : base(text, importance, isActive, Model.Voc)
        {
            SetProperties(answer);
        }

        private void SetProperties(string answer)
        {
            Answer = answer;
        }

        public VocModel ToModel()
        {
            var isActive_raw = IsActive.ToInt();

            if (IsReal)
                return new VocModel(Id, Text, Answer, PtBr, Definition, (int)Importance, isActive_raw);
            else
                return new VocModel(Text, Answer, PtBr, Definition, (int)Importance, isActive_raw);
        }

        public override void LoadCrossData()
        {
            base.LoadCrossData();
        }

    }
}
