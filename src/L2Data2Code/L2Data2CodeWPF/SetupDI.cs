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
                .Add<IJsonSetting, JsonSetting>(ResolveLifeCycleEnum.Singleton)
                .Add<IAppSettingsConfiguration, AppSettingsConfiguration>(ResolveLifeCycleEnum.Singleton)
                .Add<IAreasConfiguration, AreasConfiguration>(ResolveLifeCycleEnum.Singleton)
                .Add<IGlobalsConfiguration, GlobalsConfiguration>(ResolveLifeCycleEnum.Singleton)
                .Add<IBasicConfiguration<ModuleConfiguration>, ModulesConfiguration>(ResolveLifeCycleEnum.Singleton)
                .Add<IBasicConfiguration<SchemaConfiguration>, SchemasConfiguration>(ResolveLifeCycleEnum.Singleton)
                .Add<ITemplatesConfiguration, TemplatesConfiguration>(ResolveLifeCycleEnum.Singleton)
                .Add<IMustacheHelpers, MustacheHelpers>(ResolveLifeCycleEnum.Singleton)
                .Add<IMustacheRenderizer, MustacheRenderizer>(ResolveLifeCycleEnum.Singleton)
                .Add<ISchemaOptionsFactory, SchemaOptionsFactory>(ResolveLifeCycleEnum.Singleton)
                .Add<MainWindowVM, MainWindowVM>(ResolveLifeCycleEnum.Singleton)
                .Add<IMessagePanelService, MessagePanelService>(ResolveLifeCycleEnum.Singleton)
                .Add<IMessageService, MessageService>(ResolveLifeCycleEnum.Singleton)
                .Add<IAppService, AppService>(ResolveLifeCycleEnum.Singleton)
                .Add<ICommandService, CommandService>(ResolveLifeCycleEnum.Singleton)
                .Add<IGeneratorAdapter, GeneratorAdapter>(ResolveLifeCycleEnum.Singleton)
                .Add<IGitService, GitService>(ResolveLifeCycleEnum.Singleton)
                .Add<ISchemaService, SchemaService>(ResolveLifeCycleEnum.Singleton)
                .Add<ICodeGeneratorService, CodeGeneratorService>(ResolveLifeCycleEnum.Singleton)
                .Add<IDispatcherWrapper, DispatcherWrapper>(ResolveLifeCycleEnum.Singleton)
                .Add<INameResolver, NameResolver>(ResolveLifeCycleEnum.Singleton)
                .Add<ITemplateService, TemplateService>(ResolveLifeCycleEnum.Singleton)
                .Add<IProcessManager, ProcessManager>(ResolveLifeCycleEnum.Singleton)
                .Add<ISchemaFactory, SchemaFactory>(ResolveLifeCycleEnum.Singleton)

                .Add<IFileMonitorService, FileMonitorService>()
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
