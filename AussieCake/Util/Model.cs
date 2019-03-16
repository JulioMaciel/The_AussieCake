using System.ComponentModel;

namespace AussieCake
{
	public enum Model
	{
		[Description("Collocation")]
		Col,

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
    }
}
