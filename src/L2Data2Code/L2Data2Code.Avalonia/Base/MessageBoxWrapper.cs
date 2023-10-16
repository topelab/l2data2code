using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using L2Data2Code.SharedContext.Base;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace L2Data2Code.Avalonia.Base
{
    internal class MessageBoxWrapper : IMessageBoxWrapper
    {
        public async Task<MessageBoxWrapperResult> Show(string messageBoxText, string caption, MessageBoxWrapperButton button, MessageBoxWrapperImage icon)
        {
            var result = MessageBoxWrapperResult.Cancel;
            var messageBos = MessageBoxManager.GetMessageBoxStandard(caption, messageBoxText, MapButton(button), MapIcon(icon));
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                result = MapResult(await messageBos.ShowWindowDialogAsync(desktop.MainWindow)); ;
            }

            return result;
        }

        private MessageBoxWrapperResult MapResult(ButtonResult buttonResult)
        {
            MessageBoxWrapperResult result;
            switch (buttonResult)
            {
                case ButtonResult.Ok:
                    result = MessageBoxWrapperResult.OK;
                    break;
                case ButtonResult.Yes:
                    result = MessageBoxWrapperResult.Yes;
                    break;
                case ButtonResult.No:
                    result = MessageBoxWrapperResult.No;
                    break;
                default:
                    result = MessageBoxWrapperResult.Cancel;
                    break;
            }
            return result;
        }

        private Icon MapIcon(MessageBoxWrapperImage icon)
        {
            Icon result;

            switch (icon)
            {
                case MessageBoxWrapperImage.None:
                    result = Icon.None;
                    break;
                case MessageBoxWrapperImage.Error:
                    result = Icon.Error;
                    break;
                case MessageBoxWrapperImage.Question:
                    result = Icon.Question;
                    break;
                case MessageBoxWrapperImage.Warning:
                    result = Icon.Warning;
                    break;
                case MessageBoxWrapperImage.Information:
                    result = Icon.Info;
                    break;
                default:
                    result = Icon.None;
                    break;
            }

            return result;
        }

        private ButtonEnum MapButton(MessageBoxWrapperButton button)
        {
            var result = ButtonEnum.Ok;

            switch (button)
            {
                case MessageBoxWrapperButton.OK:
                    result = ButtonEnum.Ok;
                    break;
                case MessageBoxWrapperButton.OKCancel:
                    result = ButtonEnum.OkCancel;
                    break;
                case MessageBoxWrapperButton.YesNoCancel:
                    result = ButtonEnum.YesNoCancel;
                    break;
                case MessageBoxWrapperButton.YesNo:
                    result = ButtonEnum.YesNo;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
