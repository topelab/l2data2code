using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Interface
{
    public interface ISchemaReader
    {
        Tables ReadSchema(SchemaReaderOptions schemaReaderOptions);
        void WriteLine(string msg);
        void HasErrorMessage(bool setError);
        bool HasErrorMessage();
        bool CanConnect(bool includeCommentServer = false);
    }
}
