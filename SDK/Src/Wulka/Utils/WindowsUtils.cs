using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Wulka.Utils
{
    public class WindowsUtils
    {
        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(string userName, string domainName, string password, int LogonType, int LogonProvider, ref IntPtr phToken);

        public static WindowsIdentity CurrentUser = WindowsIdentity.GetCurrent();
        public static string WindowsUserName = CurrentUser.Name;

        public static bool ValidateCredentials(string userName, string password, string domain)
        {
            IntPtr tokenHandler = IntPtr.Zero;
            return LogonUser(userName, domain, password, 2, 0, ref tokenHandler);
        }
    }
}
