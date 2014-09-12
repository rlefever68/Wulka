using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;

namespace Wulka.Domain.Base
{
    [DataContract]
    public abstract class EcoObject<T> : DomainObject<T>,  IEcoObject where T:IDomainObject
    {
        private string _ecoSpaceId;

        /// <summary>
        /// Gets or sets the eco space identifier.
        /// </summary>
        /// <value>The eco space identifier.</value>
        [DataMember]
        public string EcoSpaceId
        {
            get { return _ecoSpaceId; }
            set { _ecoSpaceId = value; RaisePropertyChanged("EcoSpaceId"); }
        }





    }
}
