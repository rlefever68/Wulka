using System.ServiceModel;
using Wulka.Domain;
using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    /// <summary>
    /// Provides access to the CRUD functions (including bulk functions) of a DAO for the
    /// specified table type and identifier type
    /// </summary>
    /// <typeparam name="TTable">The type of the table.</typeparam>
    /// <typeparam name="TId">The type of the key.</typeparam>
    /// <typeparam name="TResult">The type of the result including PK.</typeparam>
    [ServiceContract(Namespace = ServiceConst.Namespace)]
    public interface IDaoBulkCrudWithSchema<TTable, TId, TResult> : IDaoCrudWithSchema<TTable, TId, TResult> where TTable : class where TResult : Result
    {

        /// <summary>
        /// Inserts the specified values in the target table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="values">The values.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        TResult[] BulkInsert(string schema, TTable[] values, string username, string applicationCode);
    }
}
