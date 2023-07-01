using L2Data2Code.SharedContext.Base;
using System.Globalization;

namespace L2Data2Code.MAUI.Converters
{
    internal class MessageStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageStatus messageStatus)
            {
                var color = new SolidColorBrush(Colors.Black);

                switch (messageStatus)
                {
                    case MessageStatus.Ok:
                        color = new SolidColorBrush(Colors.Green);
                        break;
                    case MessageStatus.Error:
                        color = new SolidColorBrush(Colors.Red);
                        break;
                }

                return color;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
