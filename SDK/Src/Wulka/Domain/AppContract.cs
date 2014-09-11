using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Base;
using Wulka.Domain.Interfaces;
using Wulka.Validation;

namespace Wulka.Domain
{
    [DataContract]
    public class AppContract : TaxonomyObject<AppContract>, IAppContract
    {
        [DataMember]
        public string Publisher { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string TechnicalName { get; set; }

        [DataMember]
        public string Layer { get; set; }

        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<AppContract>.Validate(this, columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<AppContract>.Validate(this);
        }

    }
}
