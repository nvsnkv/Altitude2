using System;
using Windows.UI.Xaml.Data;

namespace NV.Altitude2.Tracker.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public string Format { get; set; } = "{0}";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null ? string.Format(Format, value) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}