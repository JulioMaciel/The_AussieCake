using AussieCake.Util;

namespace AussieCake.Question
{
    public class PronModel : QuestionModel
    {
        public string Phonemes { get; private set; }

        public PronModel(int id, string text, string phoneme, int importance, int isActive)
            : base(id, text, importance, isActive)
        {
            SetProperties(phoneme);
        }

        public PronModel(string text, string phoneme, int importance, int isActive)
            : base(text, importance, isActive)
        {
            SetProperties(phoneme);
        }

        private void SetProperties(string phoneme)
        {
            Phonemes = phoneme;
        }

        public PronVM ToVM()
        {
            var isActive_vm = IsActive.ToBool();

            if (IsReal)
                return new PronVM(Id, Text, Phonemes, (Importance)Importance, isActive_vm);
            else
                return new PronVM(Text, Phonemes, (Importance)Importance, isActive_vm);
        }
    }
}
