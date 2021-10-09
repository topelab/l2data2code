using L2Data2Code.SharedLib.Extensions;
using System;
using System.Text;

namespace L2Data2Code.SchemaReader.Lib
{
    /// <summary>String Builder Writer</summary>
    public class StringBuilderWriter
    {
        private readonly StringBuilder outputStringBuilder;

        /// <summary>Gets the output string builder.</summary>
        /// <value>The output string builder.</value>
        public string OutputStringBuilder { get => outputStringBuilder.ToString(); }
        /// <summary>
        /// Gets or sets a value indicating whether [contains error message].
        /// </summary>
        /// <value><c>true</c> if [contains error message]; otherwise, <c>false</c>.</value>
        public bool ContainsErrorMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuilderWriter"/> class.
        /// </summary>
        public StringBuilderWriter()
        {
            ContainsErrorMessage = false;
            outputStringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuilderWriter"/> class.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        public StringBuilderWriter(StringBuilder stringBuilder)
        {
            ContainsErrorMessage = false;
            outputStringBuilder = stringBuilder;
        }

        /// <summary>Writes the line.</summary>
        /// <param name="o">The o.</param>
        public void WriteLine(string o)
        {
            WriteLine(0, o);
        }

        /// <summary>Writes the line.</summary>
        /// <param name="indent">The indent.</param>
        /// <param name="o">The o.</param>
        public void WriteLine(int indent, string o)
        {
            outputStringBuilder.AddTabs(indent);
            o = o.ReplaceEndOfLine(Environment.NewLine);
            outputStringBuilder.AppendLine(o);
        }

        /// <summary>Clears this instance.</summary>
        public void Clear()
        {
            ContainsErrorMessage = false;
            outputStringBuilder.Clear();
        }

    }
}
