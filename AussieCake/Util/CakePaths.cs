﻿using System.IO;

namespace AussieCake.Util
{
	public static class CakePaths
	{
		private static string Project = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\AussieCake";
		private static string Script = "\\Context\\Scripts";

		public static string Database = Project + "\\Cake.sqlite";

		public static string ScriptSentences = Project + Script + "\\Insert_Sentences.sql";
		public static string ScriptCollocations = Project + Script + "\\Insert_Collocations.sql";
		public static string ScriptVerbs = Project + Script + "\\Insert_Verbs.sql";

		public static string ResourceBooksFolder = Project + "\\Resources\\HtmlBooks";

		public static string GetIconPath(string icon)
		{
			return Project + @"\\Images\\Icons\\" + icon + ".ico";
		}

	}
}