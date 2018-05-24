using System.ComponentModel;

namespace AussieCake.Models
{
	public enum ModelType
	{
		[Description("Collocation")]
		Collocation,

		[Description("Sentence")]
		Sentence,

		[Description("User")]
		User,

		[Description("CollocationAttempt")]
		CollocationAttempt,

		[Description("Verb")]
		Verb
	}
}
