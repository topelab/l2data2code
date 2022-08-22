using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Extensions;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Fake
{
    public class FakeSchemaReader : Schema.SchemaReader
    {
        private readonly INameResolver nameResolver;

        public FakeSchemaReader(INameResolver nameResolver, ISchemaOptions options) : base(options.SummaryWriter)
        {
            this.nameResolver = nameResolver ?? throw new System.ArgumentNullException(nameof(nameResolver));
            this.nameResolver.Initialize(options.SchemaName);
        }

        public override Tables ReadSchema(SchemaReaderOptions options)
        {
            Tables result = new();
            AddFakeTable("first_table", result, options.RemoveFirstWord);
            AddFakeTable("second_table", result, options.RemoveFirstWord);

            return result;
        }

        private void AddFakeTable(string tableName, Tables tables, bool removeFirstWord)
        {
            Table tbl = new()
            {
                Name = tableName
            };

            tbl.CleanName = RemoveTablePrefixes(nameResolver.ResolveTableName(tbl.Name)).PascalCamelCase(false);
            tbl.ClassName = tbl.CleanName.ToSingular();
            tbl.Description = $"Description for {tbl.ClassName}";

            tbl.Columns = LoadFakeColumns(tbl, removeFirstWord);

            tables.Add(tbl.Name, tbl);
        }

        private static List<Column> LoadFakeColumns(Table tbl, bool removeFirstWord = true)
        {
            List<Column> result = new()
            {
                new Column
                {
                    Table = tbl,
                    TableName = tbl.Name,
                    Name = "id",
                    PropertyName = "id".PascalCamelCase(removeFirstWord),
                    PropertyType = "int",
                    IsPK = true,
                    IsAutoIncrement = true,
                    Precision = 10,
                    PkOrder = 1,
                    Description = "Id for the table"
                },
                new Column
                {
                    Table = tbl,
                    TableName = tbl.Name,
                    Name = "name",
                    PropertyName = "name".PascalCamelCase(removeFirstWord),
                    PropertyType = "string",
                    Precision = 50,
                    Description = "String(50) column"
                },
                new Column
                {
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
            if (cleanword.StartsWith("tbl_"))
            {
                cleanword = cleanword.Replace("tbl_", "");
            }

            if (cleanword.StartsWith("tbl"))
            {
                cleanword = cleanword.Replace("tbl", "");
            }

            return cleanword;
        }

    }

}
