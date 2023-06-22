namespace L2Data2Code.MAUI.Main.CommandBar
{
    internal interface ICommandBarBindManager
    {
        void Start(MainPageVM mainVM, CommandBarVM controlVM);
        void Stop();
    }
}