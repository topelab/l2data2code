using Prism.Commands;
using System;

namespace L2Data2Code.SharedContext.Base
{
    public class DelegateCommandWrapper : DelegateCommand, IDelegateCommand
    {
        public DelegateCommandWrapper(Action executeMethod) : base(executeMethod)
        {
        }

        public DelegateCommandWrapper(Action executeMethod, Func<bool> canExecuteMethod) : base(executeMethod, canExecuteMethod)
        {
        }
    }

    public class DelegateCommandWrapper<T> : DelegateCommand<T>, IDelegateCommand
    {
        public DelegateCommandWrapper(Action<T> executeMethod) : base(executeMethod)
        {
        }

        public DelegateCommandWrapper(Action<T> executeMethod, Func<T, bool> canExecuteMethod) : base(executeMethod, canExecuteMethod)
        {
        }
    }
}
