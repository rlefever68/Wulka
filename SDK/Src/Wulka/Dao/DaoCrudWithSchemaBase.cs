using System.Transactions;
using Wulka.Domain.Base;
using Wulka.Interfaces;

namespace Wulka.Dao
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TTable">The type of the table.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class DaoCrudWithSchemaBase<TTable, TId, TResult> : DaoBase, IDaoCrudWithSchema<TTable, TId, TResult> where TTable : class where TResult : Result
    {
        protected DaoCrudWithSchemaBase(string connectionKey)
            : base(connectionKey)
        {
        }

        #region Implementation of IDaoCrud<TTable,TId>

        /// <summary>
        /// Selects and returns all records contained in the table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns>All records contained in the table</returns>
        public abstract TTable[] SelectAll(string schema);

        /// <summary>
        /// Selects and returns a single record specified by the given identifier.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="id">The identifier of the record to return.</param>
        /// <returns>The selected record</returns>
        public abstract TTable SelectById(string schema, TId id);

        /// <summary>
        /// Updates a record in the target table from the specified value.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="value">The value.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        public Result Update(string schema, TTable value, string username, string applicationCode)
        {
            Result result;
            using (TransactionScope scope = new TransactionScope())
            {
                result = DoUpdate(schema, value);

                Auditor.Audit(ConnectionKey, schema + "." + typeof(TTable).Name, applicationCode, "UPDATE", username, Serialize(value));

                scope.Complete();
            }
            return result;
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="id">The id.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        public Result Delete(string schema, TId id, string username, string applicationCode)
        {
            Result result;
            using (TransactionScope scope = new TransactionScope())
            {
                result = DoDelete(schema, id);

                Auditor.Audit(ConnectionKey, schema + "." + typeof(TTable).Name, applicationCode, "DELETE", username, id.ToString());

                scope.Complete();
            }
            return result;
        }

        /// <summary>
        /// Inserts the specified value in the target table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="value">The value.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        /// <returns></returns>
        public TResult Insert(string schema, TTable value, string username, string applicationCode)
        {
            TResult idResult;
            using (TransactionScope scope = new TransactionScope())
            {
                idResult = DoInsert(schema, value);

                Auditor.Audit(ConnectionKey, schema + "." + typeof(TTable).Name, applicationCode, "INSERT", username, Serialize(value));

                scope.Complete();
            }
            return idResult;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Does the insert.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="value">The value to be inserted.</param>
        /// <returns></returns>
        protected abstract TResult DoInsert(string schema, TTable value);

        /// <summary>
        /// Does the delete.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="id">The identifier of the record to be deleted.</param>
        protected abstract Result DoDelete(string schema, TId id);

        /// <summary>
        /// Does the update.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="value">The value to update the record.</param>
        protected abstract Result DoUpdate(string schema, TTable value);
        
        #endregion

    }
}
