using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using Wulka.Configuration;

namespace Wulka.Networking.Wcf
{
    public static class DiscoveryHelper
    {
        /// <summary>
        /// Enables the discovery.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void EnableDiscovery(this ServiceHostBase host)
        {
            var announcementEndpointUrl = ConfigurationHelper.CloudAnnounce;
            if (String.IsNullOrWhiteSpace(announcementEndpointUrl))
            {
                var errorMessage = string.Format(
                    "No value found for key '{0}' in configuration file"
                    + ", please provide a key '{0}' in the section AppConfig and set its value to the appropriate announcement endpoint url",
                    ConfigurationHelper.CloudAnnounce
                    );
                throw new ApplicationException(errorMessage);
            }

            var announcementEndpoint = new AnnouncementEndpoint(
                BindingFactory.CreateBindingFromKey(BindingFactory.Key.WsHttpBindingNoSecurity), 
                new EndpointAddress(announcementEndpointUrl));

            var discovery = new ServiceDiscoveryBehavior();
            discovery.AnnouncementEndpoints.Add(announcementEndpoint);
            host.Description.Behaviors.Add(discovery);
        }

        /// <summary>
        /// Enables the UDP discovery.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void EnableUdpDiscovery(this ServiceHostBase host)
        {
            host.AddServiceEndpoint(new UdpAnnouncementEndpoint());
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
        }




    }
}


