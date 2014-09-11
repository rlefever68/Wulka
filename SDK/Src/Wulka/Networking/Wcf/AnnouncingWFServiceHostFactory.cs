using System;
using System.Activities;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Activation;

namespace Wulka.Networking.Wcf
{
    public class AnnouncingWFServiceHostFactory : WorkflowServiceHostFactory
    {
        protected override WorkflowServiceHost CreateWorkflowServiceHost(Activity activity, Uri[] baseAddresses)
        {
            var serviceHost = new WorkflowServiceHost(activity, baseAddresses);
            serviceHost.MakeAnnouncingService();
            return serviceHost;
        }


        protected override WorkflowServiceHost CreateWorkflowServiceHost(WorkflowService service, Uri[] baseAddresses)
        {
            var serviceHost = new WorkflowServiceHost(service, baseAddresses);
            serviceHost.MakeAnnouncingService();
            return serviceHost;
        }
    }
}
