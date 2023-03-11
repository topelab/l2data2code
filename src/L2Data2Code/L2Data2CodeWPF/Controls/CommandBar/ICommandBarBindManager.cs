using L2Data2CodeWPF.Main;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    internal interface ICommandBarBindManager
    {
        void Start(MainWindowVM mainVM, CommandBarVM controlVM);
        void Stop();
    }
}