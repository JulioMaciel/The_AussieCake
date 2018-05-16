using AussieCake.Context;
using AussieCake.Models;
using AussieCake.Util;
using AussieCake.Util.WPF;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace AussieCake.Controllers
{
	public class SentenceController : SqLiteHelper
	{
		public static List<SentenceVM> Sentences { get; private set; }

		#region Public Methods

		public static void Insert(SentenceVM sentence)
		{
			if (Sentences.Any(s => s.Text == sentence.Text))
				return;


			var model = new Sentence(sentence);
			Application.Current.Dispatcher.Invoke(() =>
			{
				InsertSentence(model);
				LoadSentencesViewModel();
				Footer.AddInfo();
			});
		}

		public static void Update(SentenceVM sentence)
		{
			var model = new Sentence(sentence);
			Application.Current.Dispatcher.Invoke(() => UpdateSentence(model));
			var oldVM = Sentences.FirstOrDefault(x => x.Id == sentence.Id);
			oldVM = sentence;
		}

		public static void Remove(SentenceVM sentence)
		{
			var model = new Sentence(sentence);
			Application.Current.Dispatcher.Invoke(() => RemoveSentence(model));
			Sentences.Remove(sentence);
		}

		public static void LoadSentencesViewModel()
		{
			if (Sentences == null)
				Sentences = new List<SentenceVM>();

			foreach (var sen in SentencesDB)
			{
				//var vm = new SentenceVM(sen);
				if (!Sentences.Any(v => v.Id == sen.Id))
					Sentences.Add(new SentenceVM(sen));
			}
		}

		public async static Task SaveSentencesFromSite(string url)
		{
			string htmlCode = string.Empty;

			using (WebClient client = new WebClient())
			{
				htmlCode = await client.DownloadStringTaskAsync(url);
			}

			string cleanedCode = CleanHtmlCode(htmlCode);

			await SaveSentencesFromString(cleanedCode);
		}

		public async static Task SaveSentencesFromStaticResource()
		{
			string htmlCode = string.Empty;

			string[] filePaths = Directory.GetFiles(CakePaths.ResourceBooksFolder, "*.htm",
																				 searchOption: SearchOption.TopDirectoryOnly);
			int i = 0;
			Parallel.For(0, filePaths.Length, x =>
			{
				htmlCode += File.ReadAllText(filePaths[i]);
				i++;
			});

			string cleanedCode = CleanHtmlCode(htmlCode);

			await SaveSentencesFromString(cleanedCode);
		}

		public async static Task SaveSentencesFromString(string source)
		{
			var sentences = GetSentencesFromSource(source);
			sentences = sentences.Where(s => (s.Length >= 40 && s.Length <= 80) &&
																			((s.EndsWith(".") && !s.EndsWith("Dr.") && !s.EndsWith("Mr.") && !s.EndsWith("Ms.")) || 
																				s.EndsWith("!") || s.EndsWith("?"))).ToList();

			var savedSentences = await GetCollocationsSentenceFromList(sentences);
			
			await Task.Run(() => InsertSentencesFound(savedSentences));
		}

		public static bool CheckSentenceContainsCollection(CollocationVM col, SentenceVM sentence)
		{
			var pref = CheckSentenceContainsPart(col.Prefixes, sentence.Text);
			if (pref == PartResult.PartNotFound)
				return false;

			var comp1 = CheckSentenceContainsComponent(col.Component1, col.IsComp1Verb, sentence.Text);
			if (!comp1.Item1)
				return false;

			var link = CheckSentenceContainsPart(col.LinkWords, sentence.Text);
			if (link == PartResult.PartNotFound)
				return false;

			var comp2 = CheckSentenceContainsComponent(col.Component2, col.IsComp2Verb, sentence.Text);
			if (!comp2.Item1)
				return false;

			var suf = CheckSentenceContainsPart(col.Suffixes, sentence.Text);
			if (suf == PartResult.PartNotFound)
				return false;

			if (comp1.Item2 < comp2.Item2 ? true : false)
			{
				var position = new StringPositions();
				position.IdSentence = sentence.Id;
				position.Comp1Pos = comp1.Item2;
				position.Comp2Pos = comp2.Item2;
				col.Positions.Add(position);

				col.SentencesId.Add(sentence.Id);
				CollocationController.Update(col);

				return true;
			}

			return false;
		}

		#endregion

		#region Private Methods

		private static string CleanHtmlCode(string htmlCode)
		{
			var cleanedCode = WebUtility.HtmlDecode(htmlCode);
			cleanedCode = cleanedCode.NormalizeWhiteSpace();
			cleanedCode = Regex.Replace(cleanedCode, "<.*?>", " ");
			cleanedCode = Regex.Replace(cleanedCode, "\\r|\\n|\\t", "");
			cleanedCode = cleanedCode.Trim(' ');
			return cleanedCode;
		}

		private static List<string> GetSentencesFromSource(string source)
		{
			MatchCollection matchList = Regex.Matches(source, @"[A-Z]+(\w+\,*\;*[ ]{0,1}[\.\?\!]*)+");
			return matchList.Cast<Match>().Select(match => match.Value).ToList();
		}

		private async static Task<List<string>> GetCollocationsSentenceFromList(List<string> sentences)
		{
			var result = new List<string>();

			var tasks = CollocationController.Collocations.Select(async col =>
			{
				await Task.Run(() =>
				{
					foreach (var sen in sentences)
					{
						if (CheckSentenceContainsCollection(col, new SentenceVM(sen)))
							if (!result.Contains(sen))
								result.Add(sen);
					}
					Footer.IncreaseProgress();
				});
			});
			await Task.WhenAll(tasks);

			return result;
		}

		private static async Task InsertSentencesFound(List<string> sentencesFound)
		{
			var tasks = sentencesFound.Select(async found =>
			{
				await Task.Run(() =>
				{
					var vm = new SentenceVM(found);
					Insert(vm);
					Footer.IncreaseProgress();
				});
			}).ToList();
			await Task.WhenAll(tasks);

			Application.Current.Dispatcher.Invoke(() => ScriptFileCommands.WriteSentencesOnFile(sentencesFound));
		}

		private static (bool, int) CheckSentenceContainsComponent(string comp, bool isAVerb, string sentence)
		{
			var infinitivePosition = sentence.GetPosition(comp);
			if (infinitivePosition >= 0)
				return (true, infinitivePosition.Value);

			if (isAVerb)
			{
				var staticVerb = VerbController.Verbs.FirstOrDefault(v => v.Infinitive.Equals(comp, StringComparison.CurrentCultureIgnoreCase));

				if (staticVerb == null)
				{
					staticVerb = VerbController.ConjugateUnknownVerb(comp);
					Footer.AddExtraInfo(ModelsType.Verb);
				}

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
					foreach (var toBe in VerbController.VerbToBe)
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
