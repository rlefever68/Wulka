using System.ServiceModel;

namespace Wulka.Interfaces
{
    [ServiceContract(Namespace = AppContractServiceConst.Namespace)]
    public interface IAppContractSentry
    {
        [OperationContract]
        string RegisterAppUsage(string item);


    }
}
