using L2Data2Code.BaseGenerator.Configuration;
using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.BaseGenerator.Services;
using L2Data2Code.BaseMustache.Extensions;
using L2Data2Code.BaseMustache.Interfaces;
using L2Data2Code.BaseMustache.Services;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SharedLib.Configuration;
using L2Data2Code.SharedLib.Helpers;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.SharedLib;
using NLog;
using Unity;

namespace L2Data2CodeWPF
{
    public class SetupDI
    {

        private static IUnityContainer container;
        public static IUnityContainer Container => container ?? Register();

        public static IUnityContainer CreateContainer()
        {
            return Register(new UnityContainer());
        }

        private static IUnityContainer Register()
        {
            UnityContainer container = new();
            SetupDI.container = Register(container);
            return container;
        }

        private static IUnityContainer Register(IUnityContainer container)
        {
            container.RegisterSingleton<IJsonSetting, JsonSetting>();
            container.RegisterSingleton<IAppSettingsConfiguration, AppSettingsConfiguration>();
            container.RegisterSingleton<IAreasConfiguration, AreasConfiguration>();
            container.RegisterSingleton<IGlobalsConfiguration, GlobalsConfiguration>();
            container.RegisterSingleton<IBasicConfiguration<ModuleConfiguration>, ModulesConfiguration>();
            container.RegisterSingleton<IBasicConfiguration<SchemaConfiguration>, SchemasConfiguration>();
            container.RegisterSingleton<ITemplatesConfiguration, TemplatesConfiguration>();
            container.RegisterSingleton<IMustacheHelpers, MustacheHelpers>();
            container.RegisterSingleton<IMustacheRenderizer, MustacheRenderizer>();
            container.RegisterSingleton<ISchemaOptionsFactory, SchemaOptionsFactory>();
            container.RegisterSingleton<MainWindowVM>();
            container.RegisterSingleton<IMessagePanelService, MessagePanelService>();
            container.RegisterSingleton<IMessageService, MessageService>();
            container.RegisterSingleton<IAppService, AppService>();
            container.RegisterSingleton<ICommandService, CommandService>();
            container.RegisterSingleton<IGeneratorAdapter, GeneratorAdapter>();
            container.RegisterSingleton<IGitService, GitService>();
            container.RegisterSingleton<ISchemaService, SchemaService>();
            container.RegisterSingleton<ICodeGeneratorService, CodeGeneratorService>();
            container.RegisterSingleton<IDispatcherWrapper, DispatcherWrapper>();
            container.RegisterSingleton<INameResolver, NameResolver>();
            container.RegisterSingleton<ITemplateService, TemplateService>();

            container.RegisterType<IFileMonitorService, FileMonitorService>();

            container.RegisterInstance<ILogger>(LogManager.GetCurrentClassLogger());
            return container;
        }
    }
}
