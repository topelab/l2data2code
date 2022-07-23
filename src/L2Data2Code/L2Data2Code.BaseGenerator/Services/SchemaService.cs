using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace L2Data2Code.BaseGenerator.Services
{
    public class SchemaService : ISchemaService
    {
        private readonly string DefaultLang = "en";
        private readonly bool Remove1stDefaultValue = false;

        private readonly ILogger logger;
        private readonly Dictionary<string, Tables> schemaNamesCached;
        private readonly ISchemaOptionsFactory schemaOptionsFactory;
        private readonly IBasicConfiguration<SchemaConfiguration> schemas;
        private readonly ISchemaFactory schemaFactory;

        public SchemaService(ILogger logger, ISchemaOptionsFactory schemaOptionsFactory, IBasicConfiguration<SchemaConfiguration> schemas, ISchemaFactory schemaFactory)
        {
            schemaNamesCached = new();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.schemaOptionsFactory = schemaOptionsFactory ?? throw new ArgumentNullException(nameof(schemaOptionsFactory));
            this.schemas = schemas ?? throw new ArgumentNullException(nameof(schemas));
            this.schemaFactory = schemaFactory ?? throw new ArgumentNullException(nameof(schemaFactory));
        }

        public Tables Read(CodeGeneratorDto options, Dictionary<string, string> alternativeDescriptions = null)
        {
            StringBuilderWriter salida = new();

            try
            {
                StringExtensions.CurrentLang = GetLang(options.CreatedFromSchemaName);

                if (!schemaNamesCached.ContainsKey(options.SchemaName))
                {
                    var schemaOptions = schemaOptionsFactory.Create(options, salida);
                    var schemaReader = schemaFactory.Create(schemaOptions);
                    if (schemaReader == null)
                    {
                        throw new Exception($"Cannot create schema reader. Reason: {LogService.LastError}");
                    }
                    SchemaReaderOptions schemaReaderOptions = new(ShouldRemoveWord1(options.SchemaName), alternativeDescriptions);
                    var tables = schemaReader.ReadSchema(schemaReaderOptions);

                    if (schemaReader.HasErrorMessage())
                    {
                        logger.Error($"\"{salida.OutputStringBuilder}\"");
                    }

                    schemaNamesCached.Add(options.SchemaName, tables);
                }

                return schemaNamesCached[options.SchemaName];
            }
            catch (Exception ex)
            {
                logger.Error($"Error reading schema: {ex.Message}");
                if (salida.ContainsErrorMessage)
                    logger.Error($"\n{salida.OutputStringBuilder}");

                throw;
            }

        }

        /// <summary>
        /// Get language for schema name
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public string GetLang(string schemaName)
        {
            return schemas[schemaName]?.TableNameLanguage ?? DefaultLang;
        }

        /// <summary>
        /// Should remove first word on table name's?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public bool ShouldRemoveWord1(string schemaName)
        {
            return schemas[schemaName]?.RemoveFirstWordOnColumnNames ?? Remove1stDefaultValue;
        }

        /// <summary>
        /// Use normalized names?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public bool NormalizedNames(string schemaName)
        {
            return schemas[schemaName]?.NormalizedNames ?? false;
        }

        /// <summary>
        /// Can create DB?
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public bool CanCreateDB(string schemaName)
        {
            return schemas[schemaName]?.CanCreateDB ?? false;
        }

        /// <summary>
        /// Get schema dictionary from file
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        public Dictionary<string, string> GetSchemaDictionaryFromFile(string schemaName)
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
        public (string ConnectionString, string Provider) GetConnectionString(string schemaName)
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
    }
}
