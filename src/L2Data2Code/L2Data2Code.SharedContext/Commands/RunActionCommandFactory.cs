using L2Data2Code.SharedContext.Base;
using L2Data2Code.SharedContext.Commands.Interfaces;
using System.Diagnostics;
using System.IO;

namespace L2Data2Code.SharedContext.Commands
{
    internal class RunActionCommandFactory : DelegateCommandFactory<string>, IRunActionCommandFactory
    {
        protected override bool CanExecute(string action)
        {
            return !string.IsNullOrEmpty(action) && File.Exists(action);
        }
        protected override void Execute(string action)
        {
            Process.Start(action);
        }
    }
}
