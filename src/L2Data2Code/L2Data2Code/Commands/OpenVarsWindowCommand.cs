using L2Data2Code.Commands.Interfaces;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2Code.Base;
using System;
using L2Data2CodeWPF.Vars;

namespace L2Data2Code.Commands
{
    internal class OpenVarsWindowCommand : DelegateCommand, IOpenVarsWindowCommand
    {
        private readonly IGeneratorAdapter adapter;

        public OpenVarsWindowCommand(IGeneratorAdapter adapter)
        {
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        public override void Execute(object parameter)
        {
            VarsVM varsVM = new(adapter.CompiledVars);
            //VarsWindow varsWindow = new(varsVM);
            //varsWindow.ShowDialog();
        }
    }
}
