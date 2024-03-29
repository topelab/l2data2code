using L2Data2Code.SharedLib.Extensions;
using L2Data2CodeUI.Shared.Adapters;
using L2Data2CodeUI.Shared.Dto;
using NLog;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace L2Data2Code.SharedContext.Main.MessagePanel
{
    public class MessagePanelChangeListener : IMessagePanelChangeListener
    {
        private MainWindowVM mainVM;
        private MessagePanelVM controlVM;

        private readonly IMessagePanelService messagePanelService;
        private readonly IMessageService messageService;
        private readonly ILogger logger;

        public MessagePanelChangeListener(IMessagePanelService messagePanelService, IMessageService messageService, ILogger logger)
        {
            this.messagePanelService = messagePanelService ?? throw new System.ArgumentNullException(nameof(messagePanelService));
            this.messageService = messageService ?? throw new System.ArgumentNullException(nameof(messageService));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public void Start(MainWindowVM mainVM, MessagePanelVM controlVM)
        {
            Stop();
            this.mainVM = mainVM;
            this.controlVM = controlVM;
            messageService.SetActions(ShowMessage, ClearMessages);
            mainVM.PropertyChanged += OnMainVMPropertyChanged;
            controlVM.PropertyChanged += OnControlVMPropertyChanged;
            messagePanelService.AllMessages.CollectionChanged += OnAllMessagesCollectionChanged;
        }

        public void Stop()
        {
            if (mainVM != null)
            {
                mainVM.PropertyChanged -= OnMainVMPropertyChanged;
            }
            if (controlVM != null)
            {
                controlVM.PropertyChanged -= OnControlVMPropertyChanged;
            }
            messagePanelService.AllMessages.CollectionChanged -= OnAllMessagesCollectionChanged;
        }

        private void OnControlVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(controlVM.MessagePanelVisible):
                    messagePanelService.ViewAll(controlVM.MessagePanelVisible);
                    break;
                default:
                    break;
            }
        }

        private void OnAllMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.Cast<MessageVM>())
                    {
                        controlVM.AllMessages.Insert(0, item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.Cast<MessageVM>())
                    {
                        controlVM.AllMessages.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    controlVM.AllMessages.Clear();
                    break;
                default:
                    break;
            }

            controlVM.MessagePanelVisible = messagePanelService.AllMessages.Any();
        }

        private void OnMainVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MainWindowVM.Working):
                    controlVM.Working = mainVM.Working;
                    break;
                case nameof(MainWindowVM.RunningGenerateCode):
                    controlVM.RunningGenerateCode = mainVM.RunningGenerateCode;
                    if (mainVM.RunningGenerateCode)
                    {
                        controlVM.MessagePanelVisible = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ShowMessage(MessageType messageType, string message, string showMessage, string code)
        {
            if (showMessage.NotEmpty())
            {
                messagePanelService.Add(showMessage, controlVM.MessagePanelVisible, code);
            }

            if (message.NotEmpty())
            {
                switch (messageType)
                {
                    case MessageType.Info:
                        logger.Info(message);
                        break;
                    case MessageType.Warning:
                        logger.Warn(message);
                        break;
                    case MessageType.Error:
                        logger.Error(message);
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
