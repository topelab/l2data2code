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
        private readonly IDataSorcesConfiguration dataSorcesConfiguration;
        private readonly IAppSettingsConfiguration settingsConfiguration;
        private readonly IBasicConfiguration<SchemaConfiguration> schemasConfiguration;

        public Schema2JsonFactory(ISchemaService schemaService,
                                  ISchemaOptionsFactory schemaOptionsFactory,
                                  IDataSorcesConfiguration dataSorcesConfiguration,
                                  IAppSettingsConfiguration settingsConfiguration,
                                  IBasicConfiguration<SchemaConfiguration> schemasConfiguration)
        {
            this.schemaService = schemaService ?? throw new ArgumentNullException(nameof(schemaService));
            this.schemaOptionsFactory = schemaOptionsFactory ?? throw new ArgumentNullException(nameof(schemaOptionsFactory));
            this.dataSorcesConfiguration = dataSorcesConfiguration ?? throw new ArgumentNullException(nameof(dataSorcesConfiguration));
            this.settingsConfiguration = settingsConfiguration ?? throw new ArgumentNullException(nameof(settingsConfiguration));
            this.schemasConfiguration = schemasConfiguration ?? throw new ArgumentNullException(nameof(schemasConfiguration));

            settingsConfiguration.Merge(settingsConfiguration["TemplateSettings"]);
            settingsConfiguration["TemplatesBasePath"] ??= Path.GetDirectoryName(settingsConfiguration["TemplateSettings"]);
        }

        public void Create(string outputPath, string schema)
        {
            var schemaName = dataSorcesConfiguration.Schema(schema);
            var descriptionsSchemaName = dataSorcesConfiguration.CommentSchema(schema);
            var templateBasePath = settingsConfiguration["TemplatesBasePath"].AddPathSeparator();
            var schemaOptions = schemaOptionsFactory.Create(templateBasePath, schemasConfiguration, schemaName, new StringBuilderWriter(), descriptionsSchemaName);
            var tables = schemaService.Read(schemaOptions);
            var fileName = $"{outputPath.AddPathSeparator()}{schemaName.ToSlug()}-dbinfo.json";

            schemaService.GenerateJsonInfo(tables.Values, fileName);
        }
    }
}
