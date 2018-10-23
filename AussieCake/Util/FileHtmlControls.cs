using AussieCake.Sentence;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AussieCake.Util
{
    public static class FileHtmlControls
    {
        public async static Task SaveSentencesFromSite(string url)
        {
            string htmlCode = string.Empty;

            using (WebClient client = new WebClient())
            {
                htmlCode = await client.DownloadStringTaskAsync(url);
            }

            string cleanedCode = CleanHtmlCode(htmlCode);

            await SenControl.SaveSentencesFromString(cleanedCode);
        }

        public async static Task SaveSentencesFromHtmlBooks()
        {
            string htmlCode = string.Empty;

            string[] filePaths = Directory.GetFiles(CakePaths.ResourceHtmlBooks, "*.htm", 
                                                    searchOption: SearchOption.TopDirectoryOnly);
            foreach (var path in filePaths)
                htmlCode += File.ReadAllText(path);

            string cleanedCode = CleanHtmlCode(htmlCode);

            await SenControl.SaveSentencesFromString(cleanedCode);
        }

        public async static Task SaveSentencesFromTxtBooks()
        {
            string allStringBooks = string.Empty;
            string[] filePaths = Directory.GetFiles(CakePaths.ResourceTxtBooks, "*.txt", 
                                                    searchOption: SearchOption.TopDirectoryOnly);

            foreach (var path in filePaths)
                allStringBooks += File.ReadAllText(path);

            await SenControl.SaveSentencesFromString(allStringBooks);
        }

        public static string CleanHtmlCode(string htmlCode)
        {
            var cleanedCode = WebUtility.HtmlDecode(htmlCode);
            cleanedCode = cleanedCode.NormalizeWhiteSpace();
            cleanedCode = Regex.Replace(cleanedCode, "<.*?>", " ");
            cleanedCode = Regex.Replace(cleanedCode, "\\r|\\n|\\t", "");
            cleanedCode = cleanedCode.Trim(' ');
            return cleanedCode;
        }

        public static List<string> GetSentencesFromSource(string source)
        {
            MatchCollection matchList = Regex.Matches(source, @"[A-Z]+(\w+\,*\;*[ ]{0,1}[\.\?\!]*)+");
            return matchList.Cast<Match>().Select(match => match.Value).ToList();
        }
    }
}
