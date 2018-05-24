using AussieCake.Models;
using AussieCake.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AussieCake.Context
{
	public class ScriptFileCommands
	{
		public static void WriteSentencesOnFile(List<string> savedSentences)
		{
			if (!savedSentences.Any())
				return;

			var actualFile = File.ReadAllLines(CakePaths.ScriptSentences);

			using (var tw = new StreamWriter(CakePaths.ScriptSentences, true))
			{
				foreach (var sen in savedSentences)
				{
					if (!actualFile.Any(s => s.Contains(sen)))
						tw.WriteLine("insert into Sentence values(NULL, '" + sen + "', NULL, 0);");
					else
						Console.WriteLine("Sentence already stored: " + sen);
				}
			}
		}

		public static void WriteVerbOnFile(Verb verb)
		{
			var actualFile = File.ReadAllLines(CakePaths.ScriptVerbs);

			using (var tw = new StreamWriter(CakePaths.ScriptVerbs, true))
			{
				if (!actualFile.Any(s => s.Contains(verb.Infinitive)))
					tw.WriteLine("insert into Verb values(NULL, '" + verb.Infinitive + "', '" + verb.Past + "', '"
																	+ verb.PastParticiple + "', '" + verb.Person + "', '" + verb.Gerund + "');");
				else
					Console.WriteLine("Verb already stored: " + verb);
			}
		}

		// Collocations script é fixo, pq são do pdf do PTE
	}
}
