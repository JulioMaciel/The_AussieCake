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
		Verb
	}

    //public enum LoadLevel
    //{
    //    Unloaded = 0,
    //    DB = 1,
    //    CrossData = 2
    //}
}
