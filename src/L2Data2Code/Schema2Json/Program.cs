using CommandLine;
using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Lib;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using System.IO;
using Topelab.Core.Resolver.Microsoft;


namespace Schema2Json
{
    class Program
    {
        static void Main(string[] args)
        {

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(Proceed);
        }

        private static void Proceed(Options options)
        {
            var resolver = ResolverFactory.Create(SetupDI.Register());
            var schemaService = resolver.Get<ISchemaService>();
            var schemaOptionsFactory = resolver.Get<ISchemaOptionsFactory>();
            var dataSorcesConfiguration = resolver.Get<IDataSorcesConfiguration>();
            var jsonSetting = resolver.Get<IJsonSetting>();
            var settingsConfiguration = resolver.Get<IAppSettingsConfiguration>();
            var schemasConfiguration = resolver.Get<IBasicConfiguration<SchemaConfiguration>>();

            jsonSetting.AddSettingFiles(settingsConfiguration["TemplateSettings"]);
            settingsConfiguration.Merge(jsonSetting.Config[AppSettingsConfiguration.APP_SETTINGS].ToNameValueCollection());
            settingsConfiguration["TemplatesBasePath"] ??= Path.GetDirectoryName(settingsConfiguration["TemplateSettings"]).AddPathSeparator();

            var schemaName = dataSorcesConfiguration.Schema(options.Schema);
            var descriptionsSchemaName = dataSorcesConfiguration.CommentSchema(options.Schema);
            var schemaOptions = schemaOptionsFactory.Create(options.OutputPath, schemasConfiguration, schemaName, new StringBuilderWriter(), descriptionsSchemaName);
            schemaOptions.CreatedFromSchemaName = schemaOptions.SchemaName;
            var tables = schemaService.Read(schemaOptions);
            var fileName = $"{options.OutputPath.AddPathSeparator()}{options.Schema.ToSlug()}-dbinfo.json";

            schemaService.GenerateJsonInfo(tables.Values, fileName);
        }
    }
}