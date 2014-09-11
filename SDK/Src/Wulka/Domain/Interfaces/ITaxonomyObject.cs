using System.Runtime.Serialization;

namespace Wulka.Domain.Interfaces
{
    public interface ITaxonomyObject : IComposedObject
    {
        IHook Hook { get; }
        [DataMember]
        string HookId { get; set; }
        void GetHookAsync();
    }
}