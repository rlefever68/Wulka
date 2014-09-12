using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Base;
using Wulka.Validation;

namespace Wulka.Test.Domain
{
    [DataContract]
    public class Sideboard : DomainObject<Sideboard>
    {
        [DataMember]
        public string Material { get; set; }

        [DataMember]
        public string CoverMaterial { get; set; }

        [DataMember]
        public int NumberOfDrawers { get; set; }

        [DataMember]
        public int NumberOfShelfs { get; set; }

        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<Sideboard>.Validate(this, columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<Sideboard>.Validate(this);}
    }

}

    
