using L2Data2Code.CLIBase.Options;

namespace L2Data2Code.CLIBase.Interfaces
{
    /// <summary>
    /// Interface for an implementation that run a HundleBars action
    /// </summary>
    public interface ICLIAction
    {
        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="options">Command line options</param>
        void Initialize(ICLIOptions options);
        /// <summary>
        /// Run HundleBars action
        /// </summary>
        void Run();
    }
}