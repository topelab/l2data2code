using System;

namespace L2Data2Code.CLIBase.Interfaces
{
    /// <summary>
    /// Interface for an implementation that execute actions over paths and files
    /// </summary>
    public interface IFileExecutor
    {
        /// <summary>
        /// Initialize root path
        /// </summary>
        /// <param name="rootPath">Root path</param>
        void Initialize(string templatePath);
        /// <summary>
        /// Run action over files
        /// </summary>
        /// <param name="actionForFiles">Action to execute over files</param>
        void RunOnFiles(Action<string> actionForFiles);
        /// <summary>
        /// Run action over paths
        /// </summary>
        /// <param name="actionForPaths">Action to execute over paths</param>
        void RunOnPaths(Action<string> actionForPaths);
    }
}