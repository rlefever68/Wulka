using System.ServiceModel;
using Wulka.Domain;

namespace Wulka.Interfaces
{
    [ServiceContract(Namespace = TransactionConst.RouterNamespace)]
    public interface ITransactionRouter
    {
        [OperationContract(IsOneWay = true)]
        void SubmitTransactionFile(TransactionFileItem transactionFile);
    }
}
