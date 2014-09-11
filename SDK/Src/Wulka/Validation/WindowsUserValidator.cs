using System;

namespace Wulka.Validation
{
    public class WindowsUserValidator
    {
        [System.Runtime.InteropServices.DllImport("advapi32.dll")]
        public static extern bool LogonUser(string userName, string domainName,
            string password, int LogonType, int LogonProvider, ref IntPtr phToken);

        /// <summary>
        /// Determines whether [is validate credentials] [the specified user name].
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="domain">The domain.</param>
        /// <returns>
        /// 	<c>true</c> if [is validate credentials] [the specified user name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidateCredentials(string userName, string password, string domain)
        {
            IntPtr tokenHandler = IntPtr.Zero;
            return LogonUser(userName, domain, password, 2, 0, ref tokenHandler);
        }
    }

}
