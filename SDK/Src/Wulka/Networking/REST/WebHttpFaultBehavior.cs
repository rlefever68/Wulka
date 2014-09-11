using System.ServiceModel.Description;

namespace Wulka.Networking.Rest
{
    public class WebHttpFaultBehavior : WebHttpBehavior
    {
        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new WebHttpFaultErrorHandler());
        }
    }
}
