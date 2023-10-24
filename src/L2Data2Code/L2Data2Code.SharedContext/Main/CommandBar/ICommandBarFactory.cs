namespace L2Data2Code.SharedContext.Main.CommandBar
{
    public interface ICommandBarFactory
    {
        CommandBarVM Create(MainWindowVM mainVM);
    }
}