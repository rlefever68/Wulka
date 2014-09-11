using System;
using System.Runtime.Serialization;

namespace Wulka.Domain.Interfaces
{
    public interface IEvent : ITaxonomyObject
    {
        [DataMember]
        TimeSpan TimeSpan { get; set; }
    }
}