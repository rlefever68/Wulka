using Wulka.Domain;
using Wulka.Domain.Interfaces;

namespace Wulka.Interfaces
{
    public interface IProvider<T>
        where T : IDomainObject, new()
    {
        T[] Save(T[] items);
        T Save(T item);
        T GetById(string id);
        T[] GetAll();

        T Delete(T item);
        T Delete(string id);
        T[] Delete(T[] items);

        T[] Query(RequestBase req);
        int Count(RequestBase req);

        byte[] ReadAttachment(T item, string name);
        byte[] ReadAttachment(string id, string name);
        bool HasAttachment(T item, string name);
        bool HasAttachment(string id, string name);
       
    }
}