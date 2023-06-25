namespace L2Data2Code.SharedContext.Main.CommandBar
{
    internal interface ICommandBarBindManager
    {
        void Start(MainWindowVM mainVM, CommandBarVM controlVM);
        void Stop();
    }
}