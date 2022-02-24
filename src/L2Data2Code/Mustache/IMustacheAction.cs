namespace Mustache
{
    /// <summary>
    /// Interface for an implemenation that run a Mustache action
    /// </summary>
    internal interface IMustacheAction
    {
        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="options">Mustache options</param>
        void Initialize(MustacheOptions options);
        /// <summary>
        /// Run Mustache action
        /// </summary>
        void Run();
    }
}