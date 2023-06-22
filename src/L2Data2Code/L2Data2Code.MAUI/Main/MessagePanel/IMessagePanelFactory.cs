namespace L2Data2Code.MAUI.Main.MessagePanel
{
    internal interface IMessagePanelFactory
    {
        MessagePanelVM Create(MainWindowVM mainVM);
    }
}