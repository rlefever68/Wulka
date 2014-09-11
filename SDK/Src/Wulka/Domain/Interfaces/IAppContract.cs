using System.Runtime.Serialization;

namespace Wulka.Domain.Interfaces
{
    public interface IAppContract : ITaxonomyObject
    {
        [DataMember]
        string Publisher { get; set; }

        [DataMember]
        string Version { get; set; }

        [DataMember]
        string Location { get; set; }

        [DataMember]
        string TechnicalName { get; set; }

        [DataMember]
        string Layer { get; set; }
    }
}