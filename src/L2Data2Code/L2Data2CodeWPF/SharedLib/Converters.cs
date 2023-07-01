using L2Data2Code.SharedContext.Base;
using LambdaConverters;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace L2Data2CodeWPF.SharedLib
{
    internal static class Converters
    {
        public static readonly IValueConverter VisibleIfTrue =
            ValueConverter.Create<bool, Visibility>(e => e.Value ? Visibility.Visible : Visibility.Collapsed);

        public static readonly IValueConverter VisibleIfFalse =
            ValueConverter.Create<bool, Visibility>(e => !e.Value ? Visibility.Visible : Visibility.Collapsed);

        public static readonly IValueConverter VisibleIfNotNull =
            ValueConverter.Create<object, Visibility>(e => e.Value != null ? Visibility.Visible : Visibility.Collapsed);

        public static readonly IValueConverter VisibleIfNull =
            ValueConverter.Create<object, Visibility>(e => e.Value == null ? Visibility.Visible : Visibility.Collapsed);

        public static readonly IValueConverter VisibleIfLessThan =
            ValueConverter.Create<int, Visibility, int>(e => e.Value < e.Parameter ? Visibility.Visible : Visibility.Collapsed);

        public static readonly IValueConverter EnableIfFalse = ValueConverter.Create<bool, bool>(e => !e.Value);

        public static readonly IValueConverter ToUpperCase =
            ValueConverter.Create<string, string>(e => e.Value.ToUpper());

        public static readonly IValueConverter ToColor = ValueConverter.Create<MessageStatus, SolidColorBrush>(e =>
        {
            switch (e.Value)
            {
                case MessageStatus.Error:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        });
    }
}
