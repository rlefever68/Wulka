using System;
using System.Linq;
using System.Text;
using Wulka.Logging;

namespace Wulka.Utils
{
    /// <summary>
    /// Exposes methods and properties to build a WCF Service Connection
    /// </summary>
    public class ServiceConnectionStringBuilder
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConnectionStringBuilder"/> class.
        /// </summary>
        public ServiceConnectionStringBuilder()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConnectionStringBuilder"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ServiceConnectionStringBuilder(string connection)
        {
            var settings = connection.Split(new[] { ';' });
            var properties = typeof(ServiceConnectionStringBuilder).GetProperties();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var setting = settings.FirstOrDefault(a => a.StartsWith(propertyName));
                if (!string.IsNullOrEmpty(setting))
                {
                    var value = setting.Substring(setting.IndexOf('=') + 1);
                    if (!string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            if (property.PropertyType.Equals(typeof(TimeSpan)))
                            {
                                var propertyValue = TimeSpan.Parse(value);
                                property.SetValue(this, propertyValue, null);
                            }
                            else
                            {
                                var propertyValue = Convert.ChangeType(value, property.PropertyType);
                                property.SetValue(this, propertyValue, null);
                            }
                        }
                        catch (FormatException ex)
                        {
                            FxLog<ServiceConnectionStringBuilder>.WarnFormat("Unable to convert {0} to {1} : {2}", value, property.PropertyType,
                                         ex.Message);
                        }
                        catch (InvalidCastException ex)
                        {
                            FxLog<ServiceConnectionStringBuilder>.WarnFormat("Unable to convert {0} to {1} : {2}", value, property.PropertyType,
                                         ex.Message);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// This can be the Service Ip address or DNS
        /// </summary>
        /// <value>The data source.</value>
        public string DataSource { get; set; }

        /// <summary>
        /// Gets or sets the port the service .
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }

        private TimeSpan _openTimeout = new TimeSpan(0, 0, 30);

        /// <summary>
        /// Gets or sets the open timeout.
        /// </summary>
        /// <value>The open timeout.</value>
        public TimeSpan OpenTimeout
        {
            get { return _openTimeout; }
            set { _openTimeout = value; }
        }

        private TimeSpan _closeTimeout = new TimeSpan(0, 0, 30);

        /// <summary>
        /// Gets or sets the close timeout.
        /// </summary>
        /// <value>The close timeout.</value>
        public TimeSpan CloseTimeout
        {
            get { return _closeTimeout; }
            set { _closeTimeout = value; }
        }

        private TimeSpan _sendTimeout = new TimeSpan(0, 0, 30);

        /// <summary>
        /// Gets or sets the send timeout.
        /// </summary>
        /// <value>The send timeout.</value>
        public TimeSpan SendTimeout
        {
            get { return _sendTimeout; }
            set { _sendTimeout = value; }
        }

        private TimeSpan _receiveTimeout = new TimeSpan(0, 0, 30);

        /// <summary>
        /// Gets or sets the receive timeout.
        /// </summary>
        /// <value>The receive timeout.</value>
        public TimeSpan ReceiveTimeout
        {
            get { return _receiveTimeout; }
            set { _receiveTimeout = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var properties = typeof(ServiceConnectionStringBuilder).GetProperties();
            var sb = new StringBuilder();

            foreach (var property in properties)
            {
                var value = property.GetValue(this, null);
                if (value != null)
                {
                    if (sb.Length > 0) sb.Append(";");

                    sb.Append(property.Name).Append("=").Append(value);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts the Service ConnectionString to a Uri.
        /// </summary>
        /// <returns>Uri representing the service connection string</returns>
        public Uri ToUri()
        {
            var uri = string.Format("http://{0}:{1}", DataSource, Port);
            return new Uri(uri);
        }

        /// <summary>
        /// Converts the Service ConnectionString to a Uri.
        /// </summary>
        /// <returns>Uri representing the service connection string</returns>
        public Uri ToUri(string relative)
        {
            relative = relative.TrimStart(new[] { '/' });
            var uri = string.Format("http://{0}:{1}/{2}", DataSource, Port, relative);
            return new Uri(uri);
        }
    }
}