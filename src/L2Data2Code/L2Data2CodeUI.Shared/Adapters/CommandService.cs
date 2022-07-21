using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SharedLib.Extensions;
using L2Data2Code.SharedLib.Interfaces;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace L2Data2CodeUI.Shared.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandService : ICommandService
    {
        private readonly IMessageService messageService;
        private readonly IMustacheRenderizer mustacheRenderizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandService"/> class.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="mustacheRenderizer">The mustache rendeizer service</param>
        public CommandService(IMessageService messageService, IMustacheRenderizer mustacheRenderizer)
        {
            this.messageService = messageService;
            this.mustacheRenderizer = mustacheRenderizer;
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="compiledVars">The compiled vars.</param>
        public void Exec(Command command, Dictionary<string, object> compiledVars = null)
        {
            var directorio = compiledVars != null ? mustacheRenderizer.Render(command.Directory, compiledVars) : command.Directory;
            var exec = compiledVars != null ? mustacheRenderizer.Render(command.Exec, compiledVars) : command.Exec ;
            messageService.Info(string.Format(Messages.ParametrizedStartingProcess, command.Name));
            StringBuilder outputData = new();

            try
            {
                Process process = new();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = $"/c {exec}";
                process.StartInfo.CreateNoWindow = !command.ShowWindow;
                process.StartInfo.UseShellExecute = false;
                if (directorio.NotEmpty() && Directory.Exists(directorio))
                {
                    process.StartInfo.WorkingDirectory = directorio;
                }
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += (s, e) =>
                {
                    messageService.Error(e.Data, string.Format(Messages.ParametrizedErrorMessage, command.Name), MessageCodes.RUN_COMMAND);
                };
                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        if (command.ShowMessages)
                        {
                            messageService.Info(e.Data);
                        }
                        outputData.AppendLine(e.Data);
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                if (process.ExitCode > 0 && command.ShowMessageWhenExitCodeNotZero)
                {
                    messageService.Error(string.Format(Messages.ParametrizedErrorMessage, command.Name), outputData.ToString(), MessageCodes.RUN_COMMAND);
                }
                if (process.ExitCode == 0 && command.ShowMessageWhenExitCodeZero)
                {
                    messageService.Info(string.Format(Messages.ParametrizedStoppingProcess, command.Name));
                }
            }
            catch (Exception ex)
            {
                messageService.Error(ex.Message, string.Format(Messages.ParametrizedErrorMessage, command.Name), MessageCodes.RUN_COMMAND);
            }

        }
    }
}
