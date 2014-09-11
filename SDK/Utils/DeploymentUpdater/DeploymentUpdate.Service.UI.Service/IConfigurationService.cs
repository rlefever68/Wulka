using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using DeploymentUpdate.DTO;

namespace DeploymentUpdate.Service.UI.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IConfigurationService
    {
        [OperationContract]
        List<MailAddressSurrogate> GetRecipients();

        [OperationContract]
        void UpdateRecipients(List<MailAddressSurrogate> recipients);        

        [OperationContract]
        String GetWatcherPath();

        [OperationContract]
        void UpdateWatcherPath(String watherPath);

        [OperationContract]
        String GetUriToWatchPath();

        [OperationContract]
        void UpdateUriToWatchPath(String uriToWathPath);
    } 
}
