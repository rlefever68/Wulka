using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Wulka.Utils;

namespace Wulka.Dao.Utilities
{
    /// <summary>
    /// Provides SqlCommand extensions.
    /// </summary>
    public static class SqlCommandExtensions
    {
        /// <summary>
        /// Gets a printable command string from the given SQL command.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns></returns>
        public static string GetCommandString(this SqlCommand sqlCommand)
        {
            return GetCommandString(sqlCommand, null, null);
        }

        /// <summary>
        /// Gets a printable command string from the given SQL command.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="sqlTimer">The SQL timer.</param>
        /// <returns></returns>
        public static string GetCommandString(this SqlCommand sqlCommand, Stopwatch sqlTimer)
        {
            return GetCommandString(sqlCommand, sqlTimer, null);
        }

        /// <summary>
        /// Gets a printable command string from the given SQL command.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="sqlTimer">The SQL timer.</param>
        /// <param name="recordCount">The record count.</param>
        /// <returns></returns>
        public static string GetCommandString(this SqlCommand sqlCommand, Stopwatch sqlTimer, int? recordCount)
        {
            string commandString;

            // Command input
            if (sqlTimer == null)
            {
                commandString = "--> " + sqlCommand.CommandText;
                bool firstParameter = true;
                foreach (SqlParameter sqlParameter in sqlCommand.Parameters)
                {
                    switch (sqlParameter.Direction)
                    {
                        case ParameterDirection.Input:
                        case ParameterDirection.InputOutput:
                            break;
                        //case ParameterDirection.Output:
                        //case ParameterDirection.ReturnValue:
                        default:
                            continue; // Do not list this parameter in the log
                    }
                    if (firstParameter == false)
                    {
                        commandString += ",";
                    }
                    commandString
                        += " "
                        + sqlParameter.ParameterName
                        + "="
                        + sqlParameter.Value;
                    firstParameter = false;
                }
            }
            else
            // Command output
            {
                commandString = "<-- " + sqlCommand.CommandText;
                bool firstParameter = true;
                foreach (SqlParameter sqlParameter in sqlCommand.Parameters)
                {
                    switch (sqlParameter.Direction)
                    {
                        case ParameterDirection.InputOutput:
                        case ParameterDirection.Output:
                        case ParameterDirection.ReturnValue:
                            break;
                        //case ParameterDirection.Input:
                        default:
                            continue; // Do not list this parameter in the log
                    }
                    if (firstParameter == false)
                    {
                        commandString += ",";
                    }
                    commandString
                        += " "
                        + sqlParameter.ParameterName
                        + "="
                        + sqlParameter.Value;
                    firstParameter = false;
                }
                if (recordCount != null)
                {
                    commandString += " (" + recordCount + ((recordCount == 1) ? " record" : " records") + " returned)";
                }
                commandString += " (" + sqlTimer.ToStringInSeconds() + ")";
            }
            return commandString;
        }
    }
}
