using L2Data2Code.SharedLib.Extensions;
using System;

namespace L2Data2Code.BaseGenerator.Entities
{
    public class EntityColumn
    {
        public string Table { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Type { get; set; }
        public bool IsNull { get; set; }
        public int Size { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool IsNumeric { get; set; }
        public bool PrimaryKey { get; set; }
        public string Description { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsCollection { get; set; }
        public string ColumnName { get; set; }
        public int PkOrder { get; internal set; }
        public bool IsAutoIncrement { get; internal set; }
        public bool IsComputed { get; internal set; }
        public string Join { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
        public string DbJoin { get; set; }
        public string DbFromField { get; set; }
        public string DbToField { get; set; }
        public string DefaultValue { get; set; }
        public bool HasDefaultValue => !string.IsNullOrWhiteSpace(DefaultValue);
        public bool HasRelation { get; set; }

        public string GetDefaultValue()
        {
            return Type switch
            {
                "DateTime?" => HasDefaultValue ? DefaultValue : "null",
                "DateTime" => HasDefaultValue ? DefaultValue : "new DateTime(1,1,1)",
                "TimeSpan?" => HasDefaultValue ? DefaultValue : "null",
                "TimeSpan" => HasDefaultValue ? DefaultValue : "TimeSpan.Zero",
                "bool?" => HasDefaultValue ? DefaultValue.IsTrue().ToString().ToLower() : "null",
                "bool" => DefaultValue.IsTrue().ToString().ToLower(),

                "int" or "long" or "float"
                or "double" or "decimal" or "short" or "byte"
                or "Int16" or "Int32" or "Int64"
                or "Double" or "Decimal" => HasDefaultValue ? DefaultValue : "0",

                "int?" or "long?" or "float?"
                or "double?" or "decimal?" or "short?" or "byte?"
                or "Int16?" or "Int32?" or "Int64?"
                or "Double?" or "Decimal?" => HasDefaultValue ? DefaultValue : "null",

                "char" => "'\0'",
                "char?" => "null",

                _ => HasDefaultValue ? DefaultValue.StringRepresentation() : "null",
            };
        }

        public string GetInitialValue()
        {
            return Type switch
            {
                nameof(DateTime) => "new DateTime(1,1,1)",
                nameof(TimeSpan) => "TimeSpan.Zero",
                "bool" => "false",

                "int" or "long" or "float"
                or "double" or "decimal" or "short" or "byte"
                or nameof(Int16)
                or nameof(Int32)
                or nameof(Int64)
                or nameof(Double)
                or nameof(Decimal) => "0",

                "char" => "'\0'",

                _ => "null",
            };
        }


        public string GetCSharpType()
        {
            return Type switch
            {
                Constants.DataBase.Binary
                or Constants.DataBase.Varbinary
                or Constants.DataBase.Image => "byte[]",

                Constants.DataBase.Date
                or Constants.DataBase.Datetime
                or Constants.DataBase.Timestamp => "DateTime" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Time => "TimeSpan" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Bit => "bool" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Money
                or Constants.DataBase.Numeric
                or Constants.DataBase.Decimal => "decimal" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Float => "double" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Bigint => "long" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Int => "int" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Smallint => "short" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Tinyint => "byte" + (IsNull ? "?" : string.Empty),

                Constants.DataBase.Char
                or Constants.DataBase.Text
                or Constants.DataBase.Varchar
                or Constants.DataBase.Nchar
                or Constants.DataBase.Nvarchar
                or Constants.DataBase.Ntext => "string",

                _ => Type,
            };
        }

    }

}
