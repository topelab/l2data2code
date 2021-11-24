using System;

namespace Mustache
{
    internal interface IFileExecutor
    {
        void Run(Action<string> actionForPaths, Action<string> actionForFiles);
    }
}