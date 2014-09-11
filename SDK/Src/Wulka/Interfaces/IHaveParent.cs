namespace Wulka.Interfaces
{
    public interface IHaveParent : IId
    {
        string ParentId { get; set; }
    }
}