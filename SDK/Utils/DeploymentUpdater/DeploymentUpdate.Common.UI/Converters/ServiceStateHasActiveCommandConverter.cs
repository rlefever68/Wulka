using System;
using System.Globalization;
using System.Windows.Data;
using DeploymentUpdate.DTO;
using System.ServiceProcess;

namespace DeploymentUpdater.Common.UI.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class ServiceStateHasActiveCommandConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch ((ServiceControllerStatus)value)
                {

                    case ServiceControllerStatus.Stopped:
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.Running:
                        {
                            return true;
                        }// break;
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.StopPending:
                    default:
                        {
                            return false;
                        }// break;
                }
                throw new InvalidCastException();
            }
            catch (Exception)
            {
                throw new InvalidCastException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
