using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;
using Wulka.Validation;

namespace Wulka.Domain.Base
{
    [DataContract]
    public class Parameter : ComposedObject<Parameter>, IParameter
    {
        [DataMember]
        public UnitOfMeasurementBase Unit { get; set; }

        [DataMember]
        public ScopeEnum Scope { get; set; }
        [DataMember]
        public object Value { get; set; }


        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<Parameter>.Validate(this, columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<Parameter>.Validate(this);
        }
    }

    public enum ScopeEnum
    {
        Private = 0,
        Protected = 1,
        Public = 2
    }
}
