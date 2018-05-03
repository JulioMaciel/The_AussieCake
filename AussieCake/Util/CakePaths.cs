using System.IO;

namespace AussieCake.Util
{
	public static class CakePaths
	{
		private static string Project = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\AussieCake";

		public static string Database = Project + "\\Cake.sqlite";

		public static string ScriptSentences = Project + "\\Context\\Scripts\\Insert_Sentences.sql";
		public static string ScriptCollocations = Project + "\\Context\\Scripts\\Insert_Collocations.sql";
		public static string ScriptVerbs = Project + "\\Context\\Scripts\\Insert_Verbs.sql";

		public static string GetIconPath(string icon)
		{
			return Project + @"\\Images\\Icons\\" + icon + ".ico";
		}

	}
}
