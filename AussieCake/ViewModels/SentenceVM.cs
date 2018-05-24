using AussieCake.Models;

namespace AussieCake.ViewModels
{
	public class SentenceVM
  {
    public int Id { get; private set; }

    public string Text { get; set; }
    public string PtBr { get; set; }

		public SentenceVM(string text)
		{
			Text = text;
		}

		public SentenceVM(string text, string ptBr) : this(text)
    {
      PtBr = ptBr;
    }

    public SentenceVM(Sentence sentence)
    {
      Id = sentence.Id;
      Text = sentence.Text;
      PtBr = sentence.PtBr;
    }
  }
}
