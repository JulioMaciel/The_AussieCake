using AussieCake.Context;
using AussieCake.Question;
using AussieCake.Util;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AussieCake.Sentence
{
    public class SenControl_Deprecated : SqLiteHelper
    {
        #region Public Methods

        public static IEnumerable<SenVM_Deprecated> Get()
        {
            LoadDB();

            return null;// Sentences;
        }

        public static bool Insert(SenVM_Deprecated sentence, bool showErrorsbox)
        {
            if (!IsSentenceValid(sentence.Text, showErrorsbox))
                return false;

            //InsertSentence(sentence);

            return true;
        }

        private static bool IsSentenceValid(string sentence, bool showErrorsbox)
        {
            if (Errors.IsNullSmallerOrBigger(sentence, SenVM_Deprecated.MinSize, SenVM_Deprecated.MaxSize, showErrorsbox))
                return false;

            //if (Sentences.Any(s => s.Text == sentence))
            //    return showErrorsbox ? Errors.ThrowErrorMsg(ErrorType.AlreadyInserted, sentence) : false;

            if (!sentence.EndsWith(".") && !sentence.EndsWith("?") && !sentence.EndsWith("!"))
                return showErrorsbox ? Errors.ThrowErrorMsg(ErrorType.NoPunctuation, sentence) : false;

            if (!char.IsUpper(sentence[0]))
                return showErrorsbox ? Errors.ThrowErrorMsg(ErrorType.InitialLowerCase, sentence) : false;

            return true;
        }

        public static bool Update(SenVM_Deprecated sentence)
        {
            if (!IsSentenceValid(sentence.Text, true))
                return false;

            //UpdateSentence(sentence);

            //var oldSen = Sentences.FindIndex(x => x.Id == sentence.Id);
            //Sentences.Insert(oldSen, sentence);

            return true;
        }

        public static void Remove(SenVM_Deprecated sentence)
        {
            //RemoveSentence(sentence);

            var qs_from_sentence = QuestSenControl_Deprecated.Get(Model.Voc).Where(x => x.IdSen == sentence.Id).ToList();

            if (qs_from_sentence.Any())
            {
                foreach (var qs in qs_from_sentence)
                {
                    QuestSenControl_Deprecated.Remove(qs);
                    var changedVoc = QuestControl.Get(Model.Voc).Where(y => y.Id == qs.IdQuest).First();
                    changedVoc.LoadCrossData();
                }
            }
        }

        public static void LoadDB()
        {
            //if (Sentences == null)
            //    GetSentencesDB();
        }

        public static void PopulateQuestions()
        {
            //foreach (var sen in Sentences)
            //    sen.GetQuestions();
        }

        public static void LinkQuestsToSentences()
        {
            int updated_questions = 0;

            for (int i = 0; i < Get().Count(); i++)
            {
                var sen = Get().ElementAt(i);
                if (Get().Any(x => x.Text == sen.Text))
                    Remove(sen);
            }

            CheckVocs(ref updated_questions);

            if (updated_questions >= 1)
                MessageBox.Show(updated_questions + " questions were updated.");
        }

        private static void CheckVocs(ref int updated_questions)
        {
            var Vocs = QuestControl.Get(Model.Voc).ToList();
            foreach (VocVM Voc in Vocs)
            {
                foreach (var sen in Get())
                {
                    //if (AutoGetSentences_Deprecated.DoesSenContainsVoc(Voc, sen.Text))
                    //{
                    //    QuestSenControl_Deprecated.Insert(new QuestSenVM_Deprecated(Voc.Id, sen.Id, Voc.Type));
                    //    Voc.LoadCrossData();
                    //    Voc.Sentences_off.Add(sen);
                    //    QuestControl.Update(Voc);
                    //    updated_questions++;
                    //}
                }
            }
        }

        #endregion
    }
}
