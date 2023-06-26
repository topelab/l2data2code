using L2Data2Code.SharedContext.Commands.Interfaces;
using System;
using System.Windows.Input;

namespace L2Data2CodeWPF.SharedLib
{
    internal class CommandManagerWrapper : ICommandManager
    {

        public event EventHandler RequerySuggested
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void InvalidateRequerySuggested()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
