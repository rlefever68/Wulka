using System.Runtime.Serialization;

namespace Wulka.Networking.Rest
{
    public class WebHttpFault : BaseFault
    {

        ///
        /// The detail of the fault
        ///
        [DataMember]
        public BaseFault Detail
        {
            get;
            set;
        }
        ///
        /// The type of the fault
        ///
        [DataMember]
        public string FaultType
        {
            get;
            set;
        }
    }
}
