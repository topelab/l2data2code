using L2Data2CodeWPF.Base;
using System.Collections.ObjectModel;

namespace L2Data2CodeWPF.Controls.MessagePanel
{
    public class MessagePanelVM : BaseVM
    {
        private bool messagePanelVisible;
        private bool runningGenerateCode;
        private ObservableCollection<MessageVM> allMessages;
        private bool messagePanelOpened;

        public MessagePanelVM()
        {
            AllMessages = new ObservableCollection<MessageVM>();
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
        public bool MessagePanelOpened
        {
            get => messagePanelOpened;
            set => SetProperty(ref messagePanelOpened, value);
        }
    }
}
