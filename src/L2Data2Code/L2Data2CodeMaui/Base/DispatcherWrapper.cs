using L2Data2Code.SharedContext.Base;

namespace L2Data2CodeMaui.Base
{
    /// <summary>
    /// Interface for a wrapper to dispatcher
    /// </summary>
    internal class DispatcherWrapper : IDispatcherWrapper
    {
        private readonly Lazy<IDispatcher> dispatcher;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dispatcher">Defined dispatcher or null to get current application dispatcher</param>
        public DispatcherWrapper()
        {
            this.dispatcher = new Lazy<IDispatcher>(() => Dispatcher.GetForCurrentThread());
        }

        /// <summary>
        /// Invoke an action
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="args">Arguments to pass to action</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Invoke(Delegate action, params object[] args)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            dispatcher.Value.Dispatch(() => action.DynamicInvoke(args));
        }
    }
}
