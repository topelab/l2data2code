using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeWPF.ViewModel;
using Unity;
using L2Data2Code.BaseGenerator.Services;
using L2Data2Code.BaseGenerator.Interfaces;

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
            var container = new UnityContainer();
            SetupDI.container = Register(container);
            return container;
        }

        private static IUnityContainer Register(IUnityContainer container)
        {
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
            return container;
        }
    }
}
