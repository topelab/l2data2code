using System;

namespace L2Data2Code.SharedContext.Base
{
    public interface IDelegateCommandFactory
    {
        IDelegateCommand Create();
    }

    public interface IDelegateCommandFactory<T>
    {
        IDelegateCommand Create();
    }
}
