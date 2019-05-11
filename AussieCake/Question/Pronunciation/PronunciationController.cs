using AussieCake.Util;
using System.Linq;

namespace AussieCake.Question
{
    public class PronControl : QuestControl
    {
        public static bool Insert(PronVM Pronunciation)
        {
            if (Pronunciations.Any(s => s.Text == Pronunciation.Text))
            {
                var old_Pron = Pronunciations.First(s => s.Text == Pronunciation.Text);
                var old_score = ScoreHelper.GetScoreFromImportance(old_Pron.Importance);
                var new_score = ScoreHelper.GetScoreFromImportance(Pronunciation.Importance);

                if (new_score > old_score)
                {
                    var to_update = new PronVM(old_Pron.Id, old_Pron.Text, old_Pron.Phonemes, Pronunciation.Importance, old_Pron.IsActive);
                    return Update(to_update);
                }
                else
                    return Errors.ThrowErrorMsg(ErrorType.AlreadyInserted, Pronunciation.Text);
            }

            if (!InsertPronunciation(Pronunciation.ToModel()))
                return false;

            Pronunciation.LoadCrossData();

            return true;
        }

        public static bool Update(PronVM Pronunciation)
        {
            if (!UpdatePronunciation(Pronunciation.ToModel()))
                return false;

            var oldVM = Pronunciations.FindIndex(x => x.Id == Pronunciation.Id);
            Pronunciations.Insert(oldVM, Pronunciation);

            return true;
        }

        public static bool Remove(PronVM Pronunciation)
        {
            if (!RemovePronunciation(Pronunciation))
                return false;

            return true;
        }
    }
}
