using System.Collections.ObjectModel;

namespace L2Data2Code.SharedContext.Main.MessagePanel
{
    public interface IMessagePanelService
    {
        ObservableCollection<MessageVM> AllMessages { get; }

        void Add(string text, bool viewStatus = false, string code = null);
        void ClearPinned(string code);
        void ViewAll(bool view);
    }
}