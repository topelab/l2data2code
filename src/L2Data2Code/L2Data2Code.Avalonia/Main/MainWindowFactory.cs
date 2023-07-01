using L2Data2Code.Avalonia.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using L2Data2Code.SharedContext.Main;
using L2Data2Code.SharedContext.Main.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Localize;
using System;
using Topelab.Core.Resolver.Interfaces;

namespace L2Data2Code.Avalonia.Main
{
    internal class MainWindowFactory : IMainWindowFactory
    {
        private readonly IResolver resolver;
        private readonly IGeneratorAdapter generatorAdapter;
        private readonly IMainWindowEventManager mainWindowEventManager;
        private readonly IMainWindowVMChangeListener mainWindowVMChangeListener;
        private readonly IMainWindowVMInitializer mainWindowVMInitializer;
        private readonly IGlobalEventManager globalEventManager;
        private MainWindowVM mainWindowVM;

        public MainWindowFactory(IResolver resolver,
                                 IGeneratorAdapter generatorAdapter,
                                 IMainWindowEventManager mainWindowEventManager,
                                 IMainWindowVMChangeListener mainWindowVMChangeListener,
                                 IMainWindowVMInitializer mainWindowVMInitializer,
                                 IGlobalEventManager globalEventManager)
        {
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            this.generatorAdapter = generatorAdapter ?? throw new ArgumentNullException(nameof(generatorAdapter));
            this.mainWindowEventManager = mainWindowEventManager ?? throw new ArgumentNullException(nameof(mainWindowEventManager));
            this.mainWindowVMChangeListener = mainWindowVMChangeListener ?? throw new ArgumentNullException(nameof(mainWindowVMChangeListener));
            this.mainWindowVMInitializer = mainWindowVMInitializer ?? throw new ArgumentNullException(nameof(mainWindowVMInitializer));
            this.globalEventManager = globalEventManager ?? throw new ArgumentNullException(nameof(globalEventManager));
        }

        public MainWindow Create()
        {
            mainWindowVM = resolver.Get<MainWindowVM>();
            var generateCommand = resolver.Get<IGenerateCommand>();
            mainWindowVM.SetCommands(generateCommand);

            var window = new MainWindow
            {
                DataContext = mainWindowVM
            };
            window.Opened += (sender, args) =>
            {
                mainWindowVMChangeListener.Start(mainWindowVM);
                mainWindowVMInitializer.Initialize(mainWindowVM);
                mainWindowEventManager.Start(mainWindowVM);
                globalEventManager.Start(window);
            };
            window.Title = $"{Strings.Title} v{generatorAdapter.GeneratorVersion}";
            return window;
        }
    }
}
