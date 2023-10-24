using System.Windows.Input;

namespace L2Data2Code.SharedContext.Base
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
