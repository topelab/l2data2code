using L2Data2Code.MAUI.Commands.Interfaces;
using L2Data2Code.MAUI.Main.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using System;
using Topelab.Core.Resolver.Interfaces;

namespace L2Data2Code.MAUI.Main
{
    internal class MainWindowFactory : IMainWindowFactory
    {
        private readonly IResolver resolver;
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly IMainWindowEventManager mainWindowEventManager;
        private readonly IMainWindowVMBindManager mainWindowVMBindManager;
        private readonly IMainWindowVMInitializer mainWindowVMInitializer;
        private MainWindowVM mainWindowVM;

        public MainWindowFactory(IResolver resolver, IGeneratorAdapter generatorAdapter, IMainWindowEventManager mainWindowEventManager, IMainWindowVMBindManager mainWindowVMBindManager, IMainWindowVMInitializer mainWindowVMInitializer)
        {
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.mainWindowEventManager = mainWindowEventManager ?? throw new ArgumentNullException(nameof(mainWindowEventManager));
            this.mainWindowVMBindManager = mainWindowVMBindManager ?? throw new ArgumentNullException(nameof(mainWindowVMBindManager));
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
                mainWindowVMBindManager.Start(mainWindowVM);
                mainWindowVMInitializer.Initialize(mainWindowVM);
                mainWindowEventManager.Start(window, mainWindowVM);
            };
            return window;
        }

        private void OnWindowLoaded(object sender, EventArgs e)
        {
        }
    }
}
