using AussieCake.Context;
using AussieCake.Models;
using AussieCake.Util;
using AussieCake.Util.WPF;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

			InsertSentence(model);
			LoadSentencesViewModel();
			Logger.LogItem(new LoggedItem(ModelType.Sentence, model.Text));
		}

		public static void Update(SentenceVM sentence)
		{
			var model = new Sentence(sentence);
			UpdateSentence(model);
			var oldVM = Sentences.FirstOrDefault(x => x.Id == sentence.Id);
			oldVM = sentence;
		}

		public static void Remove(SentenceVM sentence)
		{
			var model = new Sentence(sentence);
			RemoveSentence(model);
			Sentences.Remove(sentence);
		}

		public static void LoadSentencesViewModel()
		{
			if (Sentences == null)
				Sentences = new List<SentenceVM>();

			foreach (var sen in SentencesDB)
			{
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

		public async static Task SaveSentencesFromHtmlBooks()
		{
			string htmlCode = string.Empty;

			string[] filePaths = Directory.GetFiles(CakePaths.ResourceHtmlBooks, "*.htm",
																				 searchOption: SearchOption.TopDirectoryOnly);
			foreach (var path in filePaths)
				htmlCode += File.ReadAllText(path);

			string cleanedCode = CleanHtmlCode(htmlCode);

			await SaveSentencesFromString(cleanedCode);
		}

		public async static Task SaveSentencesFromTxtBooks()
		{
			string allStringBooks = string.Empty;
			string[] filePaths = Directory.GetFiles(CakePaths.ResourceTxtBooks, "*.txt",
																				 searchOption: SearchOption.TopDirectoryOnly);
			
			foreach (var path in filePaths)
				allStringBooks += File.ReadAllText(path);

			await SaveSentencesFromString(allStringBooks);
		}

		public async static Task SaveSentencesFromString(string source)
		{
			var sentences = GetSentencesFromSource(source);
			sentences = sentences.Where(s => (s.Length >= 40 && s.Length <= 80) &&
																			((s.EndsWith(".") && !s.EndsWith("Dr.") && !s.EndsWith("Mr.") && !s.EndsWith("Ms.")) ||
																				s.EndsWith("!") || s.EndsWith("?"))).ToList();

			var savedSentences = await GetCollocationsSentenceFromList(sentences);

			InsertSentencesFound(savedSentences);
		}

		public static bool DoesSentenceContainsCollection(CollocationVM col, SentenceVM sentence)
		{
			// start checking the comp2 because it's here where is the biggest chance of returning false
			var comp2 = CheckSentenceContainsComponent(col.Component2, col.IsComp2Verb, sentence.Text);
			if (!comp2.Item1)
				return false;

			var comp1 = CheckSentenceContainsComponent(col.Component1, col.IsComp1Verb, sentence.Text);
			if (!comp1.Item1)
				return false;

			if (comp1.Item2 > comp2.Item2)
				return false;

			var link = CheckSentenceContainsPart(col.LinkWords, sentence.Text);
			if (link == PartResult.PartNotFound)
				return false;

			var pref = CheckSentenceContainsPart(col.Prefixes, sentence.Text);
			if (pref == PartResult.PartNotFound)
				return false;

			var suf = CheckSentenceContainsPart(col.Suffixes, sentence.Text);
			if (suf == PartResult.PartNotFound)
				return false;

			var position = new StringPositions();
			position.IdSentence = sentence.Id;
			position.Comp1Pos = comp1.Item2;
			position.Comp2Pos = comp2.Item2;
			col.Positions.Add(position);

			col.SentencesId.Add(sentence.Id);
			CollocationController.Update(col);

			return true;
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
						if (DoesSentenceContainsCollection(col, new SentenceVM(sen, false)))
							if (!result.Contains(sen))
							{
								result.Add(sen);
								// debug, erase next Logger line
								//Logger.LogItem(new LoggedItem(ModelType.Sentence, sen));
							}
					}
					Logger.IncreaseProgress();
				});
			});
			await Task.WhenAll(tasks);

			return result;
		}

		private static void InsertSentencesFound(List<string> sentencesFound)
		{
			foreach (var found in sentencesFound)
			{
				var vm = new SentenceVM(found, false);
				Insert(vm);
				Logger.IncreaseProgress();
			}

			ScriptFileCommands.WriteSentencesOnFile(sentencesFound);
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
					Logger.LogItem(new LoggedItem(ModelType.Verb, staticVerb.Infinitive));
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
