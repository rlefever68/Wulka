using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Wulka.Networking.Wcf
{

    public class ComposedServiceBehavior : IServiceBehavior
    {
        private readonly IInstanceProvider _instanceProvider;

        public ComposedServiceBehavior(Type serviceType, CompositionContainer container)
        {
            _instanceProvider = new ComposedInstanceProvider(serviceType, container);
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase dispatcher in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = dispatcher as ChannelDispatcher;

                if (channelDispatcher == null)
                    continue;

                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.InstanceProvider = _instanceProvider;
                }
            }
        }
    }


}
