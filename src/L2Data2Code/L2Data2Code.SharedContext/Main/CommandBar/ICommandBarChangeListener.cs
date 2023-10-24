namespace L2Data2Code.SharedContext.Main.CommandBar
{
    public interface ICommandBarChangeListener
    {
        void Start(MainWindowVM mainVM, CommandBarVM controlVM);
        void Stop();
    }
}