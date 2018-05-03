using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.Models
{
	public class Verb
	{
		public string Infinitive { get; set; }
		public string Past { get; set; }
		public string PastParticiple { get; set; }
		public string Gerund { get; set; } // Present Participle
		public string Person { get; set; }

		public Verb(string infinitive, string past, string pastParticiple, string gerund, string person)
		{
			Infinitive = infinitive;
			Past = past;
			PastParticiple = pastParticiple;
			Gerund = gerund;
			Person = person;
		}

		public Verb()
		{				
		}
	}
}
