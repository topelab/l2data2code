using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using L2Data2CodeWPF.Main;
using System.ComponentModel;
using System.Linq;

namespace L2Data2CodeWPF.Controls.MessagePanel
{
    internal class MessagePanelBindManager : IMessagePanelBindManager
    {
        private MainWindowVM mainVM;
        private MessagePanelVM controlVM;

        private readonly IMessagePanelService messagePanelService;
        private readonly IMessageService messageService;

        public MessagePanelBindManager(IMessagePanelService messagePanelService, IMessageService messageService)
        {
            this.messagePanelService = messagePanelService ?? throw new System.ArgumentNullException(nameof(messagePanelService));
            this.messageService = messageService ?? throw new System.ArgumentNullException(nameof(messageService));
        }

        public void Start(MainWindowVM mainVM, MessagePanelVM controlVM)
        {
            Stop();
            this.mainVM = mainVM;
            this.controlVM = controlVM;
            messageService.SetActions(ShowMessage, ClearMessages);
            mainVM.PropertyChanged += OnParentVMPropertyChanged;
            controlVM.PropertyChanged += OnControlVMPropertyChanged;
            messagePanelService.AllMessages.CollectionChanged += OnAllMessagesCollectionChanged;
        }

        public void Stop()
        {
            if (mainVM != null)
            {
                mainVM.PropertyChanged -= OnParentVMPropertyChanged;
            }
        }

        private void OnControlVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(controlVM.MessagePanelOpened):
                    messagePanelService.ViewAll(controlVM.MessagePanelOpened);
                    break;
                default:
                    break;
            }
        }

        private void OnAllMessagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            controlVM.MessagePanelVisible = messagePanelService.AllMessages.Any();
        }

        private void OnParentVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainWindowVM.Working):
                    break;
                case nameof(MainWindowVM.ShowVarsWindow):
                    break;
                case nameof(MainWindowVM.HaveVSCodeInstalled):
                    break;
                case nameof(MainWindowVM.HavePSInstalled):
                    break;
                case (nameof(MainWindowVM.VSCodePath)):
                    break;
                case (nameof(MainWindowVM.SlnFile)):
                    break;
                case nameof(MainWindowVM.AppType):
                    break;
                default:
                    break;
            }
        }

        private void ShowMessage(MessageType messageType, string message, string showMessage, string code)
        {
            if (showMessage.NotEmpty())
            {
                messagePanelService.Add(showMessage, controlVM.MessagePanelOpened, code);
            }

            if (message.NotEmpty())
            {
                switch (messageType)
                {
                    case MessageType.Info:
                        App.Logger.Info(message);
                        break;
                    case MessageType.Warning:
                        App.Logger.Warn(message);
                        break;
                    case MessageType.Error:
                        App.Logger.Error(message);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ClearMessages(string code)
        {
            messagePanelService.ClearPinned(code);
        }

    }
}
