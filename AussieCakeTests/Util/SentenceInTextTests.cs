using AussieCake.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AussieCakeTests.Util
{
	[TestClass]
	public class SentenceInTextTests
	{
		[TestMethod]
		public void GetSentencesFromStringTest()
		{
			DBController.LoadData();
			
			var url = "https://www.mushroom-ebooks.com/authors/akers/samplers/AKERSSunsOfScorpio%28Sampler%29.html";
			//var isThereAnySentences = SentenceInText.SaveSentencesFromSite(url);
			//Assert.IsTrue(isThereAnySentences.Count == 2);
		}
	}
}
