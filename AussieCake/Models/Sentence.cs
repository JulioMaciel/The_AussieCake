using AussieCake.ViewModels;

namespace AussieCake.Models
{
	public class Sentence : BaseModel
	{
		public string Text { get; set; }
		public string PtBr { get; set; }

		public Sentence(string text, string ptBr)
		{
			Text = text;
			PtBr = ptBr;
		}

		public Sentence(string text) : this (text, string.Empty)
		{
		}

		public Sentence(int id, string text, string ptBr) : this(text, ptBr)
		{
			Id = id;
		}

    public Sentence(SentenceVM viewModel)
    {
      Id = viewModel.Id;
      Text = viewModel.Text;
      PtBr = viewModel.PtBr;
    }
	}
}
