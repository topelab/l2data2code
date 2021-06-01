using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using L2Data2Code.SharedLib.Extensions;

namespace L2Data2Code.SchemaReader.Lib
{
    /// <summary>Name Helper</summary>
    public static class NameHelper
    {
        #region Singular & Plural

        /// <summary>The regex camel case</summary>
        private static readonly Regex regexCamelCase = new Regex("(?<word>[A-Z]{2,}|[A-Z][^A-Z]*?|^[^A-Z]*?)(?=[A-Z]|$)", RegexOptions.Compiled);

        /// <summary>Gets the name of the singular.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetSingularName(string name)
        {
            return WordQuantifier(name, word => word.ToSingular());
        }

        /// <summary>Gets the name of the plural.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetPluralName(string name)
        {
            return WordQuantifier(name, word => word.ToPlural());
        }

        /// <summary>Words the quantifier.</summary>
        /// <param name="name">The name.</param>
        /// <param name="quantifier">The quantifier.</param>
        /// <returns></returns>
        private static string WordQuantifier(string name, Func<string, string> quantifier)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            string word = name;
            int index = 0;

            if (word.Length > 0 && word[word.Length - 1] != '_')
            {
                int index1 = word.LastIndexOf('_');
                if (index1 != -1)
                {
                    index = index1 + 1;
                    word = word.Substring(index);
                }
            }

            Match match = regexCamelCase.Matches(word).Cast<Match>().LastOrDefault();
            if (match != null)
            {
                word = match.Groups["word"].Value;
                index += match.Groups["word"].Index;
            }

            string quantifiedWord = quantifier(word);

            if (quantifiedWord == word)
                return name;

            if (name.Length == word.Length)
                return quantifiedWord;

            return name.Substring(0, index) + quantifiedWord;
        }

        #endregion

        #region Transform Name

        /// <summary>Removes the word1.</summary>
        /// <param name="name">The name.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <returns></returns>
        public static string RemoveWord1(this string name, bool remove)
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
        public static string TransformName(string name, string wordsSeparator = null, bool isCamelCase = true, bool isUpperCase = false, bool isLowerCase = false)
        {
            List<string> words = GetWords(name);

            StringBuilder transformedName = new StringBuilder();

            int index = 0;
            foreach (string word in words)
            {
                if (isCamelCase)
                    transformedName.Append(word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower());
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
        public static List<string> GetWords(string name)
        {
            List<string> camelCaseWords = new List<string>();

            string[] words = name.Split(new char[] { '_', ' ' });
            foreach (string word in words)
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

        #endregion

        #region Clean Name

        /// <summary>Cleans the name.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string CleanName(this string name)
        {
            name = name.Replace(" ", "_").Replace("-", "_").Trim('_');
            if (name.Length > 0 && '0' <= name[0] && name[0] <= '9')
                name = "_" + name;
            return name;
        }

        /// <summary>Cleans up.</summary>
        /// <param name="str">The string.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <returns></returns>
        public static string PascalCamelCase(this string str, bool remove) => TransformName(str.RemoveWord1(remove).CleanName(), null, true);

        #endregion

        #region Name Prefix

        /// <summary>The name prefixes</summary>
        private static readonly List<string> NamePrefixes = new List<string>() {
            "first",
            "second",
            "third",
            "fourth",
            "fifth",
            "sixth",
            "seventh",
            "eighth",
            "ninth",
            "tenth",
            "eleventh",
            "twelfth",
            "primary",
            "secondary",
            "tertiary",
            "quaternary",
            "quinary",
            "senary",
            "septenary",
            "octonary",
            "novenary",
            "decenary",
            "undenary",
            "duodenary",
            "current",
            "previous",
            "initial",
            "starting",
            "last",
            "ending"
        };

        /// <summary>Adds the name prefix.</summary>
        /// <param name="name">The name.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string AddNamePrefix(string name, string columnName)
        {
            string columnNameLower = columnName.ToLower();
            string prefix = NamePrefixes.OrderByDescending(p => p.Length).FirstOrDefault(p => columnNameLower.StartsWith(p));
            if (!string.IsNullOrEmpty(prefix))
                name = columnName.Substring(0, prefix.Length) + name;
            return name;
        }

        #endregion

        #region Name Verbs

        /// <summary>Conjugated Verb</summary>
        private class ConjugatedVerb
        {
            /// <summary>Gets or sets the verb.</summary>
            /// <value>The verb.</value>
            public string Verb { get; set; }
            /// <summary>Gets or sets the past participle verb.</summary>
            /// <value>The past participle verb.</value>
            public string PastParticipleVerb { get; set; }
            /// <summary>Gets or sets the verb variations.</summary>
            /// <value>The verb variations.</value>
            public List<string> VerbVariations { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConjugatedVerb"/> class.
            /// </summary>
            /// <param name="verb">The verb.</param>
            /// <param name="pastParticipleVerb">The past participle verb.</param>
            public ConjugatedVerb(string verb, string pastParticipleVerb)
            {
                this.Verb = verb;
                this.PastParticipleVerb = pastParticipleVerb;
                this.VerbVariations = new List<string>();
            }
        }

        /// <summary>The conjugated verbs</summary>
        private static readonly List<ConjugatedVerb> _conjugatedVerbs = new List<ConjugatedVerb>()
        {
            new ConjugatedVerb("insert", "inserted"),
            new ConjugatedVerb("update", "updated"),
            new ConjugatedVerb("delete", "deleted"),
            new ConjugatedVerb("create", "created"),
            new ConjugatedVerb("write", "written"),
            new ConjugatedVerb("ship", "shipped"),
            new ConjugatedVerb("send", "sent"),
        };

        /// <summary>The verb variations</summary>
        private static readonly List<string> _verbVariations = new List<string>()
        {
            "{0}",
            "{0}by",
            "{0}_by",
            "{0}id",
            "{0}_id",
            "user{0}",
            "user_{0}",
            "{0}user",
            "{0}_user",
            "person{0}",
            "person_{0}",
            "{0}person",
            "{0}_person"
        };

        /// <summary>The variations</summary>
        private static List<string> _variations;

        /// <summary>Initializes the <see cref="NameHelper"/> class.</summary>
        static NameHelper()
        {
            BuildNameVerbsAndVariations();
        }

        /// <summary>Builds the name verbs and variations.</summary>
        private static void BuildNameVerbsAndVariations()
        {
            foreach (var conjugations in _conjugatedVerbs)
            {
                foreach (var variation in _verbVariations)
                {
                    conjugations.VerbVariations.Add(string.Format(variation, conjugations.Verb));
                    conjugations.VerbVariations.Add(string.Format(variation, conjugations.PastParticipleVerb));
                }

                // order length descending
                conjugations.VerbVariations.Sort((x, y) => x.LengthCompare(y));
            }

            _variations = _conjugatedVerbs.SelectMany(p => p.VerbVariations).ToList();

            // order length descending
            _variations.Sort((x, y) => x.LengthCompare(y));
        }

        /// <summary>
        /// Determines whether [is name verb] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if [is name verb] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNameVerb(string name)
        {
            bool hasTime = (name.IndexOf("time", StringComparison.CurrentCultureIgnoreCase) != -1);
            if (hasTime)
                return false;

            bool hasDate = (name.IndexOf("date", StringComparison.CurrentCultureIgnoreCase) != -1);
            bool hasShip = (name.IndexOf("ship", StringComparison.CurrentCultureIgnoreCase) != -1);
            if (!hasDate && !hasShip)
                return _variations.Any(variation => name.IndexOf(variation, StringComparison.CurrentCultureIgnoreCase) != -1);

            if (hasDate)
            {
                bool hasUpdate = (name.IndexOf("update", StringComparison.CurrentCultureIgnoreCase) != -1);
                if (!hasUpdate)
                    return false;

                int index = name.IndexOf("date", StringComparison.CurrentCultureIgnoreCase);
                do
                {
                    hasUpdate =
                        (index - 2) >= 0 &&
                        name.IndexOf("update", index - 2, StringComparison.CurrentCultureIgnoreCase) == (index - 2);

                    if (!hasUpdate)
                        return false;

                    index = name.IndexOf("date", index + 4, StringComparison.CurrentCultureIgnoreCase);
                }
                while (index != -1);
            }

            if (hasShip)
            {
                bool hasShipment = (name.IndexOf("shipment", StringComparison.CurrentCultureIgnoreCase) != -1);
                if (hasShipment)
                    return false;
            }

            return true;
        }

        /// <summary>Conjugates the name verb to past participle.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string ConjugateNameVerbToPastParticiple(string name)
        {
            ConjugatedVerb conjugations = _conjugatedVerbs.FirstOrDefault(cv => cv.VerbVariations.Any(variation => name.IndexOf(variation, StringComparison.CurrentCultureIgnoreCase) != -1));

            if (conjugations == null)
                return name;

            // verb past participle
            if (name.IndexOf(conjugations.PastParticipleVerb, StringComparison.CurrentCultureIgnoreCase) != -1)
                return name;

            // verb
            int index = name.IndexOf(conjugations.Verb, StringComparison.CurrentCultureIgnoreCase);
            if (index != -1)
                return conjugations.PastParticipleVerb.Substring(0, 1).ToUpper() + conjugations.PastParticipleVerb.Substring(1).ToLower() + "By";

            return name;
        }

        #endregion
    }
}
