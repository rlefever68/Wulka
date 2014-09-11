using System;
using System.ServiceModel.Configuration;

namespace Wulka.Networking.Rest
{
    public class WebHttpFaultingBehaviorElement: BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof (WebHttpFaultBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new WebHttpFaultBehavior();
        }
    }
}
