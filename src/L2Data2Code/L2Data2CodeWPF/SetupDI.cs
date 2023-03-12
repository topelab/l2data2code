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
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Helpers;
using L2Data2Code.SharedLib.Interfaces;
using L2Data2Code.SharedLib.Services;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Commands;
using L2Data2CodeWPF.Commands.Interfaces;
using L2Data2CodeWPF.Controls.CommandBar;
using L2Data2CodeWPF.Controls.MessagePanel;
using L2Data2CodeWPF.Controls.TablePanel;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.SharedLib;
using NLog;
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
                .AddSingleton<MainWindowVM, MainWindowVM>()
                .AddSingleton<ICommandBarFactory, CommandBarFactory>()
                .AddSingleton<ICommandBarBindManager, CommandBarBindManager>()
                .AddSingleton<ITablePanelFactory, TablePanelFactory>()
                .AddSingleton<ITablePanelBindManager, TablePanelBindManager>()
                .AddSingleton<IMessagePanelFactory, MessagePanelFactory>()
                .AddSingleton<IMessagePanelBindManager, MessagePanelBindManager>()

                .AddSingleton<IEditTemplateCommand, EditTemplateCommand>()
                .AddSingleton<IOpenFolderCommand, OpenFolderCommand>()
                .AddSingleton<IOpenPSCommand, OpenPSCommand>()
                .AddSingleton<IOpenSettingsCommand, OpenSettingsCommand>()
                .AddSingleton<IOpenVarsWindowCommand, OpenVarsWindowCommand>()
                .AddSingleton<IOpenVSCodeCommand, OpenVSCodeCommand>()
                .AddSingleton<IOpenVSCommand, OpenVSCommand>()
                .AddSingleton<ILoadTablesCommand, LoadTablesCommand>()
                .AddSingleton<ISetDataItemsCommand, SetDataItemsCommand>()
                .AddSingleton<ISetDataItemCommand, SetDataItemCommand>()

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
                .AddSingleton<INameResolver, NameResolver>()
                .AddSingleton<ITemplateService, TemplateService>()
                .AddSingleton<IProcessManager, ProcessManager>()
                .AddSingleton<ISchemaFactory, SchemaFactory>()

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
