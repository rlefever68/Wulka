using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wulka.Versioning
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DatabaseVersionAttribute : Attribute
    {
        private readonly string _databaseVersion;
        private readonly int _requiredSequencenumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseVersionAttribute"/> class.
        /// </summary>
        /// <param name="databaseVersion">The database version.</param>
        /// <param name="requiredSequencenumber">The sequencenumber of the patch that must be applied.</param>
        public DatabaseVersionAttribute(string databaseVersion, int requiredSequencenumber)
        {
            _databaseVersion = databaseVersion;
            _requiredSequencenumber = requiredSequencenumber;
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <value>The database version.</value>
        public string DatabaseVersion { get { return _databaseVersion; } }

        /// <summary>
        /// Gets the sequence number that is required for the DatabaseVersion.
        /// </summary>
        /// <value>The sequence number.</value>
        public int RequiredSequenceNumber { get { return _requiredSequencenumber; } }

        /// <summary>
        /// Gets or sets the current sequence number of the DatabaseVersion.
        /// </summary>
        /// <remarks>
        /// This field will be filled in when calling CheckDatabaseVersion on or with this instance
        /// </remarks>
        /// <value>The current sequence number.</value>
        public int CurrentSequenceNumber { get; set; }

        /// <summary>
        /// Gets the database versions from the Versioning Assembly and
        /// the assembly that is calling this method.
        /// </summary>
        /// <returns>Array of DatabaseVersion attributes</returns>
        public static DatabaseVersionAttribute[] GetDatabaseVersions()
        {
            var versions = new List<DatabaseVersionAttribute>();

            var executingAssembly = Assembly.GetExecutingAssembly();
            versions.AddRange(GetDatabaseVersions(executingAssembly));

            var callingAssembly = Assembly.GetCallingAssembly();
            if(callingAssembly != executingAssembly)
            {
                versions.AddRange(GetDatabaseVersions(callingAssembly));
            }

            return versions.ToArray();
        }

        /// <summary>
        /// Gets the database versions from the given assembly
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Array of DatabaseVersion attributes</returns>
        public static DatabaseVersionAttribute[] GetDatabaseVersions(Assembly assembly)
        {
            var versions = assembly.GetCustomAttributes(false).OfType<DatabaseVersionAttribute>();
            return versions.ToArray();
        }
    }
}
