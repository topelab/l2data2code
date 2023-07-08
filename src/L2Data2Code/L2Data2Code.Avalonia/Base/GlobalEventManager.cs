using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using L2Data2Code.Avalonia.Main;
using L2Data2Code.Avalonia.Main.Vars;
using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Events;
using Prism.Events;
using System;
using System.Collections.Generic;

namespace L2Data2Code.Avalonia.Base
{
    internal class GlobalEventManager : IGlobalEventManager
    {
        private MainWindow window;
        private readonly IEventAggregator eventAggregator;
        private readonly IVarsFactory varsFactory;
        private readonly IDispatcherWrapper dispatcherWrapper;

        public GlobalEventManager(IEventAggregator eventAggregator, IVarsFactory varsFactory, IDispatcherWrapper dispatcherWrapper)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.varsFactory = varsFactory ?? throw new ArgumentNullException(nameof(varsFactory));
            this.dispatcherWrapper = dispatcherWrapper ?? throw new ArgumentNullException(nameof(dispatcherWrapper));
        }

        public void Start(MainWindow window)
        {
            this.window = window;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            eventAggregator.GetEvent<OpenVarsWindowEvent>().Subscribe(OnOpenVarsWindow);
            eventAggregator.GetEvent<CloseApplicationEvent>().Subscribe(OnCloseApplication);
            eventAggregator.GetEvent<ActivateMainWindowEvent>().Subscribe(OnActivateMainWindow);
        }

        private void OnActivateMainWindow()
        {
            window.Activate();
        }

        private void OnCloseApplication(bool hasToRestart)
        {
            dispatcherWrapper.Invoke(() =>
            {
                App.RestartApp = hasToRestart;
                window.Close();
            });
        }

        private void OnOpenVarsWindow(Dictionary<string, object> vars)
        {
            var varsWindow = varsFactory.Create(vars);
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                varsWindow.ShowDialog(desktop.MainWindow);
            }
        }
    }
}
