using AussieCake.Util;

namespace AussieCake.Question
{
    public class SpellModel : QuestionModel
    {
        public SpellModel(int id, string text, int importance, int isActive)
            : base(id, text, importance, isActive)
        {
        }

        public SpellModel(string text, int importance, int isActive)
            : base(text, importance, isActive)
        {
        }

        public SpellVM ToVM()
        {
            var isActive_vm = IsActive.ToBool();

            if (IsReal)
                return new SpellVM(Id, Text, (Importance)Importance, isActive_vm);
            else
                return new SpellVM(Text, (Importance)Importance, isActive_vm);
        }
    }
}
