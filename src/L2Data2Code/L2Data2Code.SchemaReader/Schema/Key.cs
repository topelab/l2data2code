namespace L2Data2Code.SchemaReader.Schema
{
    /// <summary>
    /// Key
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        public Column ColumnReferenced { get; set; }
        public Column ColumnReferencing { get; set; }

        private string _referenced;
        public string Referenced
        {
            get => _referenced ?? $"{ColumnReferenced.Table.Name}.{ColumnReferenced.Name}";
            set => _referenced = value;
        }

        private string _referencing;
        public string Referencing
        {
            get => _referencing ?? $"{ColumnReferencing.Table.Name}.{ColumnReferencing.Name}";
            set => _referencing = value;
        }
            
    }

}
