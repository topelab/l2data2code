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
                Color color;
                switch (messageStatus)
                {
                    case MessageStatus.Error:
                        color = Colors.Red;
                        break;
                    default:
                        color = Colors.White;
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
