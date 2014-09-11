using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Base;
using Wulka.Validation;

namespace Wulka.Domain
{

    [DataContract]
    public class CloudContract : DomainObject<CloudContract>
    {
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<CloudContract>.Validate(this, columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<CloudContract>.Validate(this);
        }

        [DataMember]
        public string ContractName { get; set; }
        [DataMember]
        public string Publisher { get; set; }
        
        [DataMember]
        public string Binding { get; set; }
        
        [DataMember]
        public string Address { get; set; }


        public string Behaviour { get; set; }
        
        [DataMember]
        public DateTime RegistrationTimeStamp { get; set; }
        
        [DataMember]
        public string CommonName { get; set; }

    }
}
