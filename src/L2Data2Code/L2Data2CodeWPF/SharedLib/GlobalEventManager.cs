using L2Data2Code.SharedContext.Events;
using L2Data2Code.SharedContext.Main.Vars;
using L2Data2CodeWPF.Main;
using L2Data2CodeWPF.Vars;
using Prism.Events;
using System;
using System.Collections.Generic;

namespace L2Data2CodeWPF.SharedLib
{
    internal class GlobalEventManager : IGlobalEventManager
    {
        private MainWindow window;
        private readonly IEventAggregator eventAggregator;

        public GlobalEventManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
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
            VarsVM varsVM = new(vars);
            VarsWindow varsWindow = new(varsVM);
            varsWindow.ShowDialog();
        }
    }
}
