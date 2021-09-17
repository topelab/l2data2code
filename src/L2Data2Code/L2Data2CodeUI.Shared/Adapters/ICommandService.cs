using L2Data2Code.SharedLib.Configuration;
using L2Data2CodeUI.Shared.Dto;
using System.Collections.Generic;

namespace L2Data2CodeUI.Shared.Adapters
{
    /// <summary>
    /// Interface to implementation of the system command executor service
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="compiledVars">The compiled vars.</param>
        void Exec(Command command, Dictionary<string, object> compiledVars = null);
    }
}