namespace L2Data2Code.MAUI.Main.MessagePanel
{
    internal class MessagePanelFactory : IMessagePanelFactory
    {
        private readonly IMessagePanelBindManager bindManager;

        public MessagePanelFactory(IMessagePanelBindManager bindManager)
        {
            this.bindManager = bindManager ?? throw new ArgumentNullException(nameof(bindManager));
        }

        public MessagePanelVM Create(MainPageVM mainVM)
        {
            MessagePanelVM messagePanelVM = new();
            bindManager.Start(mainVM, messagePanelVM);
            return messagePanelVM;
        }
    }
}
