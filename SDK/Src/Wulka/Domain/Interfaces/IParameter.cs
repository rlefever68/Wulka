using System.Runtime.Serialization;
using Wulka.Domain.Base;

namespace Wulka.Domain.Interfaces
{
    public interface IParameter : IComposedObject
    {
        [DataMember]
        ScopeEnum Scope { get; set; }

        [DataMember]
        object Value { get; set; }
    }
}