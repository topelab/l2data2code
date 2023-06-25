namespace L2Data2Code.SharedContext.Main.MessagePanel
{
    internal interface IMessagePanelFactory
    {
        MessagePanelVM Create(MainWindowVM mainVM);
    }
}