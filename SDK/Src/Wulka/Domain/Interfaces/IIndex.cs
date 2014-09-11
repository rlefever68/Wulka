using System.Collections.Generic;

namespace Wulka.Domain.Interfaces
{
    /// <summary>
    /// Interface IIndex
    /// </summary>
    public interface IIndex
    {
        void Register(IDomainObject domainObject);
        void Unregister(IDomainObject domainObject);
        IDomainObject Find(IDomainObject domainObject);
        IDomainObject Find(string id);
        T Find<T>(IDomainObject domainObject) where T : IDomainObject;
        T Find<T>(string id) where T : IDomainObject;
        IEnumerable<T> GetAll<T>();
        int Count<T>();
        int Count();
        IEnumerable<IDomainObject> GetAll();
        void Clear();
    }
}