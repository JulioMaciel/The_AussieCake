using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace AussieCake.Util
{
    public static class UtilExtensions
    {
        public static string ToDesc(this Enum val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static string ToText(this List<string> list)
        {
            if (list == null)
                return null;

            list.Sort();

            return list.Any() ? (list.Count == 1 ? list.First() : String.Join(";", list.ToArray())) : string.Empty;
        }

        public static string ToText(this List<int> list)
        {
            if (list == null)
                return null;

            return ToText(list.ConvertAll(x => x.ToString()));
        }

        public static int ToInt(this bool value)
        {
            return Convert.ToInt16(value);
        }

        public static bool ToBool(this int value)
        {
            return Convert.ToBoolean(value);
        }

        public static bool EqualsNoCase(this string me, string other)
        {
            return me.Equals(other, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool ContainsInsensitive(this string source, string toCheck)
        {
            return source?.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        public static int? GetPosition(this string source, string toCheck)
        {
            return source?.IndexOf(toCheck, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string ReplaceInsensitive(this string str, string from, string to)
        {
            str = Regex.Replace(str, from, to, RegexOptions.IgnoreCase);
            return str;
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string EmptyIfNull(this string str)
        {
            return str.IsEmpty() ? string.Empty : str;
        }

        public static string NormalizeWhiteSpace(this string input, char normalizeTo = ' ')
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

        public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static List<string> ToListString(this string raw)
        {
            return raw.IsEmpty() ? new List<string>() : raw.Split(';').ToList();
        }

        public static List<int> ToListInt(this string raw)
        {
            return raw.IsEmpty() ? new List<int>() : raw.Split(',').Select(Int32.Parse).ToList();
        }
    }
}
