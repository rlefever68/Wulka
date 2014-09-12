using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Base;
using Wulka.Validation;

namespace Wulka.Test.Domain
{
    public class Upholstry : DomainObject<Upholstry>
    {

        [DataMember]
        public string Material { get; set; }

        [DataMember]
        public string Color { get; set; }


        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<Upholstry>.Validate(this,columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<Upholstry>.Validate(this);
        }
    }
}