using L2Data2CodeWPF.Base;

namespace L2Data2CodeWPF.Controls.CommandBar
{
    internal interface ICommandBarFactory
    {
        CommandBarVM Create(IBaseVM baseVM);
    }
}