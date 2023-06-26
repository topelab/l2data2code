using System;

namespace L2Data2Code.SharedContext.Main.MessagePanel
{
    public class MessagePanelFactory : IMessagePanelFactory
    {
        private readonly IMessagePanelChangeListener bindManager;

        public MessagePanelFactory(IMessagePanelChangeListener bindManager)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
        }

        public MessagePanelVM Create(MainWindowVM mainVM)
        {
            MessagePanelVM messagePanelVM = new();
            bindManager.Start(mainVM, messagePanelVM);
            return messagePanelVM;
        }
    }
}
