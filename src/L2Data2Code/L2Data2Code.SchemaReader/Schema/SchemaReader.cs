using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    public abstract class SchemaReader(StringBuilderWriter outputWriter) : ISchemaReader
    {
        public abstract Tables ReadSchema(SchemaReaderOptions schemaReaderOptions);

        public void WriteLine(string msg)
        {
            outputWriter.WriteLine(msg);
        }

        public void HasErrorMessage(bool setError)
        {
            outputWriter.ContainsErrorMessage = setError;
        }

        public bool HasErrorMessage() => outputWriter.ContainsErrorMessage;

        public virtual bool CanConnect() => true;

        protected void TrySetFilterColumn(Dictionary<string, Configuration.ColumnFilter> filteredColumns, Column column)
        {
            if (filteredColumns.TryGetValue(column.Name, out var filter))
            {
                column.IsFilter = true;
                column.FilterType = filter.FilterType;
                column.FilterSpecification = filter.FilterSpecification;
            }
            else
            {
                column.IsFilter = false;
                column.FilterType = null;
                column.FilterSpecification = null;
            }
        }
    }

}
