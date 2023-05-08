using System;
using System.Text;
using System.Text.RegularExpressions;

namespace DNA.Text
{
	public static class TextTools
	{
		private static Regex SplitWordsRE = new Regex("[a-zA-Z]+", RegexOptions.Compiled);

		public static int CountSame(this string a, int starta, string b, int startb)
		{
			int i;
			for (i = 0; i + starta < a.Length && i + startb < b.Length && a[starta + i] == b[startb + i]; i++)
			{
			}
			return i;
		}

		public static string RemovePatternWhiteSpace(this string source)
		{
			throw new NotImplementedException();
		}

		public static string ReplaceAny(this string source, char[] chars, string newValue)
		{
			foreach (char c in chars)
			{
				source = source.Replace(c.ToString(), newValue);
			}
			return source;
		}

		public static string ReplaceAny(this string source, string[] strings, string newValue)
		{
			foreach (string oldValue in strings)
			{
				source = source.Replace(oldValue, newValue);
			}
			return source;
		}

		public static string Capitalize(this string word)
		{
			StringBuilder stringBuilder = new StringBuilder(word.ToLower());
			stringBuilder[0] = char.ToUpper(stringBuilder[0]);
			return stringBuilder.ToString();
		}

		public static string[] SplitWords(this string text)
		{
			MatchCollection matchCollection = SplitWordsRE.Matches(text);
			string[] array = new string[matchCollection.Count];
			int num = 0;
			foreach (Match item in matchCollection)
			{
				array[num++] = item.Value;
			}
			return array;
		}
	}
}
