namespace Wulka.Domain.Interfaces
{
    public interface IObjectFactory
    {
        IDomainObject GetById(string id);
    }
}