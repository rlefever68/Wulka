using System.ServiceModel;
using Wulka.Domain;

namespace Wulka.Interfaces
{
    [ServiceContract(Namespace = TransactionConst.ProcessorNamespace)]
    public interface ITransactionProcessor
    {
        [OperationContract(IsOneWay = true)]
        void SendTransactionMessage(TransactionItem transaction);
    }
}
