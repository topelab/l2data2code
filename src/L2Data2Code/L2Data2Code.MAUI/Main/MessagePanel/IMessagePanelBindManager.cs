using L2Data2Code.MAUI.Main;

namespace L2Data2Code.MAUI.Main.MessagePanel
{
    internal interface IMessagePanelBindManager
    {
        void Start(MainPageVM mainVM, MessagePanelVM controlVM);
        void Stop();
    }
}