using AussieCake.Util;

namespace AussieCake.Question
{
    public class ColModel : QuestionModel
    {
        public string Answer { get; private set; }

        public ColModel(int id, string text, string answer, string ptBr, string definition, int importance, int isActive)
            : base(id, text, ptBr, definition, importance, isActive)
        {
            SetProperties(answer);
        }

        public ColModel(string text, string answer, string ptBr, string definition, int importance, int isActive)
            : base(text, ptBr, definition, importance, isActive)
        {
            SetProperties(answer);
        }

        private void SetProperties(string answer)
        {
            Answer = answer;
        }

        public ColVM ToVM()
        {
            var def_vm = Definition.EmptyIfNull();
            var ptBr_vm = PtBr.EmptyIfNull();
            var isActive_vm = IsActive.ToBool();

            if (IsReal)
                return new ColVM(Id, Text, Answer, def_vm, ptBr_vm, (Importance)Importance, isActive_vm);
            else
                return new ColVM(Text, Answer, def_vm, ptBr_vm, (Importance)Importance, isActive_vm);
        }
    }
}
