using System;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Execute actions over paths and files
    /// </summary>
    internal class FileExecutor : IFileExecutor
    {
        private string[] files;
        private string[] dirs;

        /// <summary>
        /// Initialize root path
        /// </summary>
        /// <param name="rootPath">Root path</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Initialize(string rootPath)
        {
            rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
            dirs = Directory.GetDirectories(rootPath, "*.*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Run action over files
        /// </summary>
        /// <param name="actionForFiles">Action to execute over files</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RunOnFiles(Action<string> actionForFiles)
        {
            actionForFiles = actionForFiles ?? throw new ArgumentNullException(nameof(actionForFiles));
            foreach (var item in files)
            {
                actionForFiles.Invoke(item);
            }
        }

        /// <summary>
        /// Run action over paths
        /// </summary>
        /// <param name="actionForPaths">Action to execute over paths</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RunOnPaths(Action<string> actionForPaths)
        {
            actionForPaths = actionForPaths ?? throw new ArgumentNullException(nameof(actionForPaths));
            foreach (var item in dirs)
            {
                actionForPaths.Invoke(item);
            }
        }
    }
}