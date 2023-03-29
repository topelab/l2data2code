using System;
using System.Windows;
using System.Windows.Threading;

namespace L2Data2Code.Base
{
    /// <summary>
    /// Interface for a wrapper to dispatcher
    /// </summary>
    internal class DispatcherWrapper : IDispatcherWrapper
    {
        private readonly Lazy<Dispatcher> dispatcher;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dispatcher">Defined dispatcher or null to get current applicarion dispatcher</param>
        public DispatcherWrapper(Dispatcher dispatcher = null)
        {
            this.dispatcher = new Lazy<Dispatcher>(() => dispatcher ?? Application.Current?.Dispatcher);
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

            dispatcher.Value?.Invoke(action, args);
        }
    }
}
