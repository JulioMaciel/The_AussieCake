using AussieCake.Models;

namespace AussieCake.ViewModels
{
	public class VerbVM
	{
		public string Infinitive { get; set; }
		public string Past { get; set; }
		public string PastParticiple { get; set; }
		public string Gerund { get; set; } // Present Participle
		public string Person { get; set; }

		public VerbVM(Verb verb)
		{
			Infinitive = verb.Infinitive;
			Past = verb.Past;
			PastParticiple = verb.PastParticiple;
			Gerund = verb.Gerund;
			Person = verb.Person;
		}

		public VerbVM()
		{
		}

	}
}
