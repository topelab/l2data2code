using L2Data2Code.MAUI.Main;

namespace L2Data2Code.MAUI.Main.MessagePanel
{
    internal interface IMessagePanelBindManager
    {
        void Start(MainWindowVM mainVM, MessagePanelVM controlVM);
        void Stop();
    }
}