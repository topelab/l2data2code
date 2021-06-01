using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface ISchemaReader
    {
        Tables ReadSchema(Regex tableRegex = null, bool removeFirstWord = false, Dictionary<string, string> alternativeDescriptions = null, INameResolver resolver = null);
        void WriteLine(string msg);
        void HasErrorMessage(bool setError);
        bool HasErrorMessage();
        bool CanConnect(bool includeCommentServer = false);
    }
}
