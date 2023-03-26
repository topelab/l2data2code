using L2Data2Code.Main;

namespace L2Data2Code.Main.CommandBar
{
    internal interface ICommandBarFactory
    {
        CommandBarVM Create(MainWindowVM mainVM);
    }
}