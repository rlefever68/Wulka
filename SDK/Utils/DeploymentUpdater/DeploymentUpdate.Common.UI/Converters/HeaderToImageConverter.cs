using System;
using System.Globalization;
using System.Windows.Data;

namespace DeploymentUpdater.Common.UI.Converters
{
    #region HeaderToImageConverter

    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        String diskdrive = "/Deployment.ui;component/Images/diskdrive.png";
        String folder = "/Deployment.ui;component/Images/folder.png";
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as string).Contains(@"\"))
            {
                return diskdrive;
            }
            else
            {                
                return folder;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }

    #endregion // DoubleToIntegerConverter
}
