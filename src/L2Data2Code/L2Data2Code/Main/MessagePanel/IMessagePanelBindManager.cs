using L2Data2Code.Main;

namespace L2Data2Code.Main.MessagePanel
{
    internal interface IMessagePanelBindManager
    {
        void Start(MainWindowVM mainVM, MessagePanelVM controlVM);
        void Stop();
    }
}