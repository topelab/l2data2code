namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// Constants
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Constant for Id
        /// </summary>
        public const string ID = "Id";

        /// <summary>
        /// Database constants
        /// </summary>
        public class DataBase
        {
            public const string Bigint = "bigint";
            public const string Binary = "binary";
            public const string Bit = "bit";
            public const string Blob = "blob";
            public const string Char = "char";
            public const string Date = "date";
            public const string Datetime = "datetime";
            public const string Datetime2 = "datetime2";
            public const string Decimal = "decimal";
            public const string Float = "float";
            public const string Geography = "geography";
            public const string Geometry = "geometry";
            public const string Image = "image";
            public const string Int = "int";
            public const string Nchar = "nchar";
            public const string Nvarchar = "nvarchar";
            public const string Ntext = "ntext";
            public const string Money = "money";
            public const string Numeric = "numeric";
            public const string Real = "real";
            public const string Smalldatetime = "smalldatetime";
            public const string Smallint = "smallint";
            public const string Smallmoney = "smallmoney";
            public const string Text = "text";
            public const string Time = "time";
            public const string Timestamp = "timestamp";
            public const string Tinyint = "tinyint";
            public const string Uniqueidentifier = "uniqueidentifier";
            public const string Varbinary = "varbinary";
            public const string Varchar = "varchar";
        }

        /// <summary>
        /// Internal types constants
        /// </summary>
        public class InternalTypes
        {
            public const string ReferenceTo = "=";
            public const string Collection = "*";
        }

        private static readonly string[] nullabeTypes = { "bool", "char", "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "DateTime", "TimeSpan" };

        /// <summary>
        /// Nullable types constants
        /// </summary>
        public static string[] NullableTypes { get => nullabeTypes; }
    }

}
