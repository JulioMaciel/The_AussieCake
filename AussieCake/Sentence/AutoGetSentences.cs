using AussieCake.Question;
using AussieCake.Util;
using AussieCake.Verb;
using System;
using System.Collections.Generic;
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
            Footer.Log("The auto insert sentences has just started. It may take a time to finish.");

            var sentences = new List<string>();

            Task tasks = Task.Run(() => sentences = GetSentencesFromSource(source));
            await Task.WhenAll(tasks);

            tasks = Task.Run(() => sentences = sentences.Where(s => !Errors.IsNullSmallerOrBigger(s, SenVM.MinSize, SenVM.MaxSize, false) &&
                                              DoesStartEndProperly(s)).ToList());
            await Task.WhenAll(tasks);

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

            var watcher = new Stopwatch();
            watcher.Start();

            Task tasks = Task.Run(() => Parallel.ForEach(QuestControl.Get(Model.Col), col =>
            {
                foreach (var sen in sentences)
                {
                    if (DoesSenContainsCol((ColVM)col, sen) && !result.Contains(sen))
                        result.Add(sen);

                    Footer.Log("Analysing " + sentences.Count + " sentences for collocation id " + col.Id + " of " +
                        QuestControl.Get(Model.Col).Count() + ". " + result.Count + " sentences added in " +
                        Math.Round(watcher.Elapsed.TotalMinutes, 2) + " minutes.");
                }
            }));
            await Task.WhenAll(tasks);

            return result;
        }

        public static bool DoesSenContainsCol(ColVM col, string sen)
        {
            // start checking the comp2 because it's here where is the biggest chance of returning false
            var indexesComp2 = GetIndexOfCompatibleWord(col.Component2, col.IsComp2Verb, sen);
            if (!indexesComp2.Any() || indexesComp2.Count > 1)
                return false;

            var indexComp2 = indexesComp2.First();

            var indexesComp1 = GetIndexOfCompatibleWord(col.Component1, col.IsComp1Verb, sen);
            if (!indexesComp1.Any() || indexesComp1.Count > 1)
                return false;

            var indexComp1 = indexesComp1.First();

            if (indexComp2 < indexComp1)
                return false;

            if (indexComp2 - indexComp1 > 35)
                return false;

            var hasLink = col.LinkWords != null && col.LinkWords.Any();
            var indexesLink = GetIndexOfPart(col.LinkWords, sen);
            if (hasLink)
            {
                if (!indexesLink.Any())
                    return false;

                if (!indexesLink.Any(x => x > indexComp1))
                    return false;

                if (!indexesLink.Any(y => y < indexComp2))
                    return false;

                if (indexesLink.GetMinimumDistance(indexesComp1) > 35)
                    return false;

                if (indexesComp2.GetMinimumDistance(indexesLink) > 35)
                    return false;
            }

            var hasPref = col.Prefixes != null && col.Prefixes.Any();
            var indexesPref = GetIndexOfPart(col.Prefixes, sen);
            if (hasPref)
            {
                if (!indexesPref.Any())
                    return false;

                if (!indexesPref.Any(x => x < indexComp1))
                    return false;

                if (hasLink && !indexesPref.Any(y => y < indexesLink.Max()))
                    return false;

                if (indexesComp1.GetMinimumDistance(indexesPref) > 35)
                    return false;
            }

            var hasSuff = col.Suffixes != null && col.Suffixes.Any();
            var indexesSuff = GetIndexOfPart(col.Suffixes, sen);
            if (hasSuff)
            {
                if (!indexesSuff.Any())
                    return false;

                if (!indexesSuff.Any(x => x > indexComp2))
                    return false;

                if (hasLink && !indexesSuff.Any(y => y > indexesLink.Max()))
                    return false;

                if (indexesSuff.GetMinimumDistance(indexesComp2) > 35)
                    return false;
            }

            return true;
        }

        private static bool DoesSenContainsComp(string comp, bool isVerb, string sentence)
        {
            var compatible = GetCompatibleWord(comp, isVerb, sentence);

            if (!compatible.IsEmpty())
                return true;

            return false;
        }

        public static string GetCompatibleWord(string comp, bool isVerb, string sen)
        {
            if (sen.ContainsInsensitive(comp))
                return comp;

            if (isVerb)
            {
                VerbModel staticVerb = new VerbModel();

                if (VerbsController.Get().Any(v => v.Infinitive.EqualsNoCase(comp)))
                    staticVerb = VerbsController.Get().First(v => v.Infinitive.EqualsNoCase(comp));
                else
                    staticVerb = VerbsController.ConjugateUnknownVerb(comp);

                if (sen.ContainsInsensitive(staticVerb.Gerund))
                    return staticVerb.Gerund;
                else if (sen.ContainsInsensitive(staticVerb.Past))
                    return staticVerb.Past;
                else if (sen.ContainsInsensitive(staticVerb.PastParticiple))
                    return staticVerb.PastParticiple;
                else if (sen.ContainsInsensitive(staticVerb.Person))
                    return staticVerb.Person;
            }
            else
            {
                if (FileHtmlControls.Plural_service.IsSingular(comp))
                {
                    var plural = FileHtmlControls.Plural_service.Pluralize(comp);
                    if (sen.ContainsInsensitive(plural))
                        return plural;
                }
            }

            return string.Empty;
        }

        public static List<int> GetIndexOfCompatibleWord(string comp, bool isVerb, string sen)
        {
            var result = new List<int>();

            if (sen.ContainsInsensitive(comp))
            {
                var indexes = sen.IndexesFrom(comp);
                AddUniqueValues(result, indexes);
            }

            if (isVerb)
            {
                VerbModel staticVerb = new VerbModel();

                if (VerbsController.Get().Any(v => v.Infinitive.EqualsNoCase(comp)))
                    staticVerb = VerbsController.Get().First(v => v.Infinitive.EqualsNoCase(comp));
                else
                    staticVerb = VerbsController.ConjugateUnknownVerb(comp);

                if (sen.ContainsInsensitive(staticVerb.Gerund))
                {
                    var indexes = sen.IndexesFrom(staticVerb.Gerund);
                    AddUniqueValues(result, indexes);
                }

                if (sen.ContainsInsensitive(staticVerb.Past))
                {
                    var indexes = sen.IndexesFrom(staticVerb.Past);
                    AddUniqueValues(result, indexes);
                }

                if (sen.ContainsInsensitive(staticVerb.PastParticiple))
                {
                    var indexes = sen.IndexesFrom(staticVerb.PastParticiple);
                    AddUniqueValues(result, indexes);
                }

                if (sen.ContainsInsensitive(staticVerb.Person))
                {
                    var indexes = sen.IndexesFrom(staticVerb.Person);
                    AddUniqueValues(result, indexes);
                }
            }
            else
            {
                if (FileHtmlControls.Plural_service.IsSingular(comp))
                {
                    var plural = FileHtmlControls.Plural_service.Pluralize(comp);
                    if (sen.ContainsInsensitive(plural))
                    {
                        var indexes = sen.IndexesFrom(plural);
                        AddUniqueValues(result, indexes);
                    }
                }
            }

            return result;
        }

        private static void AddUniqueValues(List<int> result, List<int> indexes)
        {
            foreach (var ind in indexes)
            {
                if (!result.Contains(ind))
                    result.Add(ind);
            }
        }

        private static List<int> GetIndexOfPart(List<string> parts, string sentence)
        {
            var indexes = new List<int>();

            if (parts == null)
                return indexes;

            if (!parts.Any())
                return indexes;

            foreach (var item in parts)
            {
                // check if it is Be
                if (item.ContainsInsensitive("Be"))
                {
                    foreach (var toBe in VerbsController.VerbToBe)
                    {
                        if (sentence.ContainsInsensitive(item.ReplaceInsensitive("Be", toBe)))
                            indexes.AddRange(sentence.IndexesFrom(toBe));
                    }

                    return indexes;
                }

                if (sentence.ContainsInsensitive(item))
                    indexes.AddRange(sentence.IndexesFrom(item));
            }
            return indexes;
        }

        #endregion
    }
}
