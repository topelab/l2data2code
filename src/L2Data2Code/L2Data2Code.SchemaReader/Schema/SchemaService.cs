using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using L2Data2Code.SharedLib.Interfaces;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace L2Data2Code.SchemaReader.Schema
{
    /// <summary>
    /// Schema service
    /// </summary>
    public partial class SchemaService : ISchemaService
    {
        private readonly string DefaultLang = "en";
        private readonly bool Remove1stDefaultValue = false;

        private readonly ILogger logger;
        private readonly Dictionary<string, Tables> schemaNamesCached;
        private readonly IBasicConfiguration<SchemaConfiguration> schemas;
        private readonly ISchemaFactory schemaFactory;
        private readonly IFileService fileService;

        /// <summary>
        /// Schema service
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="schemas">Schema configuration</param>
        /// <param name="schemaFactory">Schema factory</param>
        /// <param name="fileService">File service</param>
        public SchemaService(ILogger logger, IBasicConfiguration<SchemaConfiguration> schemas, ISchemaFactory schemaFactory, IFileService fileService)
        {
            schemaNamesCached = new();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.schemas = schemas ?? throw new ArgumentNullException(nameof(schemas));
            this.schemaFactory = schemaFactory ?? throw new ArgumentNullException(nameof(schemaFactory));
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        /// <summary>
        /// Read schema info specified at options
        /// </summary>
        /// <param name="options">Schema options</param>
        /// <param name="alternativeDescriptions">Dictionary with alternative descriptions</param>
        /// <returns>Specific dictionary of tables</returns>
        public Tables Read(ISchemaOptions options, Dictionary<string, string> alternativeDescriptions = null)
        {
            try
            {
                StringExtensions.CurrentLang = GetLang(options.CreatedFromSchemaName);

                if (!schemaNamesCached.ContainsKey(options.SchemaName))
                {
                    var schemaReader = schemaFactory.Create(options) ?? throw new Exception($"Cannot create schema reader. Reason: {LogService.LastError}");
                    SchemaReaderOptions schemaReaderOptions = new(ShouldRemoveWord1(options.SchemaName), alternativeDescriptions);
                    var tables = schemaReader.ReadSchema(schemaReaderOptions);

                    if (schemaReader.HasErrorMessage())
                    {
                        logger.Error($"\"{options.SummaryWriter.OutputStringBuilder}\"");
                    }

                    schemaNamesCached.Add(options.SchemaName, tables);
                }

                return schemaNamesCached[options.SchemaName];
            }
            catch (Exception ex)
            {
                logger.Error($"Error reading schema: {ex.Message}");
                if (options.SummaryWriter.ContainsErrorMessage)
                {
                    logger.Error($"\n{options.SummaryWriter.OutputStringBuilder}");
                }

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

        private static Regex definitionsRegex = DescriptionTemplate();


        /// <summary>
        /// Get schema dictionary from file
        /// </summary>
        /// <param name="schemaName">Schema name key</param>
        /// <param name="templatePath">Base directori</param>
        public Dictionary<string, string> GetSchemaDictionaryFromFile(string schemaName, string templatePath)
        {
            Dictionary<string, string> schemaDictionary = new();
            var descriptionFile = schemas[schemaName]?.DescriptionsFile;
            if (descriptionFile == null || !File.Exists(Path.Combine(templatePath, descriptionFile)))
            {
                return schemaDictionary;
            }

            var content = File.ReadAllLines(Path.Combine(templatePath, descriptionFile));
            for (var i = 1; i < content.Length; i++)
            {
                var line = content[i].Trim();
                var matchCondition = DescriptionTemplate().Match(line);
                if (matchCondition.Success)
                {
                    schemaDictionary.Add(matchCondition.Groups["column"].Value, matchCondition.Groups["description"].Value);
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

        /// <summary>
        /// Generate JSON file with schema info
        /// </summary>
        /// <param name="processTables">Tables to process</param>
        /// <param name="outputFileName">Output file name (full path) for JSON file</param>
        public void GenerateJsonInfo(IEnumerable<Table> processTables, string outputFileName)
        {
            PropertyRenameAndIgnoreSerializerContractResolver jsonResolver = new();
            jsonResolver.IgnoreProperty(typeof(Column),
                nameof(Column.Table),
                nameof(Column.FullName),
                nameof(Column.FullNameWithOwner),
                nameof(Column.PropertyName)
                );
            jsonResolver.IgnoreProperty(typeof(Table),
                nameof(Table.PK),
                nameof(Table.CleanName),
                nameof(Table.ClassName),
                nameof(Table.Type)
                );
            jsonResolver.IgnoreProperty(typeof(Key),
                nameof(Key.ColumnReferenced),
                nameof(Key.ColumnReferencing)
                );

            fileService.Write(outputFileName, JsonConvert.SerializeObject(new TablesDTO { Tables = processTables }, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = jsonResolver,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                }));
        }

        [GeneratedRegex(@"^\s*(?<column>[^;,\s]+)(;|,|\s)+(?<description>.+)$")]
        private static partial Regex DescriptionTemplate();
    }
}
