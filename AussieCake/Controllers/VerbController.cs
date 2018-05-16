using AussieCake.Context;
using AussieCake.Models;
using AussieCake.ViewModels;
using System.Collections.Generic;
using System.Net;
using System.Windows;

namespace AussieCake.Controllers
{
	public class VerbController : SqLiteHelper
	{
		public static List<VerbVM> Verbs { get; private set; }

		public static void Insert(VerbVM verbVM)
		{
			var verb = new Verb(verbVM);
			Application.Current.Dispatcher.Invoke(() =>
			{
				InsertVerb(verb);
				ScriptFileCommands.WriteVerbOnFile(verb);
			});
		}

		public static void LoadVerbsViewModel()
		{
			if (Verbs == null)
				Verbs = new List<VerbVM>();

			foreach (var verb in VerbsDB)
			{
				var vm = new VerbVM(verb);
				if (!Verbs.Contains(vm))
					Verbs.Add(vm);
			}
		}

		public static List<string> VerbToBe = new List<string>()
		{
			"am", "'m", "is", "'s", "are", "'re", "being", "been", "was", "were"
		};

		public static VerbVM ConjugateUnknownVerb(string verb)
		{
			string htmlCode = string.Empty;

			using (WebClient client = new WebClient())
			{
				htmlCode = client.DownloadString("http://conjugator.reverso.net/conjugation-english-verb-" + verb + ".html");
			}

			var newVerb = new VerbVM();
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
			
			Insert(newVerb);

			return newVerb;
		}

		private static string GetVerbAfterTheIndex(string htmlCode, string time, string initial, bool isPP = false)
		{
			var timeInitial = htmlCode.IndexOf(time);

			if (isPP)
				timeInitial = htmlCode.IndexOf("<p>Past</p>", timeInitial);

			var initialIndex = htmlCode.IndexOf(initial, timeInitial) + initial.Length;
			var finalIndex = htmlCode.IndexOf('<', initialIndex);
			return htmlCode.Substring(initialIndex, finalIndex - initialIndex);
		}
	}
}
