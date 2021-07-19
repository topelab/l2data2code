using L2Data2Code.SchemaReader.Interface;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Schema
{
    public class SchemaReaderOptions
    {
        public Regex TableRegex { get; set; }
        public bool RemoveFirstWord { get; set; }
        public Dictionary<string, string> AlternativeDescriptions { get; set; }
        public INameResolver NameResolver { get; set; }

        public SchemaReaderOptions(Regex tableRegex, bool removeFirstWord, Dictionary<string, string> alternativeDescriptions, INameResolver nameResolver)
        {
            TableRegex = tableRegex;
            RemoveFirstWord = removeFirstWord;
            AlternativeDescriptions = alternativeDescriptions;
            NameResolver = nameResolver;
        }
        public SchemaReaderOptions(bool removeFirstWord, Dictionary<string, string> alternativeDescriptions, INameResolver nameResolver) : this(null, removeFirstWord, alternativeDescriptions, nameResolver)
        {

        }
        public SchemaReaderOptions(Regex tableRegex, bool removeFirstWord, Dictionary<string, string> alternativeDescriptions) : this(tableRegex, removeFirstWord, alternativeDescriptions, null)
        {

        }
        public SchemaReaderOptions(Regex tableRegex, bool removeFirstWord) : this(tableRegex, removeFirstWord, null, null)
        {

        }
        public SchemaReaderOptions(Regex tableRegex) : this(tableRegex, false, null, null)
        {

        }
        public SchemaReaderOptions() : this(null, false, null, null)
        {

        }
    }
}
