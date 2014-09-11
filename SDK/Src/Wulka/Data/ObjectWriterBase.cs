using Wulka.Domain.Base;
using Wulka.Domain.Interfaces;

namespace Wulka.Data
{
    public abstract class ObjectWriterBase : IWriteEvents
    {
        public abstract void OnSaved<T>(DomainObject<T> domainObject) 
            where T : IDomainObject;
        public abstract bool OnDeleting<T>(DomainObject<T> domainObject)
            where T : IDomainObject;
        public abstract bool OnSaving<T>(DomainObject<T> domainObject) 
            where T : IDomainObject;
        public abstract void OnDeleted<T>(DomainObject<T> domainObject)
            where T : IDomainObject;
    }
}
