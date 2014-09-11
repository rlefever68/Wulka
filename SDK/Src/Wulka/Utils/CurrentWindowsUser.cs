using System.Globalization;
using System.Security.Principal;

namespace Wulka.Utils
{
    public class CurrentWindowsUser
    {
        public static CultureInfo Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        public static string UserName = WindowsIdentity.GetCurrent().Name;
        public static bool IsAnonymous = WindowsIdentity.GetCurrent().IsAnonymous;
        public static bool IsAuthenticated = WindowsIdentity.GetCurrent().IsAuthenticated;
        public static bool IsGuest = WindowsIdentity.GetCurrent().IsGuest;
        public static bool IsSystem = WindowsIdentity.GetCurrent().IsSystem;
        public static SecurityIdentifier SID = WindowsIdentity.GetCurrent().User;
        public static CultureInfo[] AllCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
    }
}
