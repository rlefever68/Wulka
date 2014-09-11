using System;
using System.Runtime.Serialization;

namespace Wulka.Domain.Interfaces
{
    public interface ILink : IEcoObject
    {
        [DataMember]
        IDomainObject Source { get; set; }
        [DataMember]
        IDomainObject Target { get; set; }
        [DataMember]
        string TargetId { get; set; }
        [DataMember]
        string SourceId { get; set; }
        [DataMember]
        string RelationType { get; set; }
        [DataMember]
        bool IsActive { get; set; }

        event EventHandler OnGetSource;
        event EventHandler OnGetTarget;
    }

    

    




}