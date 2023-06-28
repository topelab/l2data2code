using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using L2Data2Code.Main;
using L2Data2Code.Main.Vars;
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

        public GlobalEventManager(IEventAggregator eventAggregator, IVarsFactory varsFactory)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.varsFactory = varsFactory ?? throw new ArgumentNullException(nameof(varsFactory));
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
            App.RestartApp = hasToRestart;
            window.Close();
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
