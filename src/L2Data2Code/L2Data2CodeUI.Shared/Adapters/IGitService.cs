namespace L2Data2CodeUI.Shared.Adapters
{
    /// <summary>
    /// Defines implementation for GitService, a basic service to manage git repo
    /// </summary>
    public interface IGitService
    {
        /// <summary>
        /// Add files in <paramref name="path"/> to git repo.
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        void GitAdd(string path);
        /// <summary>
        /// Commit to git repo on <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        void GitCommit(string path);
        /// <summary>
        /// Initialize git repo on <paramref name="path"/>
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        void GitInit(string path);
        /// <summary>
        /// Executes a Git pull operation in the specified directory.
        /// </summary>
        /// <remarks>This method performs a Git fetch followed by a Git pull, updating the local
        /// repository with changes from the remote. Ensure that the specified directory is a valid Git
        /// repository.</remarks>
        /// <param name="path">The file system path to the directory where the Git pull operation will be executed. Must not be null or
        /// empty.</param>
        void GitPull(string path);
        /// <summary>
        /// Reset unstahed files.
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        void GitReset(string path);
    }
}