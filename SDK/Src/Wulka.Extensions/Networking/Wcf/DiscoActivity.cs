using System;
using System.Activities;
using System.ServiceModel.Description;
using Microsoft.VisualStudio.Activities.Asr;

namespace Pms.Framework.Networking.Wcf
{
    class DiscoActivity
    {
        public DiscoActivity() 
        {
            OperationDescription descr = GetOperationDescription();
            string configName = String.Empty;
            string proxyNamespace = String.Empty;
            var cltt = new ClientActivityBuilder( descr, configName, proxyNamespace);
            ActivityBuilder bld = cltt.Build();
        }

        private OperationDescription GetOperationDescription()
        {
            return null;
        }
    }
}
