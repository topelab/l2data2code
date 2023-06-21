using System;
using System.Windows.Input;

namespace L2Data2Code.MAUI.Base
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> executeAction;
        private readonly Func<object, bool> canExecuteAction;

        public DelegateCommand()
        {
        }

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteAction)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = canExecuteAction;
        }

        public virtual void Execute(object parameter) => executeAction?.Invoke(parameter);
        public virtual bool CanExecute(object parameter) => canExecuteAction?.Invoke(parameter) ?? true;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
