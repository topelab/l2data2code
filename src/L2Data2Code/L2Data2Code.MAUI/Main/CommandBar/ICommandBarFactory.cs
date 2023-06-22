namespace L2Data2Code.MAUI.Main.CommandBar
{
    internal interface ICommandBarFactory
    {
        CommandBarVM Create(MainPageVM mainVM);
    }
}