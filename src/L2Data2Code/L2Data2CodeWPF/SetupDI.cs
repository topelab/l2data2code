using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.BaseGenerator.Services;
using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.BaseMustache.Services;
using L2Data2Code.SchemaReader.Fake;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Json;
using L2Data2Code.SchemaReader.MySql;
using L2Data2Code.SchemaReader.Object;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SchemaReader.SqlServer;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.SharedLib;
using NLog;
using Topelab.Core.Resolver.Entities;
using Topelab.Core.Resolver.Enums;

namespace L2Data2CodeWPF
{
    public class SetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .Add<IJsonSetting, JsonSetting>(ResolveTypeEnum.Singleton)
                .Add<IAppSettingsConfiguration, AppSettingsConfiguration>(ResolveTypeEnum.Singleton)
                .Add<IAreasConfiguration, AreasConfiguration>(ResolveTypeEnum.Singleton)
                .Add<IGlobalsConfiguration, GlobalsConfiguration>(ResolveTypeEnum.Singleton)
                .Add<IBasicConfiguration<ModuleConfiguration>, ModulesConfiguration>(ResolveTypeEnum.Singleton)
                .Add<IBasicConfiguration<SchemaConfiguration>, SchemasConfiguration>(ResolveTypeEnum.Singleton)
                .Add<ITemplatesConfiguration, TemplatesConfiguration>(ResolveTypeEnum.Singleton)
                .Add<IMustacheHelpers, MustacheHelpers>(ResolveTypeEnum.Singleton)
                .Add<IMustacheRenderizer, MustacheRenderizer>(ResolveTypeEnum.Singleton)
                .Add<ISchemaOptionsFactory, SchemaOptionsFactory>(ResolveTypeEnum.Singleton)
                .Add<MainWindowVM, MainWindowVM>(ResolveTypeEnum.Singleton)
                .Add<IMessagePanelService, MessagePanelService>(ResolveTypeEnum.Singleton)
                .Add<IMessageService, MessageService>(ResolveTypeEnum.Singleton)
                .Add<IAppService, AppService>(ResolveTypeEnum.Singleton)
                .Add<ICommandService, CommandService>(ResolveTypeEnum.Singleton)
                .Add<IGeneratorAdapter, GeneratorAdapter>(ResolveTypeEnum.Singleton)
                .Add<IGitService, GitService>(ResolveTypeEnum.Singleton)
                .Add<ISchemaService, SchemaService>(ResolveTypeEnum.Singleton)
                .Add<ICodeGeneratorService, CodeGeneratorService>(ResolveTypeEnum.Singleton)
                .Add<IDispatcherWrapper, DispatcherWrapper>(ResolveTypeEnum.Singleton)
                .Add<INameResolver, NameResolver>(ResolveTypeEnum.Singleton)
                .Add<ITemplateService, TemplateService>(ResolveTypeEnum.Singleton)
                .Add<IProcessManager, ProcessManager>(ResolveTypeEnum.Singleton)
                .Add<ISchemaFactory, SchemaFactory>(ResolveTypeEnum.Singleton)

                .Add<IFileMonitorService, FileMonitorService>(ResolveTypeEnum.PerResolve)
                .Add<ISchemaReader, SqlServerSchemaReader>(nameof(SqlServerSchemaReader), typeof(INameResolver), typeof(SchemaOptions))
                .Add<ISchemaReader, MySqlSchemaReader>(nameof(MySqlSchemaReader), typeof(INameResolver), typeof(SchemaOptions))
                .Add<ISchemaReader, FakeSchemaReader>(nameof(FakeSchemaReader), typeof(INameResolver), typeof(SchemaOptions))
                .Add<ISchemaReader, JsonSchemaReader>(nameof(JsonSchemaReader), typeof(INameResolver), typeof(SchemaOptions))
                .Add<ISchemaReader, ObjectSchemaReader>(nameof(ObjectSchemaReader), typeof(INameResolver), typeof(SchemaOptions))

                .Add<ILogger, Logger>(LogManager.GetCurrentClassLogger())

                ;
        }
    }
}
