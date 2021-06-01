using L2Data2CodeUI.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace L2Data2CodeUI.Shared.Adapters
{
    public class MessageService : IMessageService
    {
        private string _warning;
        private string _error;

        protected Action<MessageType, string, string, string> UpdateAction { get; set; }
        protected Action<string> ClearAction { get; set; }

        public MessageService()
        {
        }

        public void SetActions(Action<MessageType, string, string, string> updateAction, Action<string> clearAction)
        {
            UpdateAction = updateAction;
            ClearAction = clearAction;
        }

        public void Error(string message, string showMessage = null, string code = null)
        {
            bool callToUpdate = !string.IsNullOrWhiteSpace(message);
            _error = message;
            if (callToUpdate)
            {
                UpdateAction?.Invoke(MessageType.Error, message, showMessage, code);
            }
        }

        public void Warning(string showMessage, string code = null, string message = null)
        {
            bool callToUpdate = !string.IsNullOrWhiteSpace(showMessage);
            _warning = showMessage;
            if (callToUpdate)
            {
                UpdateAction?.Invoke(MessageType.Warning, message ?? showMessage, showMessage, code);
            }
        }

        public void Info(string showMessage, string code = null, string message = null)
        {
            UpdateAction?.Invoke(MessageType.Info, message, showMessage, code);
        }

        public void Clear(string code)
        {
            ClearAction?.Invoke(code);
        }
    }
}
