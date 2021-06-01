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
        /// Reset unstahed files.
        /// </summary>
        /// <param name="path">Path where git repo resides.</param>
        void GitReset(string path);
    }
}