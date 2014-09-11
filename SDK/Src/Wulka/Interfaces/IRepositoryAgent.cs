namespace Wulka.Interfaces
{
    public interface IRepositoryAgent
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        string TableName { get; }

        /// <summary>
        /// Gets the full name of the table.
        /// </summary>
        /// <value>The full name of the table.</value>
        string FullTableName { get; }
    }
}
