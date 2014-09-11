using System;
using Wulka.Domain.Authentication;

namespace Wulka.Authentication
{
    public class AuthenticationDefaults
    {
        public const string Email = "no.no@no.no";
        public static readonly string Id = "ACCOUNT_GUEST";
        public static byte[] CardId = { };
        public static DateTime? StartDate = DateTime.UtcNow;
        public static DateTime? EndDate = DateTime.MaxValue;
        public static string FirstName = "Guest";
        public static string LastName = "User";
        public static string MiddleName = String.Empty;
        public static string SessionId = "_GUEST_SESSION_";
        public static string Telephone1 = String.Empty;
        public static string Telephone2 = String.Empty;
        public static byte IsActive = 0;
        public const string AuthModeId = AuthenticationMode.Native;
        public const string GuestUserName = "guest@broobu.com";
        public const string GuestPwd = "GUEST_PASSWORD";
        public const string GuestEncPwd = "2eYwGBOgrAoQtfLaUyb3qQ==";
        public const string ServiceCode = "BROOBU";
        public const string AdminId = "root@broobu.com";
        public const string RootUserName = "root@broobu.com";
        public const string RootEncPwd = "EcgFw6T0yT3tGUQxRUHHmA==";
        public static string RootPwd = "I5H@llP@55";
    }
}
