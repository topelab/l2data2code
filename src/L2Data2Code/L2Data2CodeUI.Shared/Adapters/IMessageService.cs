using L2Data2CodeUI.Shared.Dto;
using System;

namespace L2Data2CodeUI.Shared.Adapters
{
    public interface IMessageService
    {
        void Clear(string code);
        void Error(string message, string showMessage = null, string code = null);
        void Info(string showMessage, string code = null, string message = null);
        void SetActions(Action<MessageType, string, string, string> updateAction, Action<string> clearAction);
        void Warning(string showMessage, string code = null, string message = null);
    }
}