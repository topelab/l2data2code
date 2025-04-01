using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    public abstract class SchemaReader : ISchemaReader
    {
        private readonly StringBuilderWriter _outputWriter;
        public abstract Tables ReadSchema(SchemaReaderOptions schemaReaderOptions);
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
