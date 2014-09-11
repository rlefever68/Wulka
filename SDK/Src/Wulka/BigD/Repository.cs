using System;
using Wulka.BigD.Configuration;
using Wulka.BigD.Contract.Interfaces;
using Wulka.Domain.Interfaces;

namespace Wulka.BigD
{
    class Repository<T> where T : IDomainObject, new()
    {

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public static IBigDDatabase Db
        {
            get
            {
                return GetDb();
            }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <returns>ICouchDatabase.</returns>
        protected static IBigDDatabase GetDb()
        {
            var srv = new BigDServer(ConfigurationHelper.CouchDbHost,
                ConfigurationHelper.CouchDbPort,
                ConfigurationHelper.CouchDbUser,
                ConfigurationHelper.CouchDbPassword);
            if (srv == null)
            {
                throw new Exception("Could not obtain a valid CouchDB instance.");
            }
            return srv.GetDatabase(GetDbName());
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <returns>System.String.</returns>
        protected  static string GetDbName()
        {
            var s = new T();
            return s.DocType
                .ToLower()
                .Replace('.', '-');
        }
    }
}
