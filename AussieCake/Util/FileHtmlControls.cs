﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;

namespace AussieCake.Util
{
    public static class FileHtmlControls
    {
        public static string GetTextFromSite(string url)
        {
            string htmlCode = string.Empty;

            using (WebClient client = new WebClient())
                htmlCode = client.DownloadString(url);

            string cleanedCode = CleanHtmlCode(htmlCode);

            return cleanedCode;
        }

        public static string GetTextFromHtmlBooks()
        {
            string htmlCode = string.Empty;

            string[] filePaths = Directory.GetFiles(CakePaths.ResourceHtmlBooks, "*.htm",
                                                    searchOption: SearchOption.TopDirectoryOnly);
            foreach (var path in filePaths)
                htmlCode += File.ReadAllText(path);

            string cleanedCode = CleanHtmlCode(htmlCode);

            return cleanedCode;
        }

        public static string GetTextFromTxtBooks()
        {
            string allStringBooks = string.Empty;
            string[] filePaths = Directory.GetFiles(CakePaths.ResourceTxtBooks, "*.txt",
                                                    searchOption: SearchOption.TopDirectoryOnly);

            foreach (var path in filePaths)
                allStringBooks += File.ReadAllText(path);

            return allStringBooks;
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

        public static List<string> GetSynonymsOnWeb(string word, List<string> invalid_synonyms, bool isFirstUp)
        {
            string htmlCode = string.Empty;

            using (WebClient client = new WebClient())
                htmlCode = client.DownloadString("https://www.thesaurus.com/browse/" + word);

            if (htmlCode.IsEmpty())
                Errors.ThrowErrorMsg(ErrorType.InexistentWordOrSiteOff, word);

            htmlCode = htmlCode.GetSinceTo("iframe ends here", "adBlockDetector start");

            var pattern = "3kshty etbu2a31\">(\\w+)<";
            MatchCollection matchList = Regex.Matches(htmlCode, pattern);
            var regexResult = matchList.Cast<Match>().Select(match => match.Value).ToList();

            var result = new List<string>(6);

            var limit = regexResult.Count > 6 ? 6 : result.Count;
            for (int i = 0; i < limit; i++)
            {
                var syn = regexResult[i];
                syn = syn.Remove(syn.Length - 1); // remove (<)
                syn = syn.Remove(0, 17); // remove (3kshty etbu2a31">)

                if (!syn.IsLettersOnly() && syn.Length < 20)
                {
                    if (!invalid_synonyms.Contains(syn))
                    {
                        result.Add(syn);
                    }
                    else
                        Debug.WriteLine("Synonym " + syn + " was blocked because it was on the invalid list.");
                }
            }

            if (!result.Any() || result.Count < 6)
            {
                using (WebClient client = new WebClient())
                    htmlCode = client.DownloadString("https://www.synonym.com/synonyms/" + word);

                if (htmlCode.IsEmpty())
                    Errors.ThrowErrorMsg(ErrorType.InexistentWordOrSiteOff, word);

                pattern = @"synonyms/([a-z]+)"">[a-z]+</a>\n +</li><li class=""syn";
                matchList = Regex.Matches(htmlCode, pattern);
                regexResult = matchList.Cast<Match>().Select(match => match.Value).ToList();

                limit = regexResult.Count > 6 ? (6 - result.Count) : (regexResult.Count - result.Count);
                for (int i = 0; i < limit; i++)
                {
                    var syn = regexResult[i];

                    syn = syn.GetBetween(">", "<");

                    if (!syn.IsLettersOnly() && !result.Contains(syn) && syn.Length < 20)
                    {
                        if (!invalid_synonyms.Contains(syn))
                        {
                            result.Add(syn);
                        }
                        else
                            Debug.WriteLine("Synonym " + syn + " was blocked because it was on the invalid list.");
                    }
                }
            }

            if (isFirstUp)
                result.ForEach(x => x.First().ToString().ToUpper());

            return result;
        }

        public static string GetPluralOnWeb(string word)
        {
            string htmlCode = string.Empty;

            using (WebClient client = new WebClient())
                htmlCode = client.DownloadString("https://dictionary.cambridge.org/dictionary/english/" + word);

            if (htmlCode.IsEmpty())
                Errors.ThrowErrorMsg(ErrorType.InexistentWordOrSiteOff, word);

            htmlCode = htmlCode.GetSinceTo("cald4-1-1-1", "accord-basic js-accord accord-basic--shallow");

            var pattern = @"class=\""phrase\"">[a-z]+</b></span>";
            var matchList = Regex.Matches(htmlCode, pattern);
            var result = matchList.Cast<Match>().Select(match => match.Value).First();

            result = result.GetBetween(">", "<");

            return result;
        }

        public static IEnumerable<string> GetSynonyms(string word, List<string> invalid_synonyms, Microsoft.Office.Interop.Word.Application wordApp)
        {
            word = word.ToLower();

            var found = new List<string>();
            var theSynonyms = wordApp.get_SynonymInfo(word);

            foreach (var Meaning in theSynonyms.MeaningList as Array)
            {
                if (found.Count >= 4)
                    return found;

                var synonym = Meaning.ToString();
                if (!IsSynonymTooSimilar(word, synonym, found))
                {
                    if (!invalid_synonyms.Contains(synonym))
                    {
                        found.Add(synonym);
                    }
                    else
                        Debug.WriteLine("Synonym " + synonym + " was blocked because it was on the invalid list.");
                }
            }

            for (int ii = 0; ii < found.Count; ii++)
            {
                theSynonyms = wordApp.SynonymInfo[found[ii]];

                foreach (string synonym in theSynonyms.MeaningList as Array)
                {
                    if (found.Count >= 4)
                        return found;

                    if (IsSynonymTooSimilar(word, synonym, found))
                        continue;

                    found.Add(synonym);
                }
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

        public static void PlayMp3FromUrl(string url)
        {
            using (Stream ms = new MemoryStream())
            {
                using (Stream stream = WebRequest.Create(url)
                    .GetResponse().GetResponseStream())
                {
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                }

                ms.Position = 0;
                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new Mp3FileReader(ms))))
                {
                    using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        waveOut.Init(blockAlignedStream);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
        }

        public static void PlayPronunciation(string text, Control ctrl = null)
        {
            var url = "https://dictionary.cambridge.org/dictionary/english/" + text.ToLower();
            var html = string.Empty;

            using (WebClient client = new WebClient())
                html = client.DownloadString(url);

            var pattern = "British English pronunciation\" data-src-mp3=\"/media/english/(.*?)\"";
            var matches = Regex.Matches(html, pattern, RegexOptions.Singleline);

            if (matches.Count < 1)
            {
                pattern = "American pronunciation\" data-src-mp3=\"/media/english/(.*?)\"";
                matches = Regex.Matches(html, pattern, RegexOptions.Singleline);

                if (matches.Count < 1)
                {
                    (ctrl as TextBox).Background = Brushes.LightGoldenrodYellow;
                    (ctrl as TextBox).Text = url;
                }
                else
                {
                    PlayIt(matches);
                }
            }
            else
                PlayIt(matches);
        }

        private static void PlayIt(MatchCollection matches)
        {
            var url_audio_word = matches[0].Value;
            var form = url_audio_word.Remove(0, 46);
            var form2 = form.Remove(form.IndexOf(".mp3") + 4);
            var url_audio = "https://dictionary.cambridge.org/" + form2;

            PlayMp3FromUrl(url_audio);
        }
    }
}
