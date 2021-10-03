using System;

namespace L2Data2CodeWPF.SharedLib
{
    /// <summary>
    /// Interface to Dsipatcher Wrapper
    /// </summary>
    public interface IDispatcherWrapper
    {
        /// <summary>
        /// Invoke an action
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="args">Arguments to pass to action</param>
        void Invoke(Action action, params object[] args);
    }
}
