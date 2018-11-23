using AussieCake.Question;
using AussieCake.Util;
using AussieCake.Verb;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AussieCake.Challenge
{
    public static class Sentences
    {
        public async static Task<string> GetSentenceToCollocation(IQuest col)
        {
            var raw_Text = await FileHtmlControls.GetTextFromSite(col.ToLudwigUrl());

            var raw_sentences = await GetRawSentencesFromSource(raw_Text);

            var result = new List<string>();
            var task = raw_sentences.Select(sen => Task.Factory.StartNew(() =>
            {
                if (DoesSenContainsCol((ColVM)col, sen) && !result.Contains(sen))
                    result.Add(sen);
            }));
            await Task.WhenAll(task);

            var chosen = result.PickRandom();

            return chosen;
        }

        private static bool DoesStartEndProperly(string s)
        {
            return ((s.EndsWith(".") && !s.EndsWith("Dr.") && !s.EndsWith("Mr.") && !s.EndsWith("Ms.") && !s.EndsWith("..."))
                                                || s.EndsWith("!") || s.EndsWith("?"));
        }

        private static List<string> GetSentencesFromSource(string source)
        {
            var matchList = Regex.Matches(source, @"[A-Z]+(\w+\,*\;*[ ]{0,1}[\.\?\!]*)+");
            return matchList.Cast<Match>().Select(match => match.Value).ToList();
        }

        private static async Task<List<string>> GetRawSentencesFromSource(string source)
        {
            var sentences = new List<string>();

            Task tasks = Task.Run(() => sentences = GetSentencesFromSource(source));
            await Task.WhenAll(tasks);

            var filteredSentences = new List<string>();
            var task = sentences.Select(sen => Task.Factory.StartNew(() =>
            {
                if (!Errors.IsNullSmallerOrBigger(sen, 40, 120, false))
                {
                    if (DoesStartEndProperly(sen))
                        filteredSentences.Add(sen);
                }
            }));
            await Task.WhenAll(task);

            return filteredSentences;
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

            var max_comp_distance = 25;
            var max_part_distance = 15;

            if (indexComp2 - indexComp1 > max_comp_distance)
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

                if (indexesLink.GetMinimumDistance(indexesComp1) > max_part_distance)
                    return false;

                if (indexesComp2.GetMinimumDistance(indexesLink) > max_part_distance)
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

                if (indexesComp1.GetMinimumDistance(indexesPref) > max_part_distance)
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

                if (indexesSuff.GetMinimumDistance(indexesComp2) > max_part_distance)
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
                var service = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentCulture);
                if (service.IsSingular(comp))
                {
                    var plural = service.Pluralize(comp);
                    if (sen.ContainsInsensitive(plural))
                        return plural;
                }

            }

            return string.Empty;
        }

        private static List<int> GetIndexOfCompatibleWord(string comp, bool isVerb, string sen)
        {
            var result = new List<int>();

            if (sen.ContainsInsensitive(comp, true))
            {
                var indexes = sen.IndexesFrom(comp, true);
                AddUniqueValues(result, indexes);
            }

            if (isVerb)
            {
                VerbModel staticVerb = new VerbModel();

                if (VerbsController.Get().Any(v => v.Infinitive.EqualsNoCase(comp)))
                    staticVerb = VerbsController.Get().First(v => v.Infinitive.EqualsNoCase(comp));
                else
                    staticVerb = VerbsController.ConjugateUnknownVerb(comp);

                if (sen.ContainsInsensitive(staticVerb.Gerund, true))
                {
                    var indexes = sen.IndexesFrom(staticVerb.Gerund, true);
                    AddUniqueValues(result, indexes);
                }

                if (sen.ContainsInsensitive(staticVerb.Past, true))
                {
                    var indexes = sen.IndexesFrom(staticVerb.Past, true);
                    AddUniqueValues(result, indexes);
                }

                if (sen.ContainsInsensitive(staticVerb.PastParticiple, true))
                {
                    var indexes = sen.IndexesFrom(staticVerb.PastParticiple, true);
                    AddUniqueValues(result, indexes);
                }

                if (sen.ContainsInsensitive(staticVerb.Person, true))
                {
                    var indexes = sen.IndexesFrom(staticVerb.Person, true);
                    AddUniqueValues(result, indexes);
                }
            }
            else
            {
                var service = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentCulture);
                if (service.IsSingular(comp))
                {
                    var plural = service.Pluralize(comp);
                    if (sen.ContainsInsensitive(plural, true))
                    {
                        var indexes = sen.IndexesFrom(plural, true);
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
    }
}
