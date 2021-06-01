using L2Data2Code.SharedLib.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace L2Data2Code.BaseGenerator.Entities
{
    public static class Config
    {
        private static readonly string DefaultLang = "en";
        private static readonly bool Remove1stDefaultValue = false;

        public static string GetLang(string connectionName)
        {
            return ConfigHelper.Schemas[connectionName]?.TableNameLanguage ?? DefaultLang;
        }

        public static bool ShouldRemoveWord1(string connectionName)
        {
            return ConfigHelper.Schemas[connectionName]?.RemoveFirstWordOnColumnNames ?? Remove1stDefaultValue;
        }

        public static bool NormalizedNames(string connectionName)
        {
            return ConfigHelper.Schemas[connectionName]?.NormalizedNames ?? false;
        }

        public static bool CanCreateDB(string connectionName)
        {
            return ConfigHelper.Schemas[connectionName]?.CanCreateDB ?? false;
        }

        public static Dictionary<string, string> GetSchemaDictionaryFromFile(string connectionName)
        {
            var schemaDictionary = new Dictionary<string, string>();
            var descriptionFile = ConfigHelper.Schemas[connectionName]?.DescriptionsFile;
            if (descriptionFile == null || !File.Exists(descriptionFile))
            {
                return schemaDictionary;
            }

            var content = File.ReadAllLines(descriptionFile);
            for (int i = 1; i < content.Length; i++)
            {
                string line = content[i].Trim();
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

        public static (string ConnectionString, string Provider) GetConnectionString(string connectionName)
        {
            var schemaInfo = ConfigHelper.Schemas[connectionName];
            string connectionString = schemaInfo?.ConnectionString;
            string provider = schemaInfo?.Provider;
            if (provider.Equals("System.Data.FakeClient") || provider.Equals("System.Data.JsonClient"))
            {
                connectionString = "Data Source=:memory:";
            }

            return (connectionString, provider);
        }

        public static Dictionary<string, string> GetTableRenames(string connectionName)
        {
            var renameTable = new Dictionary<string, string>();
            var renameDescriptions = ConfigHelper.Schemas[connectionName]?.RenameTables;
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

        public static Dictionary<string, string> GetColumnRenames(string connectionName)
        {
            var renameColumn = new Dictionary<string, string>();
            var renameDescriptions = ConfigHelper.Schemas[connectionName]?.RenameColumns;
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
