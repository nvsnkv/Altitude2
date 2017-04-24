using System;
using Windows.UI.Xaml.Data;

namespace NV.Altitude2.Tracker.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                throw new ArgumentException("Unable to inverse non-boolean value!",nameof(value));
            }

            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                throw new ArgumentException("Unable to inverse non-boolean value!", nameof(value));
            }

            return !(bool)value;
        }
    }
}