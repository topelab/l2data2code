namespace L2Data2Code.SharedContext.Main.MessagePanel
{
    public interface IMessagePanelChangeListener
    {
        void Start(MainWindowVM mainVM, MessagePanelVM controlVM);
        void Stop();
    }
}