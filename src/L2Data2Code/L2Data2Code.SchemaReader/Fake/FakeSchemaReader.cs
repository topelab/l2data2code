using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Fake
{
    public class FakeSchemaReader : Schema.SchemaReader
    {
        private INameResolver _resolver;

        public FakeSchemaReader(SchemaOptions options) : base(options.SummaryWriter)
        {
        }

        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            _resolver = options.NameResolver ?? new DefaultNameResolver();
            var result = new Tables();
            AddFakeTable("first_table", result, options.RemoveFirstWord);
            AddFakeTable("second_table", result, options.RemoveFirstWord);

            return result;
        }

        private void AddFakeTable(string tableName, Tables tables, bool removeFirstWord)
        {
            var tbl = new Table
            {
                Name = tableName
            };

            tbl.CleanName = RemoveTablePrefixes(_resolver.ResolveTableName(tbl.Name)).PascalCamelCase(false);
            tbl.ClassName = tbl.CleanName.ToSingular();
            tbl.Description = $"Description for {tbl.ClassName}";

            tbl.Columns = LoadFakeColumns(tbl, removeFirstWord);

            tables.Add(tbl.Name, tbl);
        }

        private List<Column> LoadFakeColumns(Table tbl, bool removeFirstWord = true)
        {
            var result = new List<Column>() {
                new Column {
                    Table = tbl,
                    TableName = tbl.Name,
                    Name = "id",
                    PropertyName = "id".PascalCamelCase(removeFirstWord),
                    PropertyType = "int",
                    IsPK = true,
                    IsAutoIncrement = true,
                    Precision = 10,
                    PkOrder = 1,
                    Description = "Id for de table"
                },
                new Column {
                    Table = tbl,
                    TableName = tbl.Name,
                    Name = "name",
                    PropertyName = "name".PascalCamelCase(removeFirstWord),
                    PropertyType = "string",
                    Precision = 50,
                    Description = "String(50) column"
                },
                new Column {
                    Table = tbl,
                    TableName = tbl.Name,
                    Name = "age",
                    PropertyName = "age".PascalCamelCase(removeFirstWord),
                    PropertyType = "int",
                    Precision = 3,
                    Description = "Age column"
                },

            };

            return result;
        }

        private static string RemoveTablePrefixes(string word)
        {
            var cleanword = word;
            if (cleanword.StartsWith("tbl_")) cleanword = cleanword.Replace("tbl_", "");
            if (cleanword.StartsWith("tbl")) cleanword = cleanword.Replace("tbl", "");
            return cleanword;
        }

    }

}
