using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.BaseGenerator.Services;
using L2Data2Code.BaseHandleBars;
using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Fake;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Json;
using L2Data2Code.SchemaReader.MySql;
using L2Data2Code.SchemaReader.Object;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SchemaReader.SqlServer;
using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Main;
using L2Data2Code.SharedContext.Main.CommandBar;
using L2Data2Code.SharedContext.Main.Interfaces;
using L2Data2Code.SharedContext.Main.MessagePanel;
using L2Data2Code.SharedContext.Main.TablePanel;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using L2Data2Code.SharedLib.Interfaces;
using L2Data2Code.SharedLib.Services;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.SharedLib;
using NLog;
using Prism.Events;
using Topelab.Core.Resolver.Entities;

namespace L2Data2CodeWPF
{
    public class SetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddSingleton<IJsonSetting, JsonSetting>()
                .AddSingleton<IAppSettingsConfiguration, AppSettingsConfiguration>()
                .AddSingleton<IDataSorcesConfiguration, DataSourcesConfiguration>()
                .AddSingleton<IGlobalsConfiguration, GlobalsConfiguration>()
                .AddSingleton<IBasicConfiguration<ModuleConfiguration>, ModulesConfiguration>()
                .AddSingleton<IBasicConfiguration<SchemaConfiguration>, SchemasConfiguration>()
                .AddSingleton<ITemplatesConfiguration, TemplatesConfiguration>()
                .AddSingleton<IMustacheRenderizer, HandleBarsRenderizer>()
                .AddSingleton<IConditionalPathRenderizer, ConditionalPathRenderizer>()
                .AddSingleton<IFileService, FileService>()
                .AddSingleton<ISchemaOptionsFactory, SchemaOptionsFactory>()
                .AddSingleton<ICommandBarFactory, CommandBarFactory>()
                .AddSingleton<ITablePanelFactory, TablePanelFactory>()
                .AddSingleton<IMessagePanelFactory, MessagePanelFactory>()

                .AddSingleton<IEditorLocatorService, EditorLocatorService>()
                .AddSingleton<IMessagePanelService, MessagePanelService>()
                .AddSingleton<IMessageService, MessageService>()
                .AddSingleton<IAppService, AppService>()
                .AddSingleton<ICommandService, CommandService>()
                .AddSingleton<IGeneratorAdapter, GeneratorAdapter>()
                .AddSingleton<IGitService, GitService>()
                .AddSingleton<ISchemaService, SchemaService>()
                .AddSingleton<ICodeGeneratorService, CodeGeneratorService>()
                .AddSingleton<IDispatcherWrapper, DispatcherWrapper>()
                .AddSingleton<IMessageBoxWrapper, MessageBoxWrapper>()
                .AddSingleton<INameResolver, NameResolver>()
                .AddSingleton<ITemplateService, TemplateService>()
                .AddSingleton<IProcessManager, ProcessManager>()
                .AddSingleton<ISchemaFactory, SchemaFactory>()
                .AddSingleton<IMainWindowFactory, MainWindowFactory>()
                .AddSingleton<IReplacementCollectionFactory, ReplacementCollectionFactory>()
                .AddSingleton<IGlobalEventManager, GlobalEventManager>()
                .AddSingleton<IEventAggregator, EventAggregator>()

                .AddTransient<MainWindowVM, MainWindowVM>()
                .AddTransient<IMainWindowEventManager, MainWindowEventManager>()
                .AddTransient<IMainWindowVMChangeListener, MainWindowVMChangeListener>()
                .AddTransient<IMainWindowVMInitializer, MainWindowVMInitializer>()
                .AddTransient<ICommandBarChangeListener, CommandBarChangeListener>()
                .AddTransient<ITablePanelChangeListener, TablePanelChangeListener>()
                .AddTransient<IMessagePanelChangeListener, MessagePanelChangeListener>()
                .AddTransient<IGenerateCommandFactory, GenerateCommandFactory>()
                .AddTransient<IEditTemplateCommandFactory, EditTemplateCommandFactory>()
                .AddTransient<IOpenFolderCommandFactory, OpenFolderCommandFactory>()
                .AddTransient<IOpenPSCommandFactory, OpenPSCommandFactory>()
                .AddTransient<IOpenSettingsCommandFactory, OpenSettingsCommandFactory>()
                .AddTransient<IOpenVarsWindowCommandFactory, OpenVarsWindowCommandFactory>()
                .AddTransient<IOpenVSCodeCommandFactory, OpenVSCodeCommandFactory>()
                .AddTransient<IOpenVSCommandFactory, OpenVSCommandFactory>()
                .AddTransient<ILoadTablesCommandFactory, LoadTablesCommandFactory>()
                .AddTransient<ISetDataItemsCommandFactory, SetDataItemsCommand>()
                .AddTransient<ISetDataItemCommandFactory, SetDataItemCommandFactory>()

                .AddTransient<IFileMonitorService, FileMonitorService>()
                .AddTransient<ISchemaReader, SqlServerSchemaReader>(nameof(SqlServerSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, MySqlSchemaReader>(nameof(MySqlSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, FakeSchemaReader>(nameof(FakeSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, JsonSchemaReader>(nameof(JsonSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, ObjectSchemaReader>(nameof(ObjectSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, SQLiteSchemaReader>(nameof(SQLiteSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))

                .AddInstance<ILogger>(LogManager.GetCurrentClassLogger())

                ;
        }
    }
}
