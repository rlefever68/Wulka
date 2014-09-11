using System;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using Wulka.Domain.Authentication;
using Wulka.Networking.Wcf.Interfaces;

namespace Wulka.Networking.Wcf
{
    public class UserNameSessionMessageInspector : IUserNameMessageInspector
    {
      
        public string Session { get; set; }
      

        
        public string UserName
        {
            get;
            set;
        }

        public CredentialsTypeEnum Type
        {
            get { return CredentialsTypeEnum.UserNameSession; }
        }

     


        public UserNameSessionMessageInspector(string user, string session)
        {
            UserName = user;
            Session = session;
        }

        #region IClientMessageInspector Members

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {}

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The  client object channel.</param>
        /// <returns>
        /// The object that is returned as the <paramref name="correlationState "/>argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)"/> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid"/> to ensure that no two <paramref name="correlationState"/> objects are the same.
        /// </returns>
        public object BeforeSendRequest(ref Message request, System.ServiceModel.IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequest;
            if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                httpRequest = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            else
            {
                httpRequest = new HttpRequestMessageProperty();
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);
            }

            string hash = Convert.ToBase64String(new HMACSHA256(Encoding.UTF8.GetBytes(UserName)).ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}{1}", UserName, Session))));

            httpRequest.Headers["Authorization"] = string.Format("Wulkaas {0}:{1}", Session, hash);
            return null;
        }

        #endregion
    }
}
