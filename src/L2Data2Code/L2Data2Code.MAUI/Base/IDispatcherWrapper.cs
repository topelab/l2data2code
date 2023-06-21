using System;

namespace L2Data2Code.MAUI.Base
{
    /// <summary>
    /// Interface to Dispatcher Wrapper
    /// </summary>
    public interface IDispatcherWrapper
    {
        /// <summary>
        /// Invoke an action
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="args">Arguments to pass to action</param>
        void Invoke(Delegate action, params object[] args);
    }
}
