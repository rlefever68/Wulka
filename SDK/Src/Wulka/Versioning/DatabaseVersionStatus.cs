namespace Wulka.Versioning
{
    /// <summary>
    /// List the possible status a database can be in.
    /// </summary>
    public enum DatabaseVersionStatus
    {
        /// <summary>
        /// The database is up-to-date and ready to be used
        /// </summary>
        Ok,
        /// <summary>
        /// The DatabaseVersion or SequenceNumber that is passed along was invalid.
        /// </summary>
        InvalidParameters,
        /// <summary>
        /// The requested patch has not been applied
        /// </summary>
        PatchNotApplied,
        /// <summary>
        /// The requested patch was applied but failed to complete
        /// </summary>
        PatchFailed,
        /// <summary>
        /// The database has newer patches applied that are incompatible with the requested patch.
        /// </summary>
        Incompatible,
        /// <summary>
        /// A database error occurred while trying to get the version.
        /// </summary>
        Internal
    }
}