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
using System.Windows;

namespace AussieCake.Util
{
	public static class QuestionInSentence
	{
		private static List<string> VerbToBe = new List<string>()
		{
			"am", "'m", "is", "'s", "are", "'re", "being", "been", "was", "were"
		};

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

				return true;
			}

			return false;
		}

		private static (bool, int) CheckSentenceContainsComponent(string comp, bool isAVerb, string sentence)
		{
			var infinitivePosition = sentence.GetPosition(comp);
			if (infinitivePosition >= 0)
				return (true, infinitivePosition.Value);

			if (isAVerb)
			{
				var staticVerb = SqLiteHelper.VerbsDB.FirstOrDefault(v => v.Infinitive.Equals(comp, StringComparison.CurrentCultureIgnoreCase));

				if (staticVerb == null)
				{
					staticVerb = ConjugateUnknownVerb(comp);
					Footer.AddExtraInfo(ModelsType.Verb, 1);
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
					foreach (var toBe in VerbToBe)
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

		private static Verb ConjugateUnknownVerb(string verb)
		{
			string htmlCode = string.Empty;

			using (WebClient client = new WebClient())
			{
				htmlCode = client.DownloadString("http://conjugator.reverso.net/conjugation-english-verb-"+verb+".html");
			}

			var newVerb = new Verb();
			newVerb.Infinitive = verb;

			if (string.IsNullOrEmpty(htmlCode))
			{
				MessageBox.Show("Site Conjugator doesn't have the verb '" + verb + "'. Operation will continue.");
				return newVerb;
			}

			newVerb.Past = GetVerbAfterTheIndex(htmlCode, "<p>Preterite</p>", "I </i><i class=\"verbtxt\">");
			newVerb.PastParticiple = GetVerbAfterTheIndex(htmlCode, "<h4>Participle</h4>", "<li><i class=\"verbtxt\">", true);
			newVerb.Person = GetVerbAfterTheIndex(htmlCode, "<h4>Indicative</h4>", "it </i><i class=\"verbtxt\">");
			newVerb.Gerund = GetVerbAfterTheIndex(htmlCode, "<p>Present continuous</p>", "am </i><i class=\"verbtxt\">");

			WriteVerbOnFile(newVerb);

			return newVerb;
		}

		private static string GetVerbAfterTheIndex(string htmlCode, string time, string initial, bool isPP = false)
		{
			var timeInitial = htmlCode.IndexOf(time);

			if (isPP)
				timeInitial = htmlCode.IndexOf("<p>Past</p>", timeInitial);

			//var aaaa = htmlCode.Substring(timeInitial, time.Length);
			var initialIndex = htmlCode.IndexOf(initial, timeInitial) + initial.Length;
			//var bbbb = htmlCode.Substring(initialIndex, initial.Length);
			var finalIndex = htmlCode.IndexOf('<', initialIndex);
			//var cccc = htmlCode.Substring(finalIndex, 1);
			return htmlCode.Substring(initialIndex, finalIndex - initialIndex);
		}

		private static void WriteVerbOnFile(Verb verb)
		{
			var filePath = SqLiteHelper.CakePath + "\\Context\\Scripts\\Insert_Verbs.sql";
			var actualFile = File.ReadAllLines(filePath);

			using (var tw = new StreamWriter(filePath, true))
			{
				if (!actualFile.Any(s => s.Contains(verb.Infinitive)))
					tw.WriteLine("insert into Verb values(NULL, '" + verb.Infinitive + "', '" + verb.Past + "', '" 
																	+ verb.PastParticiple + "', '" + verb.Person + "', '" + verb.Gerund + "');");
				else
					Console.WriteLine("Verb already stored: " + verb);
			}
			
			SqLiteHelper.InsertVerb(verb);
			SqLiteHelper.GetVerbsDB();
		}

	}

	public enum PartResult
	{
		PartsNotSent,
		PartNotFound,
		PartFound,
	}

	public static class StringExtensions
	{
		public static bool ContainsInsensitive(this string source, string toCheck)
		{
			return source?.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase) >= 0;
		}

		public static int? GetPosition(this string source, string toCheck)
		{
			return source?.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase);
		}

		static public string ReplaceInsensitive(this string str, string from, string to)
		{
			str = Regex.Replace(str, from, to, RegexOptions.IgnoreCase);
			return str;
		}
	}
}
