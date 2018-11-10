using AussieCake.Sentence;
using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AussieCake.Util
{
    public static class FileHtmlControls
    {
        public static PluralizationService Plural_service = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentCulture);

        public async static Task<List<string>> GetSentencesFromSite(string url)
        {
            string htmlCode = string.Empty;

            using (WebClient client = new WebClient())
            {
                htmlCode = await client.DownloadStringTaskAsync(url);
            }

            string cleanedCode = CleanHtmlCode(htmlCode);

            return await AutoGetSentences.GetSentencesFromString(cleanedCode);
        }

        public async static Task SaveSentencesFromHtmlBooks()
        {
            string htmlCode = string.Empty;

            string[] filePaths = Directory.GetFiles(CakePaths.ResourceHtmlBooks, "*.htm",
                                                    searchOption: SearchOption.TopDirectoryOnly);
            foreach (var path in filePaths)
                htmlCode += File.ReadAllText(path);

            string cleanedCode = CleanHtmlCode(htmlCode);

            await AutoGetSentences.GetSentencesFromString(cleanedCode);
        }

        public async static Task<List<string>> SaveSentencesFromTxtBooks()
        {
            string allStringBooks = string.Empty;
            string[] filePaths = Directory.GetFiles(CakePaths.ResourceTxtBooks, "*.txt",
                                                    searchOption: SearchOption.TopDirectoryOnly);

            foreach (var path in filePaths)
                allStringBooks += File.ReadAllText(path);

            return await AutoGetSentences.GetSentencesFromString(allStringBooks);
        }

        private static string CleanHtmlCode(string htmlCode)
        {
            var cleanedCode = WebUtility.HtmlDecode(htmlCode);
            cleanedCode = cleanedCode.NormalizeWhiteSpace();
            cleanedCode = Regex.Replace(cleanedCode, "<.*?>", " ");
            cleanedCode = Regex.Replace(cleanedCode, "\\r|\\n|\\t", "");
            cleanedCode = cleanedCode.Trim(' ');
            return cleanedCode;
        }

        //public static List<string> GetSynonyms(string word, bool isFirstUp)
        //{
        //    string htmlCode = string.Empty;

        //    using (WebClient client = new WebClient())
        //        htmlCode = client.DownloadString("https://www.thesaurus.com/browse/" + word);

        //    if (htmlCode.IsEmpty())
        //        Errors.ThrowErrorMsg(ErrorType.InexistentWordOrSiteOff, word);

        //    var result = new List<string>();

        //    htmlCode = htmlCode.GetSinceTo("iframe ends here", "adBlockDetector start");

        //    var pattern = "3kshty etbu2a31\">(\\w+)<";
        //    MatchCollection matchList = Regex.Matches(htmlCode, pattern);
        //    result = matchList.Cast<Match>().Select(match => match.Value).ToList();

        //    for (int i = 0; i < result.Count; i++)
        //    {
        //        result[i] = result[i].Remove(result[i].Length - 1); // remove (<)
        //        result[i] = result[i].Remove(0, 17); // remove (3kshty etbu2a31">)
        //    }

        //    if (!result.Any())
        //    {
        //        using (WebClient client = new WebClient())
        //            htmlCode = client.DownloadString("https://www.synonym.com/synonyms/" + word);

        //        if (htmlCode.IsEmpty())
        //            Errors.ThrowErrorMsg(ErrorType.InexistentWordOrSiteOff, word);

        //        pattern = @"synonyms/([a-z]+)"">[a-z]+</a>\n +</li><li class=""syn";
        //        matchList = Regex.Matches(htmlCode, pattern);
        //        result = matchList.Cast<Match>().Select(match => match.Value).ToList();

        //        for (int i = 0; i < result.Count; i++)
        //            result[i] = result[i].GetBetween(">", "<");
        //    }

        //    if (isFirstUp)
        //        result.ForEach(x => x.First().ToString().ToUpper());

        //    return result;
        //}

        //public static string GetPlural(string word)
        //{
        //    string htmlCode = string.Empty;

        //    using (WebClient client = new WebClient())
        //        htmlCode = client.DownloadString("https://dictionary.cambridge.org/dictionary/english/" + word);

        //    if (htmlCode.IsEmpty())
        //        Errors.ThrowErrorMsg(ErrorType.InexistentWordOrSiteOff, word);

        //    htmlCode = htmlCode.GetSinceTo("cald4-1-1-1", "accord-basic js-accord accord-basic--shallow");
        //    //htmlCode = CleanHtmlCode(htmlCode);

        //    var pattern = @"class=\""phrase\"">[a-z]+</b></span>";
        //    var matchList = Regex.Matches(htmlCode, pattern);
        //    var result = matchList.Cast<Match>().Select(match => match.Value).First();//.ToList();

        //    result = result.GetBetween(">", "<");

        //    return result;
        //}

        public static IEnumerable<string> GetSynonyms(string word)
        {
            word = word.ToLower();

            var found = new List<string>();
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            //var objLanguage = Microsoft.Office.Interop.Word.WdLanguageID.wdEnglishUS;
            var theSynonyms = wordApp.get_SynonymInfo(word);

            foreach (var Meaning in theSynonyms.MeaningList as Array)
            {
                var synonym = Meaning.ToString();
                if (!IsSynonymTooSimilar(word, synonym, found))
                    found.Add(synonym);
            }

            for (int ii = 0; ii < found.Count; ii++)
            {
                theSynonyms = wordApp.SynonymInfo[found[ii]];

                foreach (string synonym in theSynonyms.MeaningList as Array)
                {
                    if (IsSynonymTooSimilar(word, synonym, found))
                        continue;

                    found.Add(synonym);
                }

                if (found.Count > 6)
                    return found;
            }

            return found;
        }

        private static bool IsSynonymTooSimilar(string word, string synonym, List<string> found)
        {
            var are_bigger_than_5 = synonym.Length > 5 && word.Length > 5;

            var cut_slice = are_bigger_than_5 ? 5 : (word.Length < synonym.Length ? word.Length - 1 : synonym.Length - 1);

            var syn_part = synonym.Substring(0, cut_slice);
            var word_part = word.Substring(0, cut_slice);

            if (syn_part == word_part || found.Any(x => x.StartsWith(syn_part)))
                return true;

            return false;
        }
    }
}
