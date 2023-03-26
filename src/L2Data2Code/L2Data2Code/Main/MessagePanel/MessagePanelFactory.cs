using L2Data2Code.Main;
using System;

namespace L2Data2Code.Main.MessagePanel
{
    internal class MessagePanelFactory : IMessagePanelFactory
    {
        private readonly IMessagePanelBindManager bindManager;

        public MessagePanelFactory(IMessagePanelBindManager bindManager)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
        }

        public MessagePanelVM Create(MainWindowVM mainVM)
        {
            MessagePanelVM MessagePanelVM = new();
            bindManager.Start(mainVM, MessagePanelVM);
            return MessagePanelVM;
        }
    }
}
