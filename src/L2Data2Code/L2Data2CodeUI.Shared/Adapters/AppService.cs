using L2Data2Code.BaseGenerator.Entities;
using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeUI.Shared.Localize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace L2Data2CodeUI.Shared.Adapters
{
    public class AppService : IAppService
    {
        public const string FILE_PATTERN = "{file}";
        public const string DIRECTORY_PATTERN = "{directory}";
        public const string PARENT_PATTERN = "{parent}";
        private AppDto AppDto { get; set; } = new AppDto();
        private readonly IMessageService messageService;

        public AppType AppType => AppDto.AppType;

        public AppService(IMessageService messageService)
        {
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        }

        public AppService Set(string solutionType)
        {
            AppDto defaultAppDto = new();

            if (solutionType.IsEmpty())
            {
                return this;
            }

            var values = solutionType.Split(',');

            AppDto.CommandArguments = values.Length > 3 ? values[3].Trim() : defaultAppDto.CommandArguments;
            AppDto.CommandLine = values.Length > 2 ? values[2].Trim() : defaultAppDto.CommandLine;
            AppDto.SearchExpression = values.Length > 1 ? values[1].Trim() : defaultAppDto.SearchExpression;
            AppDto.AppType = values.Length > 0 ? (Enum.TryParse(typeof(AppTypeAbbr), values[0].Trim(), out var result) ? (AppType)((int)result) : AppType.Undefined) : defaultAppDto.AppType;

            return this;
        }

        public IEnumerable<string> Find(string path)
        {
            List<string> emptyList = new();
            if (AppDto.SearchExpression.IsEmpty() || path.IsEmpty())
            {
                return emptyList;
            }

            try
            {
                messageService.Clear(MessageCodes.FIND_SERVICE);
                DirectoryInfo directoryInfo = new(path);
                return directoryInfo.GetFiles(AppDto.SearchExpression, SearchOption.AllDirectories).Select(f => f.FullName);
            }
            catch (Exception ex)
            {
                messageService.Warning(string.Format(Messages.ErrorFindPath, path), MessageCodes.FIND_SERVICE, $"AppService.Find({path}): {ex.Message}");
                return emptyList;
            }
        }

        public void Open(string file)
        {
            Open(file, AppDto.CommandLine, AppDto.CommandArguments);
        }

        public void Open(string file, string commandLinePattern, string commandArgumentsPattern)
        {
            if (file.IsEmpty())
            {
                messageService.Error($"AppService.Open(file): Argument {nameof(file)} is null or empty");
                throw new ArgumentNullException(nameof(file));
            }

            var cmdLine = commandLinePattern.IfNull(FILE_PATTERN);
            var cmdArguments = commandArgumentsPattern.IfNull(string.Empty);

            cmdLine = cmdLine
                .Replace(FILE_PATTERN, file)
                .Replace(DIRECTORY_PATTERN, Path.GetDirectoryName(file))
                .Replace(PARENT_PATTERN, Path.GetDirectoryName(Path.GetDirectoryName(file)));
            cmdArguments = cmdArguments
                .Replace(FILE_PATTERN, file)
                .Replace(DIRECTORY_PATTERN, Path.GetDirectoryName(file))
                .Replace(PARENT_PATTERN, Path.GetDirectoryName(Path.GetDirectoryName(file)));

            ProcessManager.Run(cmdLine, cmdArguments);
        }

    }
}
