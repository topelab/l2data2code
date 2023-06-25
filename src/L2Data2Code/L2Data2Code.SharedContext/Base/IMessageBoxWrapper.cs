namespace L2Data2Code.SharedContext.Base
{
    public interface IMessageBoxWrapper
    {
        MessageBoxWrapperResult Show(string messageBoxText, string caption, MessageBoxWrapperButton button, MessageBoxWrapperImage icon);
    }

    public enum MessageBoxWrapperResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7
    }

    public enum MessageBoxWrapperButton
    {
        OK = 0,
        OKCancel = 1,
        YesNoCancel = 3,
        YesNo = 4
    }

    public enum MessageBoxWrapperImage
    {
        None = 0,
        Error = 16,
        Hand = 16,
        Stop = 16,
        Question = 32,
        Exclamation = 48,
        Warning = 48,
        Asterisk = 64,
        Information = 64
    }

}
