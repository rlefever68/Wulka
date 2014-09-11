using System;
using System.ComponentModel;
using Wulka.Core;
using Wulka.Interfaces;

namespace Wulka.Networking.Wcf
{
    public class ValidateSessionAgent : DiscoProxy<IValidateSession>,IValidateSessionAgent
    {
   

        #region IValidateSession Members

        public ValidateSessionAgent(string discoUrl) 
            : base(discoUrl)
        {
        }

        public bool  Validate(string userName, string sessionId)
        {
            var clt = Client;
            WulkaContext c = WulkaContext.Current;
            var b= clt.Validate(userName, sessionId);
            return b;
        }
        #endregion

      

        #region IValidateSessionAgent Members


        public event Action<bool> ValidateCompleted;


        /// <summary>
        /// Validates the async.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionId">The session id.</param>
        public void ValidateAsync(string userName, string sessionId)
        {
            bool b = false;
            using (var wrk = new BackgroundWorker())
            {
                wrk.DoWork += (s, e) =>
                {
                    b = Validate(userName, sessionId);
                };
                wrk.RunWorkerCompleted += (s, e) =>
                {
                    if (ValidateCompleted != null)
                        ValidateCompleted(b);
                };
                wrk.RunWorkerAsync();
            }
        }

        #endregion
    }
}
