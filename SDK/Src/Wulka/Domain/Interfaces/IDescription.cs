namespace Wulka.Domain.Interfaces
{
    public interface IDescription : IComposedObject
    {
        string CultureId { get; set; }
        string ObjectId { get; set; }
    }
}