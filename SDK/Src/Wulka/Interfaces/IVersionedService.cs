using System.ServiceModel;

namespace Wulka.Interfaces
{
    [ServiceContract]
    public interface IVersionedService
    {
        [OperationContract]
        string GetVersion();
    }
}
