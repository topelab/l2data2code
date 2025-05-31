using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Scheme2Json
{
    /// <summary>
    /// Args options
    /// </summary>
    internal class Options
    {
        /// <summary>
        /// Output path
        /// </summary>
        [Option('o', "output", Required = false, HelpText = "Set output path")]
        public string OutputPath { get; set; }

        /// <summary>
        /// Schema to read
        /// </summary>
        [Option('s', "schema", Required = true, HelpText = "Set schema name to be read")]
        public string Schema { get; set; }

        /// <summary>
        /// Examples
        /// </summary>
        [Usage(ApplicationAlias = "Schema2Json")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Normal scenario", new Options { Schema = "demo" });
            }
        }
    }
}
