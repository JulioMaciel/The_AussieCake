using AussieCake.Question;
using AussieCake.Util;
using AussieCake.Verb;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AussieCake.Sentence
{
    public static class AutoGetSentences
    {
        public async static Task<List<string>> GetSentencesFromString(string source)
        {
            var sentences = GetSentencesFromSource(source);
            sentences = sentences.Where(s => !Errors.IsNullSmallerOrBigger(s, SenVM.MinSize, SenVM.MaxSize, false) &&
                                              DoesStartEndProperly(s)).ToList();

            var found = new List<string>();
            found.AddRange(await GetColSentenceFromList(sentences));

            return found;
        }

        private static bool DoesStartEndProperly(string s)
        {
            return ((s.EndsWith(".") && !s.EndsWith("Dr.") && !s.EndsWith("Mr.") && !s.EndsWith("Ms."))
                                                || s.EndsWith("!") || s.EndsWith("?"));
        }

        private static List<string> GetSentencesFromSource(string source)
        {
            MatchCollection matchList = Regex.Matches(source, @"[A-Z]+(\w+\,*\;*[ ]{0,1}[\.\?\!]*)+");
            return matchList.Cast<Match>().Select(match => match.Value).ToList();
        }

        #region Collocations

        private async static Task<List<string>> GetColSentenceFromList(List<string> sentences)
        {
            var result = new List<string>();

            var tasks = QuestControl.Get(Model.Col).Select(async col =>
            {
                await Task.Run(() =>
                {
                    foreach (var sen in sentences)
                    {
                        if (DoesSenContainsCol((ColVM)col, sen) && !result.Contains(sen))
                            result.Add(sen);
                    }
                });
            });
            await Task.WhenAll(tasks);

            return result;
        }

        public static bool DoesSenContainsCol(ColVM col, string sen)
        {
            // start checking the comp2 because it's here where is the biggest chance of returning false
            if (!DoesSenContainsComp(col.Component2, col.IsComp2Verb, sen))
                return false;

            if (!DoesSenContainsComp(col.Component1, col.IsComp1Verb, sen))
                return false;

            if (sen.IndexFrom(col.Component1) > sen.IndexFrom(col.Component2))
                return false;

            if (sen.IndexFrom(col.Component2) - sen.IndexFrom(col.Component1) < 35)
                return false;

            var link = DoesSenContainsPart(col.LinkWords, sen);
            if (link == PartResult.PartNotFound)
                return false;

            var pref = DoesSenContainsPart(col.Prefixes, sen);
            if (pref == PartResult.PartNotFound)
                return false;

            var suf = DoesSenContainsPart(col.Suffixes, sen);
            if (suf == PartResult.PartNotFound)
                return false;

            return true;
        }

        private static bool DoesSenContainsComp(string comp, bool isVerb, string sentence)
        {
            var compatible = GetCompatibleWord(comp, isVerb, sentence);

            if (!compatible.IsEmpty())
                return true;

            return false;
        }

        public static string GetCompatibleWord(string comp, bool isVerb, string sentence)
        {
            if (sentence.ContainsInsensitive(comp))
                return comp;

            if (isVerb)
            {
                VerbModel staticVerb = new VerbModel();

                if (VerbsController.Get().Any(v => v.Infinitive.EqualsNoCase(comp)))
                    staticVerb = VerbsController.Get().First(v => v.Infinitive.EqualsNoCase(comp));
                else
                    staticVerb = VerbsController.ConjugateUnknownVerb(comp);

                if (sentence.ContainsInsensitive(staticVerb.Gerund))
                    return staticVerb.Gerund;
                else if (sentence.ContainsInsensitive(staticVerb.Past))
                    return staticVerb.Past;
                else if (sentence.ContainsInsensitive(staticVerb.PastParticiple))
                    return staticVerb.PastParticiple;
                else if (sentence.ContainsInsensitive(staticVerb.Person))
                    return staticVerb.Person;
            }
            else
            {
                var sv = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentCulture);

                if (sv.IsSingular(comp))
                {
                    var plural = sv.Pluralize(comp);
                    if (sentence.ContainsInsensitive(plural))
                        return plural;
                }
            }

            return string.Empty;
        }

        #endregion

        private static PartResult DoesSenContainsPart(List<string> parts, string sentence)
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
    }

    internal enum PartResult
    {
        PartsNotSent,
        PartNotFound,
        PartFound,
    }
}
