using AussieCake.ViewModels;
using System;

namespace AussieCake.Models
{
	public class Collocation : BaseModel
	{
		public string Prefixes { get; set; } // it's a concatenate list
		public string Component1 { get; set; }
		public string LinkWords { get; set; } // it's a concatenate list
		public string Component2 { get; set; }
		public string Suffixes { get; set; } // it's a concatenate list

		public string PtBr { get; set; }
    public int Importance { get; set; }
    public string SentencesId { get; set; } // it's a concatenate list

		public Collocation()
		{
		}

		public Collocation(CollocationVM viewModel)
		{
			Id = viewModel.Id;
			Prefixes = String.Join(";", viewModel.Prefixes.ToArray());
			Component1 = viewModel.Component1;
			LinkWords = String.Join(";", viewModel.LinkWords.ToArray());
			Component2 = viewModel.Component2;
			Suffixes = String.Join(";", viewModel.Suffixes.ToArray());
			PtBr = viewModel.PtBr;
      Importance = (int)viewModel.Importance;
			SentencesId = String.Join(";", viewModel.SentencesId.ToArray());
		}

		public Collocation(string prefixes, string component1, string linkWords, string component2, string suffixes, string ptBr, int importance, string sentencesId)
		{
			Prefixes = prefixes;
			Component1 = component1;
			LinkWords = linkWords;
			Component2 = component2;
			Suffixes = suffixes;
			PtBr = ptBr;
      Importance = importance;
			SentencesId = sentencesId;
		}

		public Collocation(int id, string prefixes, string component1, string linkWords, string component2, string suffixes, string ptBr, int importance, string sentencesId)
			: this(prefixes, component1, linkWords, component2, suffixes, ptBr, importance, sentencesId)
		{
			Id = id;
		}
	}
}
