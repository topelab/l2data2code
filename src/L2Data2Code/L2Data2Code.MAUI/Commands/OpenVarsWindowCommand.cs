using L2Data2Code.MAUI.Main.Vars;
using L2Data2Code.MAUI.Base;
using L2Data2Code.MAUI.Commands.Interfaces;

namespace L2Data2Code.MAUI.Commands
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
            varsWindow.Focus();
            // varsWindow.ShowDialog(desktop.MainWindow);
        }
    }
}
