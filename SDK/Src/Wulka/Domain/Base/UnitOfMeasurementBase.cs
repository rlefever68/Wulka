using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Validation;

namespace Wulka.Domain.Base
{
    [DataContract]
    public abstract class UnitOfMeasurementBase : TaxonomyObject<UnitOfMeasurementBase>
    {
        [DataMember]
        public string SymbolPrefix { get; set; }
        [DataMember]
        public string Symbol { get; set; }
        [DataMember]
        public int Exponent { get; set; }
        [DataMember]
        public bool IsIso { get; set; }


        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<UnitOfMeasurementBase>.Validate(this, columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<UnitOfMeasurementBase>.Validate(this);
        }
    }
}