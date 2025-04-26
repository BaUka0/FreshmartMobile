using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Project.Models
{
    class StatusToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 4 || !(values[0] is string status))
                return Colors.Gray;

            if (values[1] is not Color acceptedColor || values[2] is not Color rejectedColor || values[3] is not Color pendingColor)
                return Colors.Gray;

            switch (status)
            {
                case "Accepted":
                    return acceptedColor;
                case "Rejected":
                    return rejectedColor;
                case "Pending":
                    return pendingColor;
                default:
                    return Colors.Gray;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
