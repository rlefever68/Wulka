using System;
using System.ComponentModel.Composition.Hosting;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Wulka.Networking.Wcf
{

    public class ComposedServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            CompositionContainer container = ComposedServiceHosts.CompositionContainer;

            if (container == null)
                throw new InvalidOperationException();

            return CreateComposedServiceHost(serviceType, baseAddresses, container);
        }

        protected virtual ComposedServiceHost CreateComposedServiceHost(Type serviceType, Uri[] baseAddresses, CompositionContainer container)
        {
            return new ComposedServiceHost(serviceType, container, baseAddresses);
        }
    }


}
