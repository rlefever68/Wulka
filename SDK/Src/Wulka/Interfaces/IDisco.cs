using System.ServiceModel;
using Wulka.Domain;
using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    [ServiceContract(Namespace = DiscoServiceConst.Namespace)]
    [ServiceKnownType(typeof (Result))]
    [ServiceKnownType(typeof (DiscoItem))]
    public interface IDisco
    {
        [OperationContract]
        SerializableEndpoint[] GetEndpoints(string contractType);

        [OperationContract]
        SerializableEndpoint[] GetAllEndpoints();

        [OperationContract]
        DiscoItem[] GetAllEndpointAddresses();

    }
}

