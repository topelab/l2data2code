using System;

namespace L2Data2Code.SharedContext.Commands.Interfaces
{
    public interface ICommandManager
    {
        event EventHandler RequerySuggested;
        void InvalidateRequerySuggested();
    }
}
