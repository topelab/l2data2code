namespace L2Data2Code.SharedContext.Main.MessagePanel
{
    public interface IMessagePanelFactory
    {
        MessagePanelVM Create(MainWindowVM mainVM);
    }
}