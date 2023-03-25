using L2Data2Code.Main.Interfaces;
using System;
using Topelab.Core.Resolver.Interfaces;

namespace L2Data2Code.Main
{
    internal class MainWindowFactory : IMainWindowFactory
    {
        private MainWindow mainWindow;
        private MainWindowViewModel viewModel;

        private readonly IResolver resolver;
        private readonly IMainWindowInitializer mainWindowInitializer;

        public MainWindowFactory(IResolver resolver, IMainWindowInitializer mainWindowInitializer)
        {
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            this.mainWindowInitializer = mainWindowInitializer ?? throw new ArgumentNullException(nameof(mainWindowInitializer));
        }

        public MainWindow Create()
        {
            viewModel = resolver.Get<MainWindowViewModel>();
            mainWindow = resolver.Get<MainWindow>();
            mainWindowInitializer.Initialize(viewModel);
            mainWindow.DataContext = viewModel;
            return mainWindow;
        }
    }
}
