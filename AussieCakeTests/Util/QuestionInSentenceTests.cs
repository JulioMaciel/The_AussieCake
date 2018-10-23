using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AussieCake.Util.Tests
{
    [TestClass()]
	public class QuestionInSentenceTests
	{
		[TestMethod()]
		public void CheckSentenceContainsCollectionTest()
		{
			SqLiteHelper.InitializeDB();

			//var col1 = new CollocationVM(new Collocation(null, "assume[v]", "the", "role", "of", null, 0, null));
			//var col2 = new CollocationVM(new Collocation("on;upon", "closer", null, "inspection", null, null, 0, null));
			//var col3 = new CollocationVM(new Collocation("be of", "considerable", null, "importance", null, null, 0, null));
			//var col4 = new CollocationVM(new Collocation(null, "create[v]", "an", "opportunity", null, null, 0, null));
			//var col5 = new CollocationVM(new Collocation(null, "take[v]", "up the", "role", "of;as", null, 0, null));

			//var sen1 = new SentenceVM("He assumed the role of president of the company.");
			//var sen2 = new SentenceVM("However, upon closer inspection, the link may actually take you to a website that has nothing to do with the company.");
			//var sen3 = new SentenceVM("Visa-free travel to the EU is of considerable importance to the people of the Western Balkans. ");
			//var sen4 = new SentenceVM("They create an opportunity for direct contact, also physical, through touch.");
			//var sen5 = new SentenceVM("Players took up the role of Hunters and selected from three types of races and different classes.");
			//var sen6 = new SentenceVM("He was assuming his new genre.");
			//var sen7 = new SentenceVM("They're on a minutious inspection.");
			//var sen8 = new SentenceVM("It had a determinant importance to be of considerable.");
			//var sen9 = new SentenceVM("An opportunity has been created.");
			//var sen10 = new SentenceVM("We're going to taking up the role which is vital.");

			//Assert.IsTrue(SentenceController.DoesSentenceContainsCollection(col1, sen1));
			//Assert.IsTrue(SentenceController.DoesSentenceContainsCollection(col2, sen2));
			//Assert.IsTrue(SentenceController.DoesSentenceContainsCollection(col3, sen3));
			//Assert.IsTrue(SentenceController.DoesSentenceContainsCollection(col4, sen4));
			//Assert.IsTrue(SentenceController.DoesSentenceContainsCollection(col5, sen5));

			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col1, sen5));
			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col2, sen4));
			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col3, sen2));
			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col4, sen3));
			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col5, sen1));

			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col1, sen6));
			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col2, sen7));
			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col3, sen8));
			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col4, sen9));
			//Assert.IsFalse(SentenceController.DoesSentenceContainsCollection(col5, sen10));
		}
	}
}