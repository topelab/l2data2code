using System.Collections.Generic;
using System.Linq;

namespace L2Data2Code.SchemaReader.Schema
{
    /// <summary>Table</summary>
    public class Table
    {
        /// <summary>
        /// Source where table was originally created
        /// </summary>
        public string SourceDB { get; set; }
        /// <summary>Gets the columns.</summary>
        /// <value>The columns.</value>
        public List<Column> Columns { get; set; }
        /// <summary>
        /// Indexes for tables
        /// </summary>
        public List<Index> Indexes { get; set; } = new();
        /// <summary>Gets the inner keys.</summary>
        /// <value>The inner keys.</value>
        public List<Key> InnerKeys { get; set; } = new();
        /// <summary>Gets the outer keys.</summary>
        /// <value>The outer keys.</value>
        public List<Key> OuterKeys { get; set; } = new();
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>Gets or sets the schema.</summary>
        /// <value>The schema.</value>
        public string Schema { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is view.
        /// </summary>
        /// <value><c>true</c> if this instance is view; otherwise, <c>false</c>.</value>
        public bool IsView { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this table or view is updatable.
        /// Tables are always updatable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is updatable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdatable { get; set; } = true;
        /// <summary>Gets or sets the name of the clean.</summary>
        /// <value>The name of the clean.</value>
        public string CleanName { get; set; }
        /// <summary>Gets or sets the name of the class.</summary>
        /// <value>The name of the class.</value>
        public string ClassName { get; set; }
        /// <summary>Gets or sets the name of the sequence.</summary>
        /// <value>The name of the sequence.</value>
        public string SequenceName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Table"/> is ignore.
        /// </summary>
        /// <value><c>true</c> if ignore; otherwise, <c>false</c>.</value>
        public bool Ignore { get; set; }
        /// <summary>Gets or sets the description.</summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Table type
        /// </summary>
        public string Type { get; set; }

        /// <summary>Gets the pk.</summary>
        /// <value>The pk.</value>
        public IEnumerable<Column> PK => Columns.Where(x => x.IsPK).OrderBy(x => x.PkOrder);

        /// <summary>Gets the column.</summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Column GetColumn(string columnName)
        {
            return Columns.Single(x => string.Compare(x.Name, columnName, true) == 0);
        }

        /// <summary>Gets the <see cref="Column"/> with the specified column name.</summary>
        /// <value>The <see cref="Column"/>.</value>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Column this[string columnName]
        {
            get
            {
                return GetColumn(columnName);
            }
        }

    }

}
