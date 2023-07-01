using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using Topelab.Core.Resolver.Interfaces;
using L2Data2Code.SharedContext.Main.Interfaces;
using L2Data2Code.SharedContext.Main;
using L2Data2Code.SharedContext.Commands.Interfaces;

namespace L2Data2Code.MAUI.Main
{
    internal class MainPageFactory : IMainPageFactory
    {
        private readonly IResolver resolver;
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly IMainWindowEventManager mainWindowEventManager;
        private readonly IMainWindowVMChangeListener mainWindowVMChangeListener;
        private readonly IMainWindowVMInitializer mainWindowVMInitializer;
        private MainWindowVM mainWindowVM;

        public MainPageFactory(IResolver resolver, IGeneratorAdapter generatorAdapter, IMainWindowEventManager mainWindowEventManager, IMainWindowVMChangeListener mainWindowVMChangeListener, IMainWindowVMInitializer mainWindowVMInitializer)
        {
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.mainWindowEventManager = mainWindowEventManager ?? throw new ArgumentNullException(nameof(mainWindowEventManager));
            this.mainWindowVMChangeListener = mainWindowVMChangeListener ?? throw new ArgumentNullException(nameof(mainWindowVMChangeListener));
            this.mainWindowVMInitializer = mainWindowVMInitializer ?? throw new ArgumentNullException(nameof(mainWindowVMInitializer));
        }

        public Page Create()
        {
            mainWindowVM = resolver.Get<MainWindowVM>();
            var generateCommand = resolver.Get<IGenerateCommand>();
            mainWindowVM.SetCommands(generateCommand);

            Page window = new AppShell
            {
                BindingContext = mainWindowVM,
                Title = $"{Strings.Title} v{generatorAdapter.GeneratorVersion}"
            };
            window.Loaded += (sender, args) =>
            {
                mainWindowVMChangeListener.Start(mainWindowVM);
                mainWindowVMInitializer.Initialize(mainWindowVM);
                mainWindowEventManager.Start(mainWindowVM);
            };
            return window;
        }

        private void OnWindowLoaded(object sender, EventArgs e)
        {
        }
    }
}
