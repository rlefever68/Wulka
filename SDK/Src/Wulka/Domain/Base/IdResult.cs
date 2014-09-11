using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Validation;

namespace Wulka.Domain.Base
{
    public class IdResult : DomainObject<IdResult>
    {
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<IdResult>.Validate(this, columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<IdResult>.Validate(this);
        }
    }

    /// <summary>
    /// Use only with Id's of numeric type [int, long, decimal]
    /// </summary>
    /// <typeparam name="TId">Type of numeric</typeparam>
    public class IdResult<TId> : DomainObject<IdResult<TId>>
    {
        [DataMember]
        public new TId Id { get; set; }

        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<IdResult<TId>>.Validate(this, columnName);
        }

        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<IdResult<TId>>.Validate(this);
        }
    }
}
