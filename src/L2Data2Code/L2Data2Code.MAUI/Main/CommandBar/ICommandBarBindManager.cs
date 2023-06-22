namespace L2Data2Code.MAUI.Main.CommandBar
{
    internal interface ICommandBarBindManager
    {
        void Start(MainWindowVM mainVM, CommandBarVM controlVM);
        void Stop();
    }
}