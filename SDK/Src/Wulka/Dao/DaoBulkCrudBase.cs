using System.Transactions;
using Wulka.Domain.Base;
using Wulka.Interfaces;

namespace Wulka.Dao
{
    public abstract class DaoBulkCrudBase<TTable, TId, TResult> 
        : DaoCrudBase<TTable, TId, TResult>, IDaoBulkCrud<TTable, TId, TResult> where TTable : class where TResult : Result
    {
        protected DaoBulkCrudBase(string connectionKey)
            : base(connectionKey)
        {
        }

        #region Implementation of IDaoBulkCrudWithSchema<TTable,TId>

        /// <summary>
        /// Inserts the specified values in the target table.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        /// <returns></returns>
        public TResult[] BulkInsert(TTable[] values, string username, string applicationCode)
        {
            TResult[] ids;
            using (TransactionScope scope = new TransactionScope())
            {
                ids = DoBulkInsert(values);

                string auditValue = string.Format("{0} records inserted", values.Length);

                Auditor.Audit(ConnectionKey, typeof(TTable).Name, applicationCode, "BULK_INSERT", username, auditValue);

                scope.Complete();
            }
            return ids;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Does the insert of given values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        protected abstract TResult[] DoBulkInsert(TTable[] values);
        
        #endregion

    }
}
