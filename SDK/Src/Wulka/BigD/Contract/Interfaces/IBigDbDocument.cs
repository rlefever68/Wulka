using Wulka.Interfaces;

namespace Wulka.BigD.Contract.Interfaces
{
    /// <summary>
    /// An IBigDDocument needs to have a Rev and an Id. It also needs to implement ICanJson
    /// which means it can read and write itself as JSON. Either you let your domain objects
    /// that you want to store in CouchDB implement this interface or if you are free to pick
    /// your own base class you can subclass from BigDDocument (or even BigDJsonDocument).
    /// </summary>
    public interface IBigDbDocument : ICanJson, IId
    {
        string Rev { get; set; }
    }
}