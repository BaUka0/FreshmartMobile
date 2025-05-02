using System.Globalization;

namespace Project.Converters
{
    public class ItemsToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                // Базовая высота на один элемент (настройте по необходимости)
                const int itemHeight = 60;
                const int minHeight = 60;
                const int maxHeight = 200;

                var totalHeight = count * itemHeight;
                return Math.Clamp(totalHeight, minHeight, maxHeight);
            }

            return 100; // Значение по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
