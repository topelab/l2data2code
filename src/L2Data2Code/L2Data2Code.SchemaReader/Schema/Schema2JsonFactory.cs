using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.IO;
using System.Linq;

namespace L2Data2Code.SchemaReader.Schema
{
    public class Schema2JsonFactory : ISchema2JsonFactory
    {
        private readonly ISchemaService schemaService;
        private readonly ISchemaOptionsFactory schemaOptionsFactory;
        private readonly IAppSettingsConfiguration settingsConfiguration;
        private readonly IBasicConfiguration<SchemaConfiguration> schemasConfiguration;

        public Schema2JsonFactory(ISchemaService schemaService,
                                  ISchemaOptionsFactory schemaOptionsFactory,
                                  IAppSettingsConfiguration settingsConfiguration,
                                  IBasicConfiguration<SchemaConfiguration> schemasConfiguration)
        {
            this.schemaService = schemaService ?? throw new ArgumentNullException(nameof(schemaService));
            this.schemaOptionsFactory = schemaOptionsFactory ?? throw new ArgumentNullException(nameof(schemaOptionsFactory));
            this.settingsConfiguration = settingsConfiguration ?? throw new ArgumentNullException(nameof(settingsConfiguration));
            this.schemasConfiguration = schemasConfiguration ?? throw new ArgumentNullException(nameof(schemasConfiguration));

            if (settingsConfiguration[ConfigurationLabels.TEMPLATE_SETTINGS] == null)
            {
                settingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH] ??= Directory.GetCurrentDirectory();
            }
            else
            {
                settingsConfiguration.Merge(settingsConfiguration[ConfigurationLabels.TEMPLATE_SETTINGS]);
                settingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH] ??= Path.GetDirectoryName(settingsConfiguration[ConfigurationLabels.TEMPLATE_SETTINGS]);
            }
        }

        public void Create(string outputPath, string schema)
        {
            outputPath ??= settingsConfiguration["JsonGeneratedPath"];
            var templateBasePath = settingsConfiguration[ConfigurationLabels.TEMPLATES_BASE_PATH].AddPathSeparator();
            var schemaOptions = schemaOptionsFactory.Create(templateBasePath, schemasConfiguration, schema, new StringBuilderWriter());
            var tables = schemaService.Read(schemaOptions);
            var fileName = $"{outputPath.AddPathSeparator()}{schema.ToSlug()}-dbinfo.json";

            schemaService.GenerateJsonInfo(tables.Values, fileName);
        }

        public bool IsValidSchema(string schemaName, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(schemaName))
            {
                error = "Schema name cannot be null or empty.";
            }
            else
            {
                var schemes = schemasConfiguration.GetKeys();
                if (!schemes.Contains(schemaName))
                {
                    error = $"Schema name '{schemaName}' not found in configuration.";
                }

            }
            return error is null;
        }
    }
}
