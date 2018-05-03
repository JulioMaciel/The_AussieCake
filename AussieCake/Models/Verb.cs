using AussieCake.ViewModels;

namespace AussieCake.Models
{
	public class Verb
	{
		public string Infinitive { get; set; }
		public string Past { get; set; }
		public string PastParticiple { get; set; }
		public string Gerund { get; set; } // Present Participle
		public string Person { get; set; }

		public Verb()
		{
		}

		public Verb(string infinitive, string past, string pastParticiple, string gerund, string person)
		{
			Infinitive = infinitive;
			Past = past;
			PastParticiple = pastParticiple;
			Gerund = gerund;
			Person = person;
		}

		public Verb(VerbVM viewModel)
		{
			Infinitive = viewModel.Infinitive;
			Past = viewModel.Past;
			PastParticiple = viewModel.PastParticiple;
			Gerund = viewModel.Gerund;
			Person = viewModel.Person;
		}
	}
}
