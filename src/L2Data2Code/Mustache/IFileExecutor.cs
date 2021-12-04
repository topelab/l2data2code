using System;

namespace Mustache
{
    internal interface IFileExecutor
    {
        void Initialize(string templatePath);
        void Run(Action<string> actionForPaths, Action<string> actionForFiles);
    }
}