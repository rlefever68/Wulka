using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    /// <summary>
    /// Provides CRUD functions for the given table
    /// </summary>
    /// <typeparam name="TTable">The type of the table.</typeparam>
    /// <typeparam name="TId">The type of the identifier (primary key).</typeparam>
    /// <typeparam name="TResult">The type of the result including PK.</typeparam>
    public interface IRepositoryCrudAgent<TTable, TId, TResult> : IRepositoryAgent where TTable : class where TResult : Result
    {

        #region SelectAll

        /// <summary>
        /// Selects and returns all records for given table.
        /// </summary>
        /// <returns></returns>
        TTable[] SelectAll();

   

        #endregion

        #region SelectById

        /// <summary>
        /// Selects and returns the record with given identifier.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        TTable SelectById(TId id);

     

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified value in the given table.
        /// </summary>
        /// <param name="value">The value.</param>
        Result Update(TTable value);

   

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the record with given identifier from the given table.
        /// </summary>
        /// <param name="id">The id.</param>
        Result Delete(TId id);


        #endregion

        #region Insert

        /// <summary>
        /// Inserts the specified value in the given table.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        TResult Insert(TTable value);

   

        #endregion

    }
}