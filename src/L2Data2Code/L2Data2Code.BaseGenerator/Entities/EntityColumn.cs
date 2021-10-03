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
            return Type switch
            {
                Constants.DataBase.Binary or Constants.DataBase.Varbinary or Constants.DataBase.Image => "null",
                Constants.DataBase.Date or Constants.DataBase.Datetime or Constants.DataBase.Timestamp => IsNull ? "null" : "new DateTime(1900,1,1)",
                Constants.DataBase.Time => IsNull ? "null" : "TimeSpan.Zero",
                Constants.DataBase.Bit => IsNull ? "null" : "false",
                Constants.DataBase.Bigint or Constants.DataBase.Int or Constants.DataBase.Smallint or Constants.DataBase.Tinyint or Constants.DataBase.Money or Constants.DataBase.Numeric or Constants.DataBase.Float or Constants.DataBase.Decimal => IsNull ? "null" : "0",
                Constants.DataBase.Char or Constants.DataBase.Text or Constants.DataBase.Varchar or Constants.DataBase.Nchar or Constants.DataBase.Nvarchar or Constants.DataBase.Ntext => IsNull ? "null" : "string.Empty",
                _ => "null",
            };
        }

        public string GetCSharpType()
        {
            return Type switch
            {
                Constants.DataBase.Binary or Constants.DataBase.Varbinary or Constants.DataBase.Image => "byte[]",
                Constants.DataBase.Date or Constants.DataBase.Datetime or Constants.DataBase.Timestamp => "DateTime" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Time => "TimeSpan" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Bit => "bool" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Money or Constants.DataBase.Numeric or Constants.DataBase.Decimal => "decimal" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Float => "double" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Bigint => "long" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Int => "int" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Smallint => "short" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Tinyint => "byte" + (IsNull ? "?" : string.Empty),
                Constants.DataBase.Char or Constants.DataBase.Text or Constants.DataBase.Varchar or Constants.DataBase.Nchar or Constants.DataBase.Nvarchar or Constants.DataBase.Ntext => "string",
                _ => Type,
            };
        }

    }

}
