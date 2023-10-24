using L2Data2Code.SharedLib.Inflector;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace L2Data2Code.SharedLib.Extensions
{
    public static class StringExtensions
    {
        private const string DEFAULT_ISO_LANG = "en";
        private static readonly IInflector _esInflector = new SpanishInflector();
        private static readonly IInflector _enInflector = new EnglishInflector();

        static StringExtensions()
        {
        }

        public static string CurrentLang { get; set; } = DEFAULT_ISO_LANG;

        public static IInflector Service(string isoLang = null)
        {
            isoLang ??= CurrentLang;
            return isoLang.Equals("es") ? _esInflector : _enInflector;
        }

        public static string ToSingular(this string word)
        {
            return ToSingular(word, CurrentLang);
        }

        public static string ToSingular(this string word, string isoLang)
        {
            if (word.IsEmpty())
            {
                return string.Empty;
            }

            var isUpperWord = (string.Compare(word, word.ToUpper(), false) == 0);
            if (isUpperWord)
            {
                var lowerWord = word.ToLower();
                return Service(isoLang).Singularize(lowerWord).ToUpper();
            }

            return Service(isoLang).Singularize(word);
        }

        public static string ToPlural(this string word)
        {
            return ToPlural(word, CurrentLang);
        }

        public static string ToPlural(this string word, string isoLang)
        {
            if (word.IsEmpty())
            {
                return string.Empty;
            }

            var isUpperWord = (string.Compare(word, word.ToUpper(), false) == 0);
            if (isUpperWord)
            {
                var lowerWord = word.ToLower();
                return Service(isoLang).Pluralize(lowerWord).ToUpper();
            }

            return Service(isoLang).Pluralize(word);
        }

        public static string Pascalize(this string word) => PascalCase(word);

        public static string PascalCase(this string word)
        {
            return Service().Pascalize(word.Replace(" ", "_"));
        }

        public static string ToPascalCleanName(this string phrase)
        {
            var words = phrase.ToSlug().Split('-');
            string result = string.Empty;
            foreach (var word in words)
            {
                result = string.Concat(result, word.PascalCase());
            }
            return result;
        }

        public static string ToCamelCleanName(this string phrase)
        {
            var words = phrase.ToSlug().Split('-');
            string result = string.Empty;
            foreach (var word in words)
            {
                result = string.Concat(result, word.CamelCase());
            }
            return result;
        }

        public static string Camelize(this string word) => CamelCase(word);

        public static string CamelCase(this string word)
        {
            return Service().Camelize(word);
        }

        public static string LowerCase(this string word) => word.ToLower();
        public static string UpperCase(this string word) => word.ToUpper();

        public static string PluralCamelize(this string word) => PluralCamelCase(word);

        public static string PluralCamelCase(this string word)
        {
            var _service = Service();
            return _service.Pluralize(_service.Camelize(word));
        }

        public static string SingularCamelize(this string word)
        {
            var _service = Service();
            return _service.Singularize(_service.Camelize(word));
        }

        public static string Humanize(this string word)
        {
            var _service = Service();
            return _service.Humanize(_service.Underscore(_service.Pascalize(word)));
        }

        public static string HumanizeUnCapitalize(this string word)
        {
            var _service = Service();
            return _service.Uncapitalize(_service.Humanize(_service.Underscore(_service.Pascalize(word))));
        }

        public static string UnCapitalize(this string word)
        {
            return Service().Uncapitalize(word);
        }

        public static string UnderScore(this string word)
        {
            var _service = Service();
            return _service.Underscore(_service.Pascalize(word));
        }

        public static string ToTabs(this int indent)
        {
            StringBuilder result = new();
            return result.AddTabs(indent).ToString();
        }

        public static StringBuilder AddTabs(this StringBuilder output, int indent)
        {
            for (var i = 0; i < indent; i++)
            {
                output.Append('\t');
            }
            return output;
        }

        public static int LengthCompare(this string item1, string item2)
        {
            var i1 = item1.Length;
            var i2 = item2.Length;

            if (i1 == i2)
            {
                return 0;
            }
            return i1 < i2 ? 1 : -1;
        }

        public static T IfNull<T>(this object camp, T value)
        {
            if (camp == null || Convert.IsDBNull(camp))
            {
                return value;
            }

            return (T)Convert.ChangeType(camp, typeof(T));
        }

        public static bool NotEmpty(this string value)
        {
            return !IsEmpty(value);
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string IfNotEmpty(this string value, string defaultValue)
        {
            return NotEmpty(value) ? defaultValue : string.Empty;
        }

        public static string IfEmpty(this string value, string defaultValue)
        {
            return IsEmpty(value) ? defaultValue : value;
        }

        public static string ReplaceEndOfLine(this string text, string withReplacement = " ")
        {
            if (IsEmpty(text))
            {
                return text;
            }

            return text.Replace("\r\n", "\n").Replace("\n", withReplacement);
        }

        public static string TrimPathSeparator(this string path)
        {
            return path.TrimEnd(Path.DirectorySeparatorChar);
        }

        public static string AddPathSeparator(this string path)
        {
            return path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        }

        public static string AddBuildNumber(this string text) => text.AddDateTime("yy.Mdd");

        public static string AddMonthDay(this string text) => text.AddDateTime("Mdd");

        public static string AddDateTime(this string text, string format)
        {
            var now = DateTime.UtcNow.ToString(format);
            return $"{text}{now}";
        }

        /// <summary>
        /// Returns true if text is "1", "true" or "yes"
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsTrue(this string text)
        {
            if (text.IsEmpty())
            {
                return false;
            }

            var result = text.ToLower().Trim();
            if (result.Equals("1") || result.Equals("true") || result.Equals("yes"))
            {
                return true;
            }
            return false;
        }

        public static bool IsFalse(this string text)
        {
            return !IsTrue(text);
        }

        /// <summary>
        /// Gets de piece of text that correspond to n <paramref name="part"/> (0 based) returning <paramref name="defaultValue"/> if empty
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="part">The n part.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string Piece(this string text, int part, char separator = ',', string defaultValue = "")
        {
            var values = text.Split(separator);
            part = part > values.Length - 1 ? values.Length - 1 : part < 0 ? 0 : part;
            return text.IsEmpty() ? defaultValue : values[part].IfEmpty(defaultValue).Trim();
        }

        /// <summary>
        /// Gets de piece of text that correspond to n <paramref name="start"/> (0 based) returning <paramref name="defaultValue"/> if empty
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="start">The start part.</param>
        /// <param name="end">The end part</param>
        /// <param name="separator">The separator.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string Piece(this string text, int start, int end, char separator = ',', string defaultValue = "")
        {
            var values = text.Split(separator);
            start = start > values.Length - 1 ? values.Length - 1 : start < 0 ? 0 : start;
            end = end > values.Length - 1 ? values.Length - 1 : end < 0 ? 0 : end;
            return text.IsEmpty() ? defaultValue : string.Join(separator, values[start..(end + 1)]).IfEmpty(defaultValue).Trim();
        }

        /// <summary>
        /// Converts to slug (valid file name)
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string ToSlug(this string fileName) => ToSlug(fileName, string.Empty);

        /// <summary>
        /// Convierte una cadena (fileName) en un nombre válido para el sistema de archivos
        /// </summary>
        /// <param name="fileName">Cadena para convertir</param>
        /// <param name="validCharsToAdd">Caracteres adicicionales</param>
        /// <returns></returns>
        public static string ToSlug(this string fileName, string validCharsToAdd)
        {
            var ValidChars = string.Concat("0123456789abcdefghijklmnopqrstuvwxyz", validCharsToAdd);
            const string ReplaceableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZáéíóúàèìòùñÁÉÍÓÚÀÈÌÒÙÑäëïöüÄËÏÖÜâêîôûÂÊÎÔÛÇç";
            const string ReplacingChars = "abcdefghijklmnopqrstuvwxyzaeiouaeiounaeiouaeiounaeiouaeiouaeiouaeioucc";
            const char Separator = '-';

            char CharToAdd;
            var PrevChar = '\0';

            var sCleanUrl = string.Empty;

            if ((fileName ?? string.Empty) != string.Empty)
            {
                while (fileName.Length != 0)
                {
                    var firstChar = fileName[0];
                    if (ValidChars.Contains(firstChar))
                    {
                        CharToAdd = firstChar;
                        PrevChar = CharToAdd;
                    }
                    else
                    {
                        if (ReplaceableChars.Contains(firstChar))
                        {
                            CharToAdd = ReplacingChars[ReplaceableChars.IndexOf(firstChar)];
                            PrevChar = CharToAdd;
                        }
                        else
                        {
                            CharToAdd = Separator;
                            if (PrevChar.Equals(Separator))
                            {
                                CharToAdd = '\0';
                            }
                            PrevChar = Separator;
                        }
                    }

                    if (CharToAdd != 0)
                    {
                        sCleanUrl += CharToAdd;
                    }
                    fileName = fileName.Substring(1);
                }
            }

            while (sCleanUrl.Length > 0 && sCleanUrl[0] == '-')
            {
                sCleanUrl = sCleanUrl[1..];
            }

            var lon = sCleanUrl.Length;
            if (lon > 1)
            {
                while (sCleanUrl[lon - 1] == '-')
                {
                    sCleanUrl = sCleanUrl[..(lon - 1)];
                    lon--;
                    if (lon < 1)
                    {
                        break;
                    }
                }
            }

            return sCleanUrl;
        }

        /// <summary>
        /// Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string HtmlEncode(this string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        /// <summary>
        /// Double slash in path
        /// </summary>
        /// <param name="path">path</param>
        public static string DoubleSlash(this string path)
            => path.Replace(@"\", @"\\");

        public static string RemoveOuter(this string text, char startChar, char? endChar = null)
        {
            var result = text;
            if (!string.IsNullOrWhiteSpace(result))
            {
                endChar ??= startChar;
                while (result.Length > 1 && result.StartsWith(startChar) && result.EndsWith(endChar.Value))
                {
                    result = result[1..^1];
                }
            }
            return result;
        }

        public static string StringRepresentation(this string text) => text == null ? "null" : $"\"{text.DoubleSlash()}\"";
    }
}
