using System;
using System.Globalization;
using System.Windows.Data;

namespace DeploymentUpdater.Common.UI.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class BooleanToVisibilityConverter : IValueConverter
    {        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var converted = (bool)value;
                if (converted)
                { return System.Windows.Visibility.Visible; }
                else
                { return System.Windows.Visibility.Collapsed; }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
