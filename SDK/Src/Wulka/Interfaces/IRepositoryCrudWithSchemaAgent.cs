using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    /// <summary>
    /// Provides CRUD functions for the given table
    /// </summary>
    /// <typeparam name="TTable">The type of the table.</typeparam>
    /// <typeparam name="TId">The type of the identifier (primary key).</typeparam>
    /// <typeparam name="TResult">The type of the result including PK.</typeparam>
    public interface IRepositoryCrudWithSchemaAgent<TTable, TId, TResult> : IRepositoryAgent where TTable : class where TResult : Result
    {

        #region SelectAll

        /// <summary>
        /// Selects and returns all records for given table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        TTable[] SelectAll(string schema);

        #endregion

        #region SelectById

        /// <summary>
        /// Selects and returns the record with given identifier.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        TTable SelectById(string schema, TId id);

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified value in the given table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="value">The value.</param>
        Result Update(string schema, TTable value);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the record with given identifier from the given table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="id">The id.</param>
        Result Delete(string schema, TId id);

        #endregion

        #region Insert

        /// <summary>
        /// Inserts the specified value in the given table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        TResult Insert(string schema, TTable value);

        #endregion

    }
}