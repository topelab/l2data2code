using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using L2Data2Code.Base;
using L2Data2Code.Commands.Interfaces;
using L2Data2Code.Main.Vars;
using System;

namespace L2Data2Code.Commands
{
    internal class OpenVarsWindowCommand : DelegateCommand, IOpenVarsWindowCommand
    {
        private readonly IVarsFactory varsFactory;

        public OpenVarsWindowCommand(IVarsFactory varsFactory)
        {
            this.varsFactory = varsFactory ?? throw new ArgumentNullException(nameof(varsFactory));
        }

        public override void Execute(object parameter)
        {
            var varsWindow = varsFactory.Create();
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                varsWindow.ShowDialog(desktop.MainWindow);
            }
        }
    }
}
