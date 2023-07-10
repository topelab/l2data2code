using L2Data2Code.SharedContext.Base;
using System.Windows;

namespace L2Data2CodeMaui.Base
{
    internal class MessageBoxWrapper : IMessageBoxWrapper
    {
        public Task<MessageBoxWrapperResult> Show(string messageBoxText, string caption, MessageBoxWrapperButton button, MessageBoxWrapperImage icon)
        {
            return Task.Run(() => (MessageBoxWrapperResult)(int)MessageBox.Show(messageBoxText, caption, (MessageBoxButton)(int)button, (MessageBoxImage)(int)icon));
        }
    }
}
