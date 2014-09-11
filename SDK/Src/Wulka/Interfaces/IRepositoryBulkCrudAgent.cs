using System.ServiceModel;
using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    public interface IRepositoryBulkCrudAgent<TTable, TId, TResult> : IRepositoryCrudAgent<TTable, TId, TResult> 
        where TTable : class 
        where TResult : Result
    {

        /// <summary>
        /// Inserts the specified values in the target table.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        TResult[] BulkInsert(TTable[] values);
    }
}
