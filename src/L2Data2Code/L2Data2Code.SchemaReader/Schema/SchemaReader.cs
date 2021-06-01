using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Schema
{
    public abstract class SchemaReader : ISchemaReader
    {
        private readonly StringBuilderWriter _outputWriter;
        public abstract Tables ReadSchema(Regex tableRegex = null, bool removeFirstWord = false, Dictionary<string, string> alternativeDescriptions = null, INameResolver resolver = null);
        protected SchemaReader(StringBuilderWriter outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public void WriteLine(string msg)
        {
            _outputWriter.WriteLine(msg);
        }

        public void HasErrorMessage(bool setError)
        {
            _outputWriter.ContainsErrorMessage = setError;
        }

        public bool HasErrorMessage() => _outputWriter.ContainsErrorMessage;

        public virtual bool CanConnect(bool includeCommentServer = false) => true;

    }

}
