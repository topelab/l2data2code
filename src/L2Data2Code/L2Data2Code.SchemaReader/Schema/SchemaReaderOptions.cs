using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Schema
{
    public class SchemaReaderOptions
    {
        public Regex TableRegex { get; set; }
        public bool RemoveFirstWord { get; set; }
        public Dictionary<string, string> AlternativeDescriptions { get; set; }

        public SchemaReaderOptions(Regex tableRegex, bool removeFirstWord, Dictionary<string, string> alternativeDescriptions)
        {
            TableRegex = tableRegex;
            RemoveFirstWord = removeFirstWord;
            AlternativeDescriptions = alternativeDescriptions;
        }
        public SchemaReaderOptions(bool removeFirstWord, Dictionary<string, string> alternativeDescriptions) : this(null, removeFirstWord, alternativeDescriptions)
        {

        }
        public SchemaReaderOptions(Regex tableRegex, bool removeFirstWord) : this(tableRegex, removeFirstWord, null)
        {

        }
        public SchemaReaderOptions(Regex tableRegex) : this(tableRegex, false, null)
        {

        }
        public SchemaReaderOptions() : this(null, false, null)
        {

        }
    }
}
