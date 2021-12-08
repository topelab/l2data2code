using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Lib
{
    /// <summary>Name Helper</summary>
    public static class NameHelper
    {
        /// <summary>The regex camel case</summary>
        private static readonly Regex regexCamelCase = new("(?<word>[A-Z]{2,}|[A-Z][^A-Z]*?|^[^A-Z]*?)(?=[A-Z]|$)", RegexOptions.Compiled);

        /// <summary>Cleans up.</summary>
        /// <param name="str">The string.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <returns></returns>
        public static string PascalCamelCase(this string str, bool remove) => TransformName(str.RemoveWord1(remove).CleanName(), null, true);

        /// <summary>Removes the word1.</summary>
        /// <param name="name">The name.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <returns></returns>
        private static string RemoveWord1(this string name, bool remove)
        {
            if (!remove)
            {
                return name;
            }

            var parts = name.Split('_');
            if (parts.Length > 1)
            {
                return string.Join("_", parts.Skip(1));
            }

            return name;
        }

        /// <summary>Transforms the name.</summary>
        /// <param name="name">The name.</param>
        /// <param name="wordsSeparator">The words separator.</param>
        /// <param name="isCamelCase">if set to <c>true</c> [is camel case].</param>
        /// <param name="isUpperCase">if set to <c>true</c> [is upper case].</param>
        /// <param name="isLowerCase">if set to <c>true</c> [is lower case].</param>
        /// <returns></returns>
        private static string TransformName(string name, string wordsSeparator = null, bool isCamelCase = true, bool isUpperCase = false, bool isLowerCase = false)
        {
            var words = GetWords(name);

            StringBuilder transformedName = new();

            var index = 0;
            foreach (var word in words)
            {
                if (isCamelCase)
                    transformedName.Append(word[..1].ToUpper() + word[1..].ToLower());
                else if (isUpperCase)
                    transformedName.Append(word.ToUpper());
                else if (isLowerCase)
                    transformedName.Append(word.ToLower());
                else
                    transformedName.Append(word);

                index++;
                if (index < words.Count && !string.IsNullOrEmpty(wordsSeparator))
                    transformedName.Append(wordsSeparator);
            }

            return transformedName.ToString();
        }

        /// <summary>Gets the words.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static List<string> GetWords(string name)
        {
            List<string> camelCaseWords = new();

            var words = name.Split(new char[] { '_', ' ' });
            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }

                foreach (Match match in regexCamelCase.Matches(word))
                    camelCaseWords.Add(match.Groups["word"].Value);
            }

            return camelCaseWords;
        }


        /// <summary>Cleans the name.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static string CleanName(this string name)
        {
            name = name.Replace(" ", "_").Replace("-", "_").Trim('_');
            if (name.Length > 0 && '0' <= name[0] && name[0] <= '9')
                name = "_" + name;
            return name;
        }

    }
}
