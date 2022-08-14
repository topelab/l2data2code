using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.IO;

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

            settingsConfiguration.Merge(settingsConfiguration["TemplateSettings"]);
            settingsConfiguration["TemplatesBasePath"] ??= Path.GetDirectoryName(settingsConfiguration["TemplateSettings"]);
        }

        public void Create(string outputPath, string schema)
        {
            var templateBasePath = settingsConfiguration["TemplatesBasePath"].AddPathSeparator();
            var schemaOptions = schemaOptionsFactory.Create(templateBasePath, schemasConfiguration, schema, new StringBuilderWriter(), null);
            var tables = schemaService.Read(schemaOptions);
            var fileName = $"{outputPath.AddPathSeparator()}{schema.ToSlug()}-dbinfo.json";

            schemaService.GenerateJsonInfo(tables.Values, fileName);
        }
    }
}
