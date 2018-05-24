﻿using AussieCake.Models;

namespace AussieCake.ViewModels
{
	public class SentenceVM
  {
    public int Id { get; private set; }

    public string Text { get; set; }
    public string PtBr { get; set; }
		public bool IsActive { get; set; }

		public SentenceVM(string text, bool isActive)
		{
			Text = text;
			IsActive = isActive;
		}

		public SentenceVM(string text, string ptBr, bool isActive) : this(text, isActive)
    {
      PtBr = ptBr;
    }

    public SentenceVM(Sentence sentence)
    {
      Id = sentence.Id;
      Text = sentence.Text;
      PtBr = sentence.PtBr;
			IsActive = sentence.IsActive;
    }
  }
}
