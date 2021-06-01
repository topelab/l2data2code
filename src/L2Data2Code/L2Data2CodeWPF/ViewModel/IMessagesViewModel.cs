using System.Collections.ObjectModel;

namespace L2Data2CodeWPF.ViewModel
{
    public interface IMessagesViewModel
    {
        ObservableCollection<MessageViewModel> AllMessages { get; }

        void Add(string text, bool viewStatus = false, string code = null);
        void ClearPinned(string code);
        void ViewAll(bool view);
    }
}