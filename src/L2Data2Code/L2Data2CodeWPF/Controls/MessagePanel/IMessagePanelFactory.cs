using L2Data2CodeWPF.Main;

namespace L2Data2CodeWPF.Controls.MessagePanel
{
    internal interface IMessagePanelFactory
    {
        MessagePanelVM Create(MainWindowVM mainVM);
    }
}