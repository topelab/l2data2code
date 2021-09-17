using L2Data2Code.SharedLib.Configuration;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
using System.IO;

namespace L2Data2CodeUI.Shared.Adapters
{
    /// <summary>
    /// A basic service to manage git repo
    /// </summary>
    public class GitService : IGitService
    {
        private readonly IMessageService messageService;
        private readonly ICommandService commandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitService"/> class.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="commandService">The command service.</param>
        public GitService(IMessageService messageService, ICommandService commandService)
        {
            this.messageService = messageService;
            this.commandService = commandService;
        }

        /// <summary>
        /// Add files in <paramref name="path"/> to git repo.
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        public void GitAdd(string path)
        {
            var command = new Command
            {
                Name = "git add",
                Directory = path,
                Exec = "git add -A",
                ShowMessages = false,
                ShowMessageWhenExitCodeNotZero = false
            };
            GitAction(command);
        }

        /// <summary>
        /// Commit to git repo on <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        public void GitCommit(string path)
        {
            var command = new Command
            {
                Name = "git commit",
                Directory = path,
                Exec = "git add -A && git commit -a -m \"Automated internal commit by L2Data2CodeWPF\"",
                ShowMessages = false,
                ShowMessageWhenExitCodeNotZero = false
            };
            GitAction(command);
        }

        /// <summary>
        /// Reset unstahed files.
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        public void GitReset(string path)
        {
            var command = new Command
            {
                Name = "git reset",
                Directory = path,
                Exec = "git reset --hard",
                ShowMessages = false,
                ShowMessageWhenExitCodeNotZero = false
            };
            GitAction(command);
        }

        /// <summary>
        /// Initialize git repo on <paramref name="path"/>
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        public void GitInit(string path)
        {
            if (!Directory.Exists(Path.Combine(path, ".git")))
            {
                var command = new Command
                {
                    Name = "git init",
                    Directory = path,
                    Exec = "git init",
                    ShowMessageWhenExitCodeNotZero = false,
                    ShowMessages = false
                };
                GitAction(command);
            }
        }

        private void GitAction(Command command)
        {
            if (!Directory.Exists(command.Directory))
            {
                messageService.Info(Messages.NoGitRepo, MessageCodes.RUN_COMMAND);
            }
            commandService.Exec(command);
        }
    }
}
