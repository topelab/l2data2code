using L2Data2CodeUI.Shared.Localize;
using System;
using Topelab.Core.Resolver.Interfaces;

namespace L2Data2CodeWPF.Main
{
    internal class MainWindowFactory : IMainWindowFactory
    {
        private readonly IResolver resolver;
        private readonly IMainWindowEventManager mainWindowEventManager;
        private MainWindowVM mainWindowVM;

        public MainWindowFactory(IResolver resolver, IMainWindowEventManager mainWindowEventManager)
        {
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            this.mainWindowEventManager = mainWindowEventManager ?? throw new ArgumentNullException(nameof(mainWindowEventManager));
        }

        public MainWindow Create()
        {
            mainWindowVM = resolver.Get<MainWindowVM>();
            var window = new MainWindow(mainWindowVM);
            mainWindowEventManager.Start(window, mainWindowVM);
            window.Title = $"{Strings.Title} v{mainWindowVM.GeneratorVersion}";
            return window;
        }
    }
}
