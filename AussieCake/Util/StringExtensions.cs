using System;
using System.Text.RegularExpressions;

namespace AussieCake.Util
{
	public static class StringExtensions
	{
		public static bool ContainsInsensitive(this string source, string toCheck)
		{
			return source?.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase) >= 0;
		}

		public static int? GetPosition(this string source, string toCheck)
		{
			return source?.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase);
		}

		static public string ReplaceInsensitive(this string str, string from, string to)
		{
			str = Regex.Replace(str, from, to, RegexOptions.IgnoreCase);
			return str;
		}

		static public string NormalizeWhiteSpace(this string input, char normalizeTo = ' ')
		{
			if (string.IsNullOrEmpty(input))
				return string.Empty;

			int current = 0;
			char[] output = new char[input.Length];
			bool skipped = false;

			foreach (char c in input.ToCharArray())
			{
				if (char.IsWhiteSpace(c))
				{
					if (!skipped)
					{
						if (current > 0)
							output[current++] = normalizeTo;

						skipped = true;
					}
				}
				else
				{
					skipped = false;
					output[current++] = c;
				}
			}

			return new string(output, 0, skipped ? current - 1 : current);
		}
	}
}
