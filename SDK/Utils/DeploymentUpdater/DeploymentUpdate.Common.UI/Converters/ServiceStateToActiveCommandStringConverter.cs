using System;
using System.Globalization;
using System.Windows.Data;
using DeploymentUpdate.DTO;
using System.ServiceProcess;

namespace DeploymentUpdater.Common.UI.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class ServiceStateToActiveCommandStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch ((ServiceControllerStatus)value)
                {
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.Stopped:
                        {
                            return "Start";
                        }// break;
                    case ServiceControllerStatus.Running:
                        {
                            return "Stop";
                        }// break;
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.StopPending:
                    default:
                        {
                            return "Wait";
                        }// break;
                }

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
