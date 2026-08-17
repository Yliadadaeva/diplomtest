using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NovosibirskForestRegistry
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;

            if (string.IsNullOrEmpty(status))
                return new SolidColorBrush(Color.FromRgb(158, 158, 158));

            switch (status)
            {
                case "Охраняется":
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
                case "Требует внимания":
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Оранжевый
                case "Критическое":
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Красный
                default:
                    return new SolidColorBrush(Color.FromRgb(158, 158, 158)); // Серый
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}