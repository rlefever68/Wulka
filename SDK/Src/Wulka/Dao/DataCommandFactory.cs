using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Wulka.Dao
{
    public class DataCommandFactory
    {
        public static DbCommand CreateCommand(string spName, string connectionKey, params SqlParameter [] parameters)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString);
            SqlCommand cmd = new SqlCommand
                                 {
                                     Connection = con,
                                     CommandType = CommandType.StoredProcedure,
                                     CommandText = spName
                                 };
            cmd.Parameters.AddRange(parameters);

            con.Open();
            return cmd;
        }
    }
}
