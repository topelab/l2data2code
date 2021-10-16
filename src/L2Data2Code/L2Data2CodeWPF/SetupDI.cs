using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.ViewModel;
using Unity;
using L2Data2Code.BaseGenerator.Services;
using L2Data2Code.BaseGenerator.Interfaces;
using L2Data2Code.SharedLib.Helpers;
using L2Data2Code.SharedLib.Configuration;
using NLog;
using L2Data2CodeWPF.SharedLib;

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
            container.RegisterSingleton<IBasicNameValueConfiguration, AppSettingsConfiguration>(nameof(AppSettingsConfiguration));
            container.RegisterSingleton<IAreasConfiguration, AreasConfiguration>();
            container.RegisterSingleton<IGlobalsConfiguration, GlobalsConfiguration>();
            container.RegisterSingleton<IBasicConfiguration<ModuleConfiguration>, ModulesConfiguration>();
            container.RegisterSingleton<IBasicConfiguration<SchemaConfiguration>, SchemasConfiguration>();
            container.RegisterSingleton<ITemplatesConfiguration, TemplatesConfiguration>();

            container.RegisterType<MainWindowViewModel>(TypeLifetime.PerContainer);
            container.RegisterType<IMessagesViewModel, MessagesViewModel>(TypeLifetime.PerContainer);
            container.RegisterType<IMessageService, MessageService>(TypeLifetime.PerContainer);
            container.RegisterType<IAppService, AppService>(TypeLifetime.PerContainer);
            container.RegisterType<ICommandService, CommandService>(TypeLifetime.PerContainer);
            container.RegisterType<IGeneratorAdapter, GeneratorAdapter>(TypeLifetime.PerContainer);
            container.RegisterType<IMustacheRenderizer, MustacheRenderizer>(TypeLifetime.PerContainer);
            container.RegisterType<IGitService, GitService>(TypeLifetime.PerContainer);
            container.RegisterType<ISchemaService, SchemaService>(TypeLifetime.PerContainer);
            container.RegisterType<ICodeGeneratorService, CodeGeneratorService>(TypeLifetime.PerContainer);

            container.RegisterType<IFileMonitorService, FileMonitorService>(TypeLifetime.PerResolve);
            container.RegisterType<IDispatcherWrapper, DispatcherWrapper>(TypeLifetime.PerResolve);

            container.RegisterInstance<ILogger>(LogManager.GetCurrentClassLogger());
            return container;
        }
    }
}
