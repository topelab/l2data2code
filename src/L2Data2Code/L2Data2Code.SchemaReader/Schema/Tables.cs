using System.Collections.Generic;
using System;

namespace L2Data2Code.SchemaReader.Schema
{
    /// <summary>
    /// Tables
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{System.String, L2Data2Code.SchemaReader.Schema.Table}" />
    [Serializable]
    public class Tables : Dictionary<string, Table>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tables" /> class.
        /// </summary>
        public Tables()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tables"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization information.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected Tables(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public Table GetTable(string tableName)
        {
            return this.ContainsKey(tableName) ? base[tableName] : null;
        }

        /// <summary>
        /// Gets the <see cref="Table" /> with the specified table name.
        /// </summary>
        /// <value>
        /// The <see cref="Table" />.
        /// </value>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public new Table this[string tableName]
        {
            get
            {
                return GetTable(tableName);
            }
        }

    }

}
