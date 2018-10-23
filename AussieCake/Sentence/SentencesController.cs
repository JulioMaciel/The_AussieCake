using AussieCake.Context;
using AussieCake.Question;
using AussieCake.Util;
using AussieCake.Verb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AussieCake.Sentence
{
    public class SenControl : SqLiteHelper
    {
        #region Public Methods

        public static IEnumerable<SenVM> Get()
        {
            return Sentences;
        }

        public static bool Insert(SenVM sentence)
        {
            if (!IsSentenceValid(sentence.Text))
                return false;

            InsertSentence(sentence);

            return true;
        }

        private static bool IsSentenceValid(string sentence)
        {
            if (Errors.IsNullSmallOrBigger(sentence, 20, 200))
                return false;

            if (Sentences.Any(s => s.Text == sentence))
                return Errors.ThrowErrorMsg(ErrorType.AlreadyInserted, sentence);

            if (!sentence.EndsWith(".") && !sentence.EndsWith("?") && !sentence.EndsWith("!"))
                return Errors.ThrowErrorMsg(ErrorType.NoPunctuation, sentence);

            if (!Char.IsUpper(sentence[0]))
                return Errors.ThrowErrorMsg(ErrorType.InitialLowerCase, sentence);

            return true;
        }

        public static bool Update(SenVM sentence)
        {
            if (!IsSentenceValid(sentence.Text))
                return false;

            UpdateSentence(sentence);

            var oldSen = Sentences.FindIndex(x => x.Id == sentence.Id);
            Sentences.Insert(oldSen, sentence);

            return true;
        }

        public static void Remove(SenVM sentence)
        {
            RemoveSentence(sentence);
            Sentences.Remove(sentence);
        }

        public static bool DoesSentenceContainsCollection(ColVM col, SenVM sentence)
        {
            if (!DoesSentenceContainsCollection(col, sentence.Text))
                return false;

            // I know, it's redundant
            var comp1 = CheckSentenceContainsComponent(col.Component1, col.IsComp1Verb, sentence.Text);
            var comp2 = CheckSentenceContainsComponent(col.Component2, col.IsComp2Verb, sentence.Text);

            var position = new StringPositions();
            position.IdSentence = sentence.Id;
            position.Comp1Pos = comp1.Item2;
            position.Comp2Pos = comp2.Item2;
            col.Positions.Add(position);

            col.SentencesId.Add(sentence.Id);
            QuestControl.Update(col);

            return true;
        }

        public static bool DoesSentenceContainsCollection(ColVM col, string sentence)
        {
            // start checking the comp2 because it's here where is the biggest chance of returning false
            var comp2 = CheckSentenceContainsComponent(col.Component2, col.IsComp2Verb, sentence);
            if (!comp2.Item1)
                return false;

            var comp1 = CheckSentenceContainsComponent(col.Component1, col.IsComp1Verb, sentence);
            if (!comp1.Item1)
                return false;

            if (comp1.Item2 > comp2.Item2)
                return false;

            var link = CheckSentenceContainsPart(col.LinkWords, sentence);
            if (link == PartResult.PartNotFound)
                return false;

            var pref = CheckSentenceContainsPart(col.Prefixes, sentence);
            if (pref == PartResult.PartNotFound)
                return false;

            var suf = CheckSentenceContainsPart(col.Suffixes, sentence);
            if (suf == PartResult.PartNotFound)
                return false;

            return true;
        }

        public static void PopulateQuestions()
        {
            foreach (var sen in Sentences)
                sen.GetQuestions();
        }

        public async static Task SaveSentencesFromString(string source)
        {
            var sentences = FileHtmlControls.GetSentencesFromSource(source);
            sentences = sentences.Where(s => !Errors.IsNullSmallOrBigger(s) &&
                                             ((s.EndsWith(".") && !s.EndsWith("Dr.") && !s.EndsWith("Mr.") &&
                                             !s.EndsWith("Ms.")) || s.EndsWith("!") || s.EndsWith("?"))).ToList();

            var savedSentences = await GetCollocationsSentenceFromList(sentences);

            InsertSentencesFound(savedSentences);
        }

        #endregion

        #region Private Methods

        private async static Task<List<string>> GetCollocationsSentenceFromList(List<string> sentences)
        {
            var result = new List<string>();

            var tasks = QuestControl.Get(Model.Col).Select(async col =>
            {
                await Task.Run(() =>
                {
                    foreach (var sen in sentences)
                    {
                        if (DoesSentenceContainsCollection((ColVM)col, new SenVM(sen, false)) && !result.Contains(sen))
                        {
                            result.Add(sen);
                        }
                    }
                });
            });
            await Task.WhenAll(tasks);

            return result;
        }

        private static void InsertSentencesFound(List<string> sentencesFound)
        {
            foreach (var found in sentencesFound)
            {
                var vm = new SenVM(found, false);
                Insert(vm);
            }
        }

        private static (bool, int) CheckSentenceContainsComponent(string comp, bool isAVerb, string sentence)
        {
            var infinitivePosition = sentence.GetPosition(comp);
            if (infinitivePosition >= 0)
                return (true, infinitivePosition.Value);

            if (isAVerb)
            {
                var staticVerb = VerbsController.Verbs.First(v => v.Infinitive.Equals(comp, StringComparison.CurrentCultureIgnoreCase));

                if (staticVerb == null)
                    staticVerb = VerbsController.ConjugateUnknownVerb(comp);

                var position = sentence.GetPosition(staticVerb.Gerund);
                if (position >= 0)
                    return (true, position.Value);

                position = sentence.GetPosition(staticVerb.Past);
                if (position >= 0)
                    return (true, position.Value);

                position = sentence.GetPosition(staticVerb.PastParticiple);
                if (position >= 0)
                    return (true, position.Value);

                position = sentence.GetPosition(staticVerb.Person);
                if (position >= 0)
                    return (true, position.Value);
            }

            return (false, 0);
        }

        private static PartResult CheckSentenceContainsPart(List<string> parts, string sentence)
        {
            if (parts == null)
                return PartResult.PartsNotSent;

            if (!parts.Any())
                return PartResult.PartsNotSent;

            foreach (var item in parts)
            {
                // check if it is Be
                if (item.ContainsInsensitive("Be"))
                {
                    foreach (var toBe in VerbsController.VerbToBe)
                    {
                        if (sentence.ContainsInsensitive(item.ReplaceInsensitive("Be", toBe)))
                            return PartResult.PartFound;
                    }

                    return PartResult.PartNotFound;
                }

                if (sentence.ContainsInsensitive(item))
                    return PartResult.PartFound;
            }
            return PartResult.PartNotFound;
        }

        #endregion
    }

    internal enum PartResult
    {
        PartsNotSent,
        PartNotFound,
        PartFound,
    }
}
