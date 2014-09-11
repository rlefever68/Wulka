using System.Transactions;
using Wulka.Domain.Base;
using Wulka.Interfaces;

namespace Wulka.Dao
{
    public abstract class DaoBulkCrudWithSchemaBase<TTable, TId, TResult> 
        : DaoCrudWithSchemaBase<TTable, TId, TResult>, IDaoBulkCrudWithSchema<TTable, TId, TResult> where TTable : class where TResult : Result
    {
        protected DaoBulkCrudWithSchemaBase(string connectionKey)
            : base(connectionKey)
        {
        }

        #region Implementation of IDaoBulkCrudWithSchema<TTable,TId>

        /// <summary>
        /// Inserts the specified values in the target table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="values">The values.</param>
        /// <param name="username">The username.</param>
        /// <param name="applicationCode">The application code.</param>
        /// <returns></returns>
        public TResult[] BulkInsert(string schema, TTable[] values, string username, string applicationCode)
        {
            TResult[] idResults;
            using (TransactionScope scope = new TransactionScope())
            {
                idResults = DoBulkInsert(schema, values);

                string auditValue = string.Format("{0} records inserted", values.Length);

                Auditor.Audit(ConnectionKey, schema + "." + typeof(TTable).Name, applicationCode, "BULK_INSERT", username, auditValue);

                scope.Complete();
            }
            return idResults;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Does the insert of given values.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        protected abstract TResult[] DoBulkInsert(string schema, TTable[] values);
        
        #endregion

}
}
