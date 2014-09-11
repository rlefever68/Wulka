namespace Wulka.Domain.Interfaces
{
    public interface ITranslatable : IDomainObject
    {
        string DataCulture { get; set; }
        IDescription Description {get;set;}
    }
}
