namespace Wulka.Interfaces
{
    /// <summary>
    /// Provides identity information
    /// </summary>
    public interface IIdentityInfo<TId>
    {
        TId Id { get; set; }
    }
}
