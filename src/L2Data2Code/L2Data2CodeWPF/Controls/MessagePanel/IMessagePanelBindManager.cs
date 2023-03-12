using L2Data2CodeWPF.Main;

namespace L2Data2CodeWPF.Controls.MessagePanel
{
    internal interface IMessagePanelBindManager
    {
        void Start(MainWindowVM mainVM, MessagePanelVM controlVM);
        void Stop();
    }
}