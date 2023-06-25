namespace L2Data2Code.SharedContext.Main.CommandBar
{
    internal interface ICommandBarFactory
    {
        CommandBarVM Create(MainWindowVM mainVM);
    }
}