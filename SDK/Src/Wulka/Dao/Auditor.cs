using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Wulka.Logging;
using Wulka.Utils;

namespace Wulka.Dao
{
    /// <summary>
    /// Provides audit functions
    /// </summary>
    public class Auditor
    {

        /// <summary>
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <param name="affectedTable">The affected table.</param>
        /// <param name="application">The application.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="affectedFieldInfo">The affected field info.</param>
        public static void Audit(string connectionKey, string affectedTable, string application, string operation, string userName, string affectedFieldInfo)
        {
            Stopwatch timer = Stopwatch.StartNew();

            string connectionString = ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Broobu.Audit_Insert";
                cmd.Parameters.AddRange(new[]
                {
                    new SqlParameter("@AffectedDatabase", con.Database),
                    new SqlParameter("@AffectedTable", affectedTable),
                    new SqlParameter("@Application",application),
                    new SqlParameter("@Operation",operation),
                    new SqlParameter("@DateTime", DateTime.Now),
                    new SqlParameter("@User",userName),
                    new SqlParameter("@AffectedFieldInfo", affectedFieldInfo)
                });

                con.Open();
                cmd.ExecuteNonQuery();
            }

            timer.Stop();
            FxLog<Auditor>.DebugFormat("Audit record inserted ({0})", timer.ToStringInSeconds());
        }

    }
}
