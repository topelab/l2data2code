namespace L2Data2Code.SharedContext.Main.MessagePanel
{
    internal interface IMessagePanelChangeListener
    {
        void Start(MainWindowVM mainVM, MessagePanelVM controlVM);
        void Stop();
    }
}