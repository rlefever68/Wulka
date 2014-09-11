using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Wulka.Networking.Wcf
{
    public class AnnouncingConfiguredServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type t, Uri[] baseAddresses)
        {
            var serviceHost = new ServiceHost(t, baseAddresses);
            serviceHost.MakeAnnouncingService();
            return serviceHost;
        }

    }
}
