using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity;

namespace L2Data2Code.BaseGenerator.Entities
{
    /// <summary>
    /// Configuration matters
    /// </summary>
    public static class Config
    {
        private static readonly string DefaultLang = "en";
        private static readonly bool Remove1stDefaultValue = false;
        private static readonly IBasicConfiguration<SchemaConfiguration> schemas = schemas ?? ContainerManager.Container.Resolve<IBasicConfiguration<SchemaConfiguration>>();

        /// <summary>
        /// Get language for schema name
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public static string GetLang(string schemaName)
        {
            return schemas[schemaName]?.TableNameLanguage ?? DefaultLang;
        }

        /// <summary>
        /// Should remove first word on table name's?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public static bool ShouldRemoveWord1(string schemaName)
        {
            return schemas[schemaName]?.RemoveFirstWordOnColumnNames ?? Remove1stDefaultValue;
        }

        /// <summary>
        /// Use normalized names?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public static bool NormalizedNames(string schemaName)
        {
            return schemas[schemaName]?.NormalizedNames ?? false;
        }

        /// <summary>
        /// Can create DB?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public static bool CanCreateDB(string schemaName)
        {
            return schemas[schemaName]?.CanCreateDB ?? false;
        }

        /// <summary>
        /// Get schema dictionary from file
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public static Dictionary<string, string> GetSchemaDictionaryFromFile(string schemaName)
        {
            Dictionary<string, string> schemaDictionary = new();
            var descriptionFile = schemas[schemaName]?.DescriptionsFile;
            if (descriptionFile == null || !File.Exists(descriptionFile))
            {
                return schemaDictionary;
            }

            var content = File.ReadAllLines(descriptionFile);
            for (var i = 1; i < content.Length; i++)
            {
                var line = content[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var values = line.Split('\t');
                if (values.Length > 1)
                {
                    schemaDictionary.Add(values[0], string.Join("\t", values.Skip(1)));
                }
            }

            return schemaDictionary;
        }

        /// <summary>
        /// Get connection sting
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public static (string ConnectionString, string Provider) GetConnectionString(string schemaName)
        {
            var schemaInfo = schemas[schemaName];
            var connectionString = schemaInfo?.ConnectionString;
            var provider = schemaInfo?.Provider;
            if (provider.Equals("System.Data.FakeClient") || provider.Equals("System.Data.JsonClient"))
            {
                connectionString = "Data Source=:memory:";
            }

            return (connectionString, provider);
        }

        /// <summary>
        /// Get tables renames
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public static Dictionary<string, string> GetTableRenames(string schemaName)
        {
            Dictionary<string, string> renameTable = new();
            var renameDescriptions = schemas[schemaName]?.RenameTables;
            if (renameDescriptions == null)
            {
                return renameTable;
            }
            foreach (var item in renameDescriptions.Split(';'))
            {
                var def = item.Split('=');
                renameTable.Add(def[0].Trim(), def[1].Trim());
            }
            return renameTable;
        }

        /// <summary>
        /// Get columns renames
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public static Dictionary<string, string> GetColumnRenames(string schemaName)
        {
            Dictionary<string, string> renameColumn = new();
            var renameDescriptions = schemas[schemaName]?.RenameColumns;
            if (renameDescriptions == null)
            {
                return renameColumn;
            }
            foreach (var item in renameDescriptions.Split(';'))
            {
                var def = item.Split('=');
                renameColumn.Add(def[0].Trim(), def[1].Trim());
            }
            return renameColumn;
        }
    }
}
