namespace L2Data2Code.BaseGenerator.Entities
{
    public class EntityColumn
    {
        public string Table { get; set; }
        public string Name { get; set; }
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



        public string GetDefaultValue()
        {
            switch (Type)
            {
                case Constants.DataBase.Binary:
                case Constants.DataBase.Varbinary:
                case Constants.DataBase.Image:
                    return "null";

                case Constants.DataBase.Date:
                case Constants.DataBase.Datetime:
                case Constants.DataBase.Timestamp:
                    return IsNull ? "null" : "new DateTime(1900,1,1)";

                case Constants.DataBase.Time:
                    return IsNull ? "null" : "TimeSpan.Zero";

                case Constants.DataBase.Bit:
                    return IsNull ? "null" : "false";

                case Constants.DataBase.Bigint:
                case Constants.DataBase.Int:
                case Constants.DataBase.Smallint:
                case Constants.DataBase.Tinyint:
                case Constants.DataBase.Money:
                case Constants.DataBase.Numeric:
                case Constants.DataBase.Float:
                case Constants.DataBase.Decimal:
                    return IsNull ? "null" : "0";

                case Constants.DataBase.Char:
                case Constants.DataBase.Text:
                case Constants.DataBase.Varchar:
                case Constants.DataBase.Nchar:
                case Constants.DataBase.Nvarchar:
                case Constants.DataBase.Ntext:
                    return IsNull ? "null" : "string.Empty";

                default:
                    return "null";
            }

        }

        public string GetCSharpType()
        {
            switch (Type)
            {
                case Constants.DataBase.Binary:
                case Constants.DataBase.Varbinary:
                case Constants.DataBase.Image:
                    return "byte[]";

                case Constants.DataBase.Date:
                case Constants.DataBase.Datetime:
                case Constants.DataBase.Timestamp:
                    return "DateTime" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Time:
                    return "TimeSpan" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Bit:
                    return "bool" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Money:
                case Constants.DataBase.Numeric:
                case Constants.DataBase.Decimal:
                    return "decimal" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Float:
                    return "double" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Bigint:
                    return "long" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Int:
                    return "int" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Smallint:
                    return "short" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Tinyint:
                    return "byte" + (IsNull ? "?" : string.Empty);

                case Constants.DataBase.Char:
                case Constants.DataBase.Text:
                case Constants.DataBase.Varchar:
                case Constants.DataBase.Nchar:
                case Constants.DataBase.Nvarchar:
                case Constants.DataBase.Ntext:
                    return "string";

                default:
                    return Type;
            }

        }

    }

}
