//using Wulka.Authentication.Agent;
using System;
using Wulka.Authentication;
using Wulka.Configuration;
using Wulka.Core;
using Wulka.Domain.Authentication;
using Wulka.Networking.Wcf.Interfaces;

namespace Wulka.Networking.Wcf
{
    [ContextServiceBehavior]
    public abstract class ContextServiceBase : ServiceBase, IWulkaContext
    {


        /// <summary>
        /// Contains the culture value that will be used to select the correct data culture.
        /// </summary>
        public string DataCulture
        {
            get
            {
                try
                {
                    return WulkaContext.Current != null ?
                        WulkaContext.Current[WulkaContextKey.DataCulture] :
                        WulkaContextKey.DataCulture;
                }
                catch (Exception)
                {
                    return WulkaContextDefault.DataCulture;
                }
            }
        }


        /// <summary>
        /// Gets the user culture.
        /// </summary>
        /// <value>The user culture.</value>
        public string UserCulture
        {
            get
            {
                try
                {
                    return WulkaContext.Current != null ?
                        WulkaContext.Current[WulkaContextKey.UserCulture] :
                        WulkaContextKey.UserCulture;

                }
                catch (Exception)
                {
                    return WulkaContextDefault.UserCulture;
                }
            }
        }




        /// <summary>
        /// Gets the session id.
        /// </summary>
        /// <value>The session id.</value>
        public string SessionId
        {
            get
            {
                try
                {
                    return WulkaContext.Current != null ?
                        WulkaContext.Current[WulkaContextKey.SessionId] :
                        WulkaContextKey.SessionId;
                }
                catch (Exception)
                {
                    return AuthenticationDefaults.SessionId;
                }
            }
        }




        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId
        {
            get
            {
                return WulkaContext.Current != null ?
                    WulkaContext.Current[WulkaContextKey.UserId] :
                    AuthenticationDefaults.Id;
            }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get 
            {
                return WulkaContext.Current != null ?
                    WulkaContext.Current[WulkaContextKey.UserName] :
                    AuthenticationDefaults.GuestUserName;
            }
        }



        /// <summary>
        /// Gets the user time zone.
        /// </summary>
        /// <value>The user time zone.</value>
        public string UserTimeZone
        {
            get
            {
                try
                {
                    return WulkaContext.Current != null ?
                    WulkaContext.Current[WulkaContextKey.UserTimeZone] :
                    WulkaContextKey.UserTimeZone;
                }
                catch (Exception)
                {
                    return WulkaContextDefault.UserTimeZone;
                }

            }
        }




        /// <summary>
        /// Gets the authentication mode.
        /// </summary>
        /// <value>The authentication mode.</value>
        public string AuthenticationMode
        {
            get
            {
                try
                {
                    return WulkaContext.Current != null ?
                        WulkaContext.Current[WulkaContextKey.AuthenticationMode] :
                        WulkaContextKey.AuthenticationMode;
                }
                catch (Exception)
                {
                    return WulkaContextDefault.AuthenticationMode;
                }
            }
        }


        /// <summary>
        /// Gets the data mode.
        /// </summary>
        /// <value>The data mode.</value>
        public string DataMode
        {
            get
            {
                try
                {
                    return


                        WulkaContext.Current != null ?
                        ConfigurationHelper.DataMode :
                        WulkaContext.Current[WulkaContextKey.DataMode];
                        

                }
                catch (Exception)
                {
                    return WulkaContextDefault.DataMode;

                }
            }
        }



        /// <summary>
        /// Gets the service code.
        /// </summary>
        /// <value>The service code.</value>
        public string ServiceCode
        {
            get
            {
                try
                {
                    return
                        WulkaContext.Current != null ?
                        WulkaContext.Current[WulkaContextKey.ServiceCode] :
                        WulkaContextDefault.ServiceCode;
                }
                catch (Exception)
                {
                    return WulkaContextDefault.ServiceCode;

                }
            }
        }


        /// <summary>
        /// Gets the service code.
        /// </summary>
        /// <value>The service code.</value>
        public string EcoSpace
        {
            get
            {
                try
                {
                    return
                        WulkaContext.Current != null ?
                        WulkaContext.Current[WulkaContextKey.EcoSpace] :
                        WulkaContextDefault.ServiceCode;
                }
                catch (Exception)
                {
                    return WulkaContextDefault.EcoSpace;

                }
            }
        }



        
      
    }
}
