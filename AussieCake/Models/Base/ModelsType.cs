using System.ComponentModel;

namespace AussieCake.Models
{
	public enum ModelsType
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

	public static class EnumExtensions
	{
		public static string ToDescString(this ModelsType val)
		{
			DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : string.Empty;
		}
	}
}
