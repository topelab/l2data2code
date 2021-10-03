using System;
using System.Windows;
using System.Windows.Threading;

namespace L2Data2CodeWPF.SharedLib
{
    internal class DispatcherWrapper : IDispatcherWrapper
    {
        private readonly Dispatcher dispatcher;

        public DispatcherWrapper(Dispatcher dispatcher = null)
        {
            this.dispatcher = dispatcher ?? Application.Current.Dispatcher;
        }

        public void Invoke(Action action, params object[] args)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            dispatcher.Invoke(action, args);
        }
    }
}
