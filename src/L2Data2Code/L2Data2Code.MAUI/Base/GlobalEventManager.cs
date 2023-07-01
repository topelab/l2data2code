using L2Data2Code.MAUI.Main;
using L2Data2Code.SharedContext.Events;
using Prism.Events;

namespace L2Data2Code.MAUI.Base
{
    internal class GlobalEventManager : IGlobalEventManager
    {
        private MainPage mainPage;
        private readonly IEventAggregator eventAggregator;

        public GlobalEventManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        }

        public void Start(MainPage mainPage)
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
            App.RestartApp = hasToRestart;
            App.Current.CloseWindow(mainPage.Window);
        }

        private void OnOpenVarsWindow(Dictionary<string, object> vars)
        {
        }
    }
}
