using System.Runtime.Serialization;

namespace Wulka.Domain.Interfaces
{
    public interface IValueItem : IParameter
    {
        [DataMember]
        IFormula Formula { get; set; }
        [DataMember]
        string SubjectId { get; set; }
    }
}