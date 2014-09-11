using System.Runtime.Serialization;

namespace Wulka.Networking.Rest
{
    [DataContract]
    public abstract class BaseFault
    {
        #region Properties
        ///
        /// The fault message
        ///
        [DataMember]
        public string Message
        {
            get;
            set;
        }
        #endregion
    }
}
