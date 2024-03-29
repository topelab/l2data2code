using L2Data2Code.SharedContext.Base;
using System.Collections.ObjectModel;

namespace L2Data2Code.SharedContext.Main.MessagePanel
{
    public class MessagePanelVM : ViewModelBase
    {
        private bool messagePanelVisible;
        private bool runningGenerateCode;
        private ObservableCollection<MessageVM> allMessages;

        public MessagePanelVM()
        {
            AllMessages = [];
        }

        public bool MessagePanelVisible
        {
            get => messagePanelVisible;
            internal set => SetProperty(ref messagePanelVisible, value);
        }
        public bool RunningGenerateCode
        {
            get => runningGenerateCode;
            internal set => SetProperty(ref runningGenerateCode, value);
        }
        public ObservableCollection<MessageVM> AllMessages
        {
            get => allMessages;
            internal set => SetProperty(ref allMessages, value);
        }
    }
}
