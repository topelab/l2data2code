using L2Data2CodeUI.Shared.Localize;
using L2Data2CodeWPF.Commands.Interfaces;
using System;
using Topelab.Core.Resolver.Interfaces;

namespace L2Data2CodeWPF.Main
{
    internal class MainWindowFactory : IMainWindowFactory
    {
        private readonly IResolver resolver;
        private readonly IMainWindowEventManager mainWindowEventManager;
        private readonly IMainWindowVMBindManager mainWindowVMBindManager;
        private readonly IMainWindowVMInitializer mainWindowVMInitializer;
        private MainWindowVM mainWindowVM;

        public MainWindowFactory(IResolver resolver, IMainWindowEventManager mainWindowEventManager, IMainWindowVMBindManager mainWindowVMBindManager, IMainWindowVMInitializer mainWindowVMInitializer)
        {
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            this.mainWindowEventManager = mainWindowEventManager ?? throw new ArgumentNullException(nameof(mainWindowEventManager));
            this.mainWindowVMBindManager = mainWindowVMBindManager ?? throw new ArgumentNullException(nameof(mainWindowVMBindManager));
            this.mainWindowVMInitializer = mainWindowVMInitializer ?? throw new ArgumentNullException(nameof(mainWindowVMInitializer));
        }

        public MainWindow Create()
        {
            mainWindowVM = resolver.Get<MainWindowVM>();
            var generateCommand = resolver.Get<IGenerateCommand>();
            mainWindowVM.SetCommands(generateCommand);

            mainWindowVMBindManager.Start(mainWindowVM);
            mainWindowVMInitializer.Initialize(mainWindowVM);
            var window = new MainWindow(mainWindowVM);
            mainWindowEventManager.Start(window, mainWindowVM);
            window.Title = $"{Strings.Title} v{mainWindowVM.GeneratorVersion}";
            return window;
        }
    }
}
