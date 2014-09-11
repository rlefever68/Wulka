namespace Wulka.BigD.Contract.Interfaces
{
    public interface IBigDViewDefinitionBase
    {
        IBigDDatabase Db();
        BigDDesignDocument Doc { get; set; }
        string Name { get; set; }
        string Path();
        IBigDRequest Request();
    }
}
