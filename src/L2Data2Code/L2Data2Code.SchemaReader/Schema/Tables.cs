using System;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Schema
{
    /// <summary>
    /// Dictionary generalization of tables
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
        /// Gets the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public Table GetTable(string tableName)
        {
            return TryGetValue(tableName, out var table) ? table : null;
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
