using System.Runtime.Serialization;

namespace Wulka.Domain.Interfaces
{
    public interface IEcoObject : ITaxonomyObject
    {
        /// <summary>
        /// Gets or sets the eco space identifier.
        /// </summary>
        /// <value>The eco space identifier.</value>
        [DataMember]
        string EcoSpaceId { get; set; }
    }
}