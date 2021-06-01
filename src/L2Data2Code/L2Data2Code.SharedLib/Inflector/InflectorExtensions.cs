using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.SharedLib.Inflector
{
	public static class InflectorExtensions
	{
		public const string UppercaseAccentedCharacters = "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞ";
		public const string LowercaseAccentedCharacters = "ßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýþÿ";

		private static readonly Regex WordsSpliter =
			new Regex(string.Format(@"([A-Z{0}]+[a-z{1}\d]*)|[_\s]", UppercaseAccentedCharacters, LowercaseAccentedCharacters),
			          RegexOptions.Compiled);

		public static IEnumerable<string> SplitWords(this string composedPascalCaseWords)
		{
			foreach (Match regex in WordsSpliter.Matches(composedPascalCaseWords))
			{
				yield return regex.Value;
			}
		}
	}
}