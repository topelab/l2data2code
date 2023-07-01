using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using L2Data2Code.SharedContext.Base;
using System;
using System.Globalization;

namespace L2Data2Code.Avalonia.Converters
{
    internal class MessageStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageStatus messageStatus)
            {
                var color = new SolidColorBrush(Colors.White);

                switch (messageStatus)
                {
                    case MessageStatus.Ok:
                        color = new SolidColorBrush(Colors.White);
                        break;
                    case MessageStatus.Error:
                        color = new SolidColorBrush(Colors.Red);
                        break;
                }

                return color;
            }

            return BindingOperations.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
