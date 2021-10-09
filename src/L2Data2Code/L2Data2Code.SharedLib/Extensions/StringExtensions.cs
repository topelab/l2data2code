using System;
using System.IO;
using System.Text;
using System.Web;
using L2Data2Code.SharedLib.Inflector;

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
                return string.Empty;

            bool isUpperWord = (string.Compare(word, word.ToUpper(), false) == 0);
            if (isUpperWord)
            {
                string lowerWord = word.ToLower();
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
                return string.Empty;

            bool isUpperWord = (string.Compare(word, word.ToUpper(), false) == 0);
            if (isUpperWord)
            {
                string lowerWord = word.ToLower();
                return Service(isoLang).Pluralize(lowerWord).ToUpper();
            }

            return Service(isoLang).Pluralize(word);
        }

        public static string Pascalize(this string word)
        {
            return Service().Pascalize(word);
        }

        public static string Camelize(this string word)
        {
            return Service().Camelize(word);
        }

        public static string PluralCamelize(this string word)
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
            StringBuilder result = new StringBuilder();
            return result.AddTabs(indent).ToString();
        }

        public static StringBuilder AddTabs(this StringBuilder output, int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                output.Append('\t');
            }
            return output;
        }

        public static int LengthCompare(this string item1, string item2)
        {
            int i1 = item1.Length;
            int i2 = item2.Length;

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
            if (text.IsEmpty()) return false;
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
        /// <param name="defaultValue">The default value.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        public static string Piece(this string text, int part, string defaultValue = "", char separator = ',')
        {
            if (text.IsEmpty())
            {
                return defaultValue;
            }

            var values = text.Split(separator);
            if (part < values.Length)
            {
                return values[part].IfEmpty(defaultValue).Trim();
            }

            return defaultValue;
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

            string ValidChars = "0123456789abcdefghijklmnopqrstuvwxyz" + validCharsToAdd;
            const string ReplaceableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZáéíóúàèìòùñÁÉÍÓÚÀÈÌÒÙÑäëïöüÄËÏÖÜâêîôûÂÊÎÔÛÇç";
            const string ReplacingChars = "abcdefghijklmnopqrstuvwxyzaeiouaeiounaeiouaeiounaeiouaeiouaeiouaeioucc";
            const string Separator = "-";

            string CharToAdd;
            string PrevChar = string.Empty;

            string sCleanUrl = string.Empty;

            if ((fileName ?? string.Empty) != string.Empty)
            {
                while (fileName.Length != 0)
                {
                    if (ValidChars.Contains(fileName.Substring(0, 1)))
                    {
                        CharToAdd = fileName.Substring(0, 1);
                        PrevChar = CharToAdd;
                    }
                    else
                    {
                        if (ReplaceableChars.Contains(fileName.Substring(0, 1)))
                        {
                            CharToAdd = ReplacingChars.Substring(ReplaceableChars.IndexOf(fileName.Substring(0, 1)), 1);
                            PrevChar = CharToAdd;
                        }
                        else
                        {
                            CharToAdd = Separator;
                            if (PrevChar.Equals(Separator))
                            {
                                CharToAdd = "";
                            }
                            PrevChar = Separator;
                        }
                    }

                    sCleanUrl += CharToAdd;
                    fileName = fileName[1..];
                }
            }

            while (sCleanUrl.Length > 0 && sCleanUrl[0] == '-')
            {
                sCleanUrl = sCleanUrl[1..];
            }

            int lon = sCleanUrl.Length;
            if (lon > 1)
            {
                while (sCleanUrl[lon - 1] == '-')
                {
                    sCleanUrl = sCleanUrl.Substring(0, lon - 1);
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

    }
}
