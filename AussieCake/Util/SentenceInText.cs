using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.Util.WPF;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AussieCake.Util
{
	public static class SentenceInText
	{
		public async static Task SaveSentencesFromSite(string url)
		{
			string htmlCode = string.Empty;

			using (WebClient client = new WebClient())
			{
				htmlCode = await client.DownloadStringTaskAsync(url);
				//htmlCode = task.Result;
			}

			var cleanedCode = WebUtility.HtmlDecode(htmlCode);
			cleanedCode = NormalizeWhiteSpace(cleanedCode);
			cleanedCode = Regex.Replace(cleanedCode, "<.*?>", " ");
			cleanedCode = Regex.Replace(cleanedCode, "\\r|\\n|\\t", "");
			cleanedCode = cleanedCode.Trim(' ');
			
			await SaveSentencesFromString(cleanedCode);
		}

		public async static Task SaveSentencesFromString(string source)
		{
			var sentences = GetSentencesFromSource(source);
			sentences = sentences.Where(s => (s.Length >= 40 && s.Length <= 80) &&
																			(s.EndsWith(".") || s.EndsWith("!") || s.EndsWith("?"))).ToList();

			var savedSentences = await GetCollocationsSentenceFromList(sentences);

			if (savedSentences.Any())
				WriteSentencesOnFile(savedSentences);

			await Task.Run(() => InsertSentencesFound(savedSentences));

			await Footer.LogFooterOperation(ModelsType.Sentence, OperationType.Created, savedSentences.Count);
		}

		private static void WriteSentencesOnFile(List<string> savedSentences)
		{
			var filePath = SqLiteHelper.CakePath + "\\Context\\Scripts\\Insert_Sentences.sql";
			var actualFile = File.ReadAllLines(filePath);

			using (var tw = new StreamWriter(filePath, true))
			{
				foreach (var sen in savedSentences)
				{
					if (!actualFile.Any(s => s.Contains(sen)))
						tw.WriteLine("insert into Sentence values(NULL, '" + sen + "', NULL);");
					else
						Console.WriteLine("Sentence already stored: " + sen);
				}
			}
		}

		private static List<string> GetSentencesFromSource(string source)
		{
			MatchCollection matchList = Regex.Matches(source, @"[A-Z]+(\w+\,*\;*[ ]{0,1}[\.\?\!]*)+");
			return matchList.Cast<Match>().Select(match => match.Value).ToList();
		}

		private async static Task<List<string>> GetCollocationsSentenceFromList(List<string> sentences)
		{
			//foreach (var col in DBController.Collocations) // aqui vai o select -> acho que aqui pq tem mais Cols do que Sens
			//{
			//	foreach (var sen in sentences) // ou aqui, ou ainda em ambos
			//	{
			//		if (QuestionInSentence.CheckSentenceContainsCollection(col, new SentenceVM(sen)))
			//			if (!result.Contains(sen))
			//				result.Add(sen);
			//	}
			//}

			var result = new List<string>();

			var tasks = DBController.Collocations.Select(async col =>
			{
				await Task.Run(async () => {
					foreach (var sen in sentences)
					{
						if (QuestionInSentence.CheckSentenceContainsCollection(col, new SentenceVM(sen)))
							if (!result.Contains(sen))
								result.Add(sen);
					}
					//report.Report(col.Id);
					await Footer.IncreaseProgress();
				});
			});
			await Task.WhenAll(tasks);//.WaitAll(tasks);//WhenAll(tasks);

			return result;
		}

		private static async Task InsertSentencesFound(List<string> sentencesFound)
		{
			var tasks = sentencesFound.Select(async found =>
			{
				await Task.Run(async () =>
				{
					var vm = new SentenceVM(found, string.Empty);
					SentenceController.Insert(vm);
					await Footer.IncreaseProgress();
				});
			}).ToList();
			await Task.WhenAll(tasks);
		}

		internal static string NormalizeWhiteSpace(string input, char normalizeTo = ' ')
		{
			if (string.IsNullOrEmpty(input))
				return string.Empty;

			int current = 0;
			char[] output = new char[input.Length];
			bool skipped = false;

			foreach (char c in input.ToCharArray())
			{
				if (char.IsWhiteSpace(c))
				{
					if (!skipped)
					{
						if (current > 0)
							output[current++] = normalizeTo;

						skipped = true;
					}
				}
				else
				{
					skipped = false;
					output[current++] = c;
				}
			}

			return new string(output, 0, skipped ? current - 1 : current);
		}

	}
}
