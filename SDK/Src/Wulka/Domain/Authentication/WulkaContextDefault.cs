using System;
using Wulka.Authentication;

namespace Wulka.Domain.Authentication
{

    public class WulkaContextDefault
    {
        public const string DataCulture = "en-us";
        public const string UserCulture = "en-us";
        public static readonly string NullGuid = Guid.Empty.ToString();
        public static string DataMode = "Instance";
        public static string UserTimeZone = "GMT";
        public static string UserId = AuthenticationDefaults.GuestUserName;
        public static string SessionId = AuthenticationDefaults.SessionId;
        public static string AuthenticationMode = AuthenticationDefaults.AuthModeId;
        public const string ServiceCode = "FOO_APP";
        public static string EcoSpace = "MASTER_ECOSPACE";
    }
}
