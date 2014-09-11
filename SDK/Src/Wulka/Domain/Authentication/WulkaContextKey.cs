namespace Wulka.Domain.Authentication
{

    public class WulkaContextKey
    {
        public const string PasswordEnc = "PasswordEnc";

     
        public const string UserCulture = "UserCulture";
        public const string DataCulture = "DataCulture";
        /// <summary>
        /// The Current session Id
        /// </summary>
        public const string SessionId = "SessionId";
        /// <summary>
        /// The Current User Id
        /// </summary>
        public const string UserId = "UserId";
        /// <summary>
        /// The Current User's Time zone.
        /// </summary>
        public const string UserTimeZone = "UserTimeZone";
        /// <summary>
        /// The Current Account Name
        /// </summary>
        public const string UserName = "UserName";
        /// <summary>
        /// The AuthenticationMode as selected by the user.
        /// </summary>
        public const string AuthenticationMode = "AuthenticationMode";

        /// <summary>
        /// The Datamode operation
        /// </summary>
        public const string DataMode = "DataMode";

        /// <summary>
        /// Code of the Calling Service or Application
        /// </summary>
        public const string ServiceCode = "ServiceCode";

        public const string EcoSpace = "EcoSpace";
    }
}
