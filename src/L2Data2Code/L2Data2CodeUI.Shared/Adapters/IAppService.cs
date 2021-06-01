using L2Data2CodeUI.Shared.Dto;
using System.Collections.Generic;

namespace L2Data2CodeUI.Shared.Adapters
{
    public interface IAppService
    {
        AppType AppType { get; }

        IEnumerable<string> Find(string path);
        void Open(string file);
        void Open(string file, string commandLinePattern, string commandArgumentsPattern);
        AppService Set(string solutionType);
    }
}