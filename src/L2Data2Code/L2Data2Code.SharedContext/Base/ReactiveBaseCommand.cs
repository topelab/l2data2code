using L2Data2Code.SharedContext.Commands.Interfaces;
using System;
using System.Windows.Input;

namespace L2Data2Code.SharedContext.Base
{
    public class ReactiveBaseCommand : ICommand
    {
        protected readonly ICommandManager commandManager;

        public ReactiveBaseCommand(ICommandManager commandManager)
        {
            this.commandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
        }

        public virtual void Execute(object parameter)
        {
        }

        public virtual bool CanExecute(object parameter) => true;

        public event EventHandler CanExecuteChanged
        {
            add { commandManager.RequerySuggested += value; }
            remove { commandManager.RequerySuggested -= value; }
        }
    }
}
