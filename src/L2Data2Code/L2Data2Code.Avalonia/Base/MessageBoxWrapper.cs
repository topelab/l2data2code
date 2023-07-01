using L2Data2Code.SharedContext.Base;
using System.Windows;

namespace L2Data2Code.Avalonia.Base
{
    internal class MessageBoxWrapper : IMessageBoxWrapper
    {
        public MessageBoxWrapperResult Show(string messageBoxText, string caption, MessageBoxWrapperButton button, MessageBoxWrapperImage icon)
        {
            return (MessageBoxWrapperResult)(int)MessageBox.Show(messageBoxText, caption, (MessageBoxButton)(int)button, (MessageBoxImage)(int)icon);
        }
    }
}
