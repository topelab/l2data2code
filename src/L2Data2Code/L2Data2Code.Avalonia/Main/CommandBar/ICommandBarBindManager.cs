using L2Data2Code.Main;

namespace L2Data2Code.Main.CommandBar
{
    internal interface ICommandBarBindManager
    {
        void Start(MainWindowVM mainVM, CommandBarVM controlVM);
        void Stop();
    }
}