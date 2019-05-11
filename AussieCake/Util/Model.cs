using System.ComponentModel;

namespace AussieCake
{
	public enum Model
	{
		[Description("Vocabulary")]
		Voc,

		[Description("Sentence")]
		Sen,

		[Description("Verb")]
		Verb,

        [Description("Essay")]
        Essay,

        [Description("SumRetell")]
        SumRetell,

        [Description("DescribeImage")]
        DescImg,

        [Description("Pronunciation")]
        Pron,

        [Description("Spelling")]
        Spell,
    }
}
