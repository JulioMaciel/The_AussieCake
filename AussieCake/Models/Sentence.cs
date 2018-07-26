using AussieCake.ViewModels;

namespace AussieCake.Models
{
    public class Sentence : BaseModel
    {
        public string Text { get; set; }
        public string PtBr { get; set; }
        public bool IsActive { get; set; }

        public Sentence(string text, string ptBr, bool isActive)
        {
            Text = text;
            PtBr = ptBr;
        }

        public Sentence(string text, bool isActive) : this(text, string.Empty, isActive)
        {
        }

        public Sentence(int id, string text, string ptBr, bool isActive) : this(text, ptBr, isActive)
        {
            Id = id;
        }

        public Sentence(SentenceVM viewModel)
        {
            Id = viewModel.Id;
            Text = viewModel.Text;
            PtBr = viewModel.PtBr;
            IsActive = viewModel.IsActive;
        }
    }
}
