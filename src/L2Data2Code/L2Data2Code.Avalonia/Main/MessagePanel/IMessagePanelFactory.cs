using L2Data2Code.Main;

namespace L2Data2Code.Main.MessagePanel
{
    internal interface IMessagePanelFactory
    {
        MessagePanelVM Create(MainWindowVM mainVM);
    }
}