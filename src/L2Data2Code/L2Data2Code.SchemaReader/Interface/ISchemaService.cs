using L2Data2Code.SchemaReader.Schema;
using System.Collections.Generic;

namespace L2Data2Code.SchemaReader.Interface
{
    /// <summary>
    /// Schema service
    /// </summary>
    public interface ISchemaService
    {
        /// <summary>
        /// Read schema info specified at options
        /// </summary>
        /// <param name="options">Schema options</param>
        /// <param name="alternativeDescriptions">Dictionary with alternative descriptions</param>
        /// <returns>Specific dictionary of tables</returns>
        Tables Read(ISchemaOptions options, Dictionary<string, string> alternativeDescriptions = null);
        /// <summary>
        /// Get language for schema name
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        string GetLang(string schemaName);
        /// <summary>
        /// Should remove first word on table name's?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        bool ShouldRemoveWord1(string schemaName);
        /// <summary>
        /// Use normalized names?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        bool NormalizedNames(string schemaName);
        /// <summary>
        /// Can create DB?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        bool CanCreateDB(string schemaName);
        /// <summary>
        /// Get schema dictionary from file
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        Dictionary<string, string> GetSchemaDictionaryFromFile(string schemaName);
        /// <summary>
        /// Get connection sting
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        (string ConnectionString, string Provider) GetConnectionString(string schemaName);
        /// <summary>
        /// Generate JSON file with schema info
        /// </summary>
        /// <param name="processTables">Tables to process</param>
        /// <param name="outputFileName">Output file name (full path) for JSON file</param>
        void GenerateJsonInfo(IEnumerable<Table> processTables, string outputFileName);
    }
}