using L2Data2CodeWPF.Main;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    internal interface ICommandBarFactory
    {
        CommandBarVM Create(MainWindowVM mainVM);
    }
}