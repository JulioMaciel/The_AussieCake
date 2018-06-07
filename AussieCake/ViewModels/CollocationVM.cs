using AussieCake.Controllers;
using AussieCake.Models;
using AussieCake.Util;
using AussieCake.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.ViewModels
{
	public class CollocationVM : IQuestion
  {
		public int Id { get; private set; }

		public List<string> Prefixes { get; set; }
		public string Component1 { get; set; }
		public List<string> LinkWords { get; set; }
		public string Component2 { get; set; }
		public List<string> Suffixes { get; set; }

		public string PtBr { get; set; }
    public Importance Importance { get; set; }
    public List<int> SentencesId { get; set; }

		public bool IsActive { get; set; }

		public bool IsComp1Verb { get; private set; }
		public bool IsComp2Verb { get; private set; }

		public List<DateTry> Tries { get; private set; }
		public int Corrects { get; private set; }
		public int Incorrects { get; private set; }
		public double AverageScore { get; private set; }
		public double ChanceToAppear { get; private set; }

		public List<StringPositions> Positions { get; set; }

		public CollocationVM(List<string> prefixes, string comp1, bool isComp1Verb, List<string> linkWords, string comp2, 
                        bool isComp2Verb, List<string> suffixes, string ptBr, Importance importance, List<int> sentencesId, bool isActive)
		{
			Prefixes = prefixes;
			Component1 = comp1 + (isComp1Verb ? "[v]" : string.Empty);
			IsComp1Verb = isComp1Verb;
			LinkWords = linkWords;
			Component2 = comp2 + (isComp2Verb ? "[v]" : string.Empty); ;
			IsComp2Verb = isComp2Verb;
			Suffixes = suffixes;
			PtBr = ptBr;
      Importance = importance;
      SentencesId = sentencesId;
			IsActive = isActive;
		}

		public CollocationVM(Collocation col)
		{
			Id = col.Id;
			Prefixes = string.IsNullOrEmpty(col.Prefixes) ? new List<string>() : col.Prefixes.Split(';').ToList();
			LinkWords = string.IsNullOrEmpty(col.LinkWords) ? new List<string>() : col.LinkWords.Split(';').ToList();
			Suffixes = string.IsNullOrEmpty(col.Suffixes) ? new List<string>() : col.Suffixes.Split(';').ToList();
			PtBr = string.IsNullOrEmpty(col.PtBr) ? null : col.PtBr;
      Importance = (Importance)col.Importance;
			IsActive = col.IsActive;

			Positions = new List<StringPositions>();

			ReadComponents(col);
			LoadSentencesId(col);
			LoadTries();
		}

		private void ReadComponents(Collocation col)
		{
			if (col.Component1.EndsWith("[v]"))
			{
				Component1 = col.Component1.Remove(col.Component1.Length - 3);
				IsComp1Verb = true;
			}
			else
			{
				Component1 = col.Component1;
				IsComp1Verb = false;
			}

			if (col.Component2.EndsWith("[v]"))
			{
				Component2 = col.Component2.Remove(col.Component2.Length - 3);
				IsComp2Verb = true;
			}
			else
			{
				Component2 = col.Component2;
				IsComp2Verb = false;
			}
		}

		private void LoadSentencesId(Collocation col)
		{
			SentencesId = new List<int>();

			if (string.IsNullOrEmpty(col.SentencesId))
				return;

			var sen = col.SentencesId.Split(';').ToList();
			foreach (var id in sen)
				if (!string.IsNullOrEmpty(id))
					SentencesId.Add(Convert.ToInt16(id));
		}

		private void LoadTries()
		{
			if (AttemptController.CollocationAttempts == null)
				return;

			var triesFromUser = AttemptController.CollocationAttempts
																				.Where(x => x.IdUser == UserController.ActiveId && x.IdCollocation == Id);
			
			if (!triesFromUser.Any())
				return;

			Tries = new List<DateTry>();
			foreach (var item in triesFromUser)
				Tries.Add(new DateTry(item.IsCorrect, item.When));

			AverageScore = Math.Round((double)(100 * Tries.Where(x => x.IsCorrect).ToList().Count / (Tries.Count)));

      ChanceToAppear = ScoreHelper.LoadChanceToAppear(this);
		}

    public List<SentenceVM> GetSentences()
		{
			var sentences = new List<SentenceVM>();
			foreach (var senId in SentencesId)
			{
				int senIdInt = Convert.ToInt16(senId);
				sentences.Add(SentenceController.Sentences.FirstOrDefault(x => x.Id == senIdInt));
			}

			return sentences;
		}

		public double GetAvaregeFromLastDays(int lastDays)
		{
			return Math.Round((double)(100 * Tries.Where(x => x.IsCorrect && 
																												x.When >= DateTime.Now.AddDays(-lastDays)).ToList().Count / (Tries.Count)));
		}
		
	}

	public class DateTry
	{
		public bool IsCorrect { get; set; }
		public DateTime When { get; set; }

		public DateTry(bool isCorrect, DateTime when)
		{
			IsCorrect = isCorrect;
			When = when;
		}
	}

	public class StringPositions
	{
		public int IdSentence { get; set; }
		public int Comp1Pos { get; set; }
		public int Comp2Pos { get; set; }

		public StringPositions()
		{}
	}
}
