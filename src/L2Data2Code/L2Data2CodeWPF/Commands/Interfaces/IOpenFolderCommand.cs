using L2Data2CodeWPF.Base;
using System.Windows.Input;

namespace L2Data2CodeWPF.Commands.Interfaces
{
    internal interface IOpenFolderCommand : ICommand
    {
        void SetViewModel(IBaseVM baseVM);
    }
}