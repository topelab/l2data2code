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
        /// General label
        /// </summary>
        public const string GeneralResourceFolder = "General";

        /// <summary>
        /// Internal types constants
        /// </summary>
        public class InternalTypes
        {
            public const string ReferenceTo = "=";
            public const string Collection = "*";
        }

        private static readonly string[] nullabeTypes = { "bool", "char", "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "DateTime", "TimeSpan", "DateOnly", "TimeOnly" };

        /// <summary>
        /// Nullable types constants
        /// </summary>
        public static string[] NullableTypes { get => nullabeTypes; }
    }

}
