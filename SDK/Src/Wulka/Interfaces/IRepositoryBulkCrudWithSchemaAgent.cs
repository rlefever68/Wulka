using System.ServiceModel;
using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    public interface IRepositoryBulkCrudWithSchemaAgent<TTable, TId, TResult> : IRepositoryCrudWithSchemaAgent<TTable, TId, TResult> where TTable : class where TResult : Result
    {

        /// <summary>
        /// Inserts the specified values in the target table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        TResult[] BulkInsert(string schema, TTable[] values);
    }
}
