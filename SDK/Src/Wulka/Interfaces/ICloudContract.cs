using System.ServiceModel;
using Wulka.Domain;
using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    [ServiceContract(Namespace = CloudContractServiceConst.Namespace)]
    [ServiceKnownType(typeof(Result))]
    [ServiceKnownType(typeof(CloudContract))]
    public interface ICloudContract
    {
        [OperationContract]
        CloudContract GetCloudContract(string contractId);
        [OperationContract]
        CloudContract[] SaveCloudContracts(CloudContract[] contract);
        [OperationContract]
        CloudContract SaveCloudContract(CloudContract contract);
    }
}
