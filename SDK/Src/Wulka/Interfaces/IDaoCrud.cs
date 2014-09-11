using System.ServiceModel;
using Wulka.Domain;
using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    /// <summary>
    /// Provides access to the CRUD functions of a DAO for the
    /// specified table type and identifier type
    /// </summary>
    /// <typeparam name="TTable">The type of the table.</typeparam>
    /// <typeparam name="TId">The type of the key.</typeparam>
    /// <typeparam name="TResult">The Result type returned with primary key.</typeparam>
    [ServiceContract(Namespace = ServiceConst.Namespace)]
    public interface IDaoCrud<TTable, TId, TResult> where TTable : class where TResult : Result
    {
        /// <summary>
        /// Selects and returns all records contained in the table.
        /// </summary>
        /// <returns>All records contained in the table</returns>
        [OperationContract]
        TTable[] SelectAll();

        /// <summary>
        /// Selects and returns a single record specified by the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the record to return.</param>
        /// <returns>The selected record</returns>
        [OperationContract]
        TTable SelectById(TId id);

        /// <summary>
        /// Updates a record in the target table from the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Result Update(TTable value, string username, string applicationCode);

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Result Delete(TId id, string username, string applicationCode);

        /// <summary>
        /// Inserts the specified value in the target table.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        TResult Insert(TTable value, string username, string applicationCode);
    }
}
