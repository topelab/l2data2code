namespace L2Data2Code.SchemaReader.Schema
{
    /// <summary>
    /// Column
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        public Table Table { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public string PropertyType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pk.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is pk; otherwise, <c>false</c>.
        /// </value>
        public bool IsPK { get; set; }

        /// <summary>
        /// Gets or sets the pk order.
        /// </summary>
        /// <value>
        /// The pk order.
        /// </value>
        public int PkOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is nullable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is automatic increment.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is automatic increment; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoIncrement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is computed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is computed; otherwise, <c>false</c>.
        /// </value>
        public bool IsComputed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Column"/> is ignore.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ignore; otherwise, <c>false</c>.
        /// </value>
        public bool Ignore { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the precision, lenth of column
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public int Precision { get; set; }

        /// <summary>
        /// Gets or sets the numeric scale, number of decimals for a numeric type.
        /// </summary>
        /// <value>
        /// The numeric scale.
        /// </value>
        public int NumericScale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is numeric.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is numeric; otherwise, <c>false</c>.
        /// </value>
        public bool IsNumeric { get; set; }

        /// <summary>
        /// Default value for column
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter by.
        /// </summary>
        public bool IsFilter { get; set; }

        /// <summary>
        /// Gets or sets the filter type.
        /// </summary>
        public string FilterType { get; internal set; }

        /// <summary>
        /// Gets or sets the filter specification.
        /// </summary>
        public string FilterSpecification { get; internal set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName => $"{TableName}.{Name}";

        /// <summary>
        /// Gets the full name with owner.
        /// </summary>
        /// <value>
        /// The full name with owner.
        /// </value>
        public string FullNameWithOwner => $"{Owner}.{FullName}";
    }

}
