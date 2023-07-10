using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Events;
using Prism.Events;

namespace L2Data2CodeMaui.Base
{
    internal class GlobalEventManager : IGlobalEventManager
    {
        private Page mainPage;
        private readonly IEventAggregator eventAggregator;
        private readonly IDispatcherWrapper dispatcherWrapper;

        public GlobalEventManager(IEventAggregator eventAggregator, IDispatcherWrapper dispatcherWrapper)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.dispatcherWrapper = dispatcherWrapper ?? throw new ArgumentNullException(nameof(dispatcherWrapper));
        }

        public void Start(Page mainPage)
        {
            this.mainPage = mainPage;
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
            mainPage.Focus();
        }

        private void OnCloseApplication(bool hasToRestart)
        {
            dispatcherWrapper.Invoke(() =>
            {
                App.RestartApp = hasToRestart;
                App.Current.CloseWindow(mainPage.Window);
            });
        }

        private void OnOpenVarsWindow(Dictionary<string, object> vars)
        {
        }
    }
}
