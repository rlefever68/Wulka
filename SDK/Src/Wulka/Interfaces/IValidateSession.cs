using System.ServiceModel;
using Wulka.Domain;

namespace Wulka.Interfaces
{
    [ServiceContract(Namespace=ServiceConst.Namespace)]
    public interface IValidateSession
    {
        [OperationContract]
        bool Validate(string userName, string sessionId);
    }
}
