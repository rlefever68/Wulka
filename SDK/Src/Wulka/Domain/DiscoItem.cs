using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Base;
using Wulka.Validation;

namespace Wulka.Domain
{
    [DataContract]
    public class DiscoItem : DomainObject<DiscoItem>
    {

        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<DiscoItem>.Validate(this, columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<DiscoItem>.Validate(this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            DiscoItem di = (obj as DiscoItem);
            if(di==null) return false;
            return (di.Endpoint == this.Endpoint) &&
                (di.Contract == this.Contract);
        }


        public override int GetHashCode()
        {
            return
            (((Id == null) ? 0 : Id.GetHashCode())
            ^ ((Endpoint == null) ? 0 : Endpoint.GetHashCode())
            ^ ((Contract == null) ? 0 : Contract.GetHashCode())
            );
        }


        [DataMember]
        public string Endpoint { get; set; }

        [DataMember]
        public string Contract { get; set; }

       

    }
}
