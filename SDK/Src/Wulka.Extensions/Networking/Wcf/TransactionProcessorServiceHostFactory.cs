using System;
using System.Messaging;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using Wulka.Transactions;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class TransactionProcessorServiceHostFactory : ServiceHostFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private const string Contract = "Iris.Fx.Interfaces.ITransactionProcessor";

        /// <summary>
        /// Creates a <see cref="T:System.ServiceModel.ServiceHost"/> for a specified type of service with a specific base address.
        /// </summary>
        /// <param name="serviceType">Specifies the type of service to host.</param>
        /// <param name="baseAddresses">The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.</param>
        /// <returns>A <see cref="T:System.ServiceModel.ServiceHost"/> for the type of service specified with a specific base address.</returns>
        /// <remarks></remarks>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            string queueName = serviceType.Name.ToLower().Replace("service", "");

            string queueAddress = "net.msmq://"+TransactionConfigurationHelper.QueueHost+"/private/" + queueName;
            string queuePath = @".\private$\" + queueName;

            CheckForQueue(queuePath);

            var host = new ServiceHost(serviceType, baseAddresses);

            var binding = new NetMsmqBinding(NetMsmqSecurityMode.None);
            binding.Security.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.None;
            binding.Security.Transport.MsmqProtectionLevel = ProtectionLevel.None;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;

            host.AddServiceEndpoint(Contract, binding, queueAddress);
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            return host;
        }

        /// <summary>
        /// Checks for queue.
        /// </summary>
        /// <param name="queuePath">The queue path.</param>
        /// <remarks></remarks>
        private void CheckForQueue(string queuePath)
        {
            // Create the transacted MSMQ queue if necessary.
            if (!MessageQueue.Exists(queuePath))
            {
                var queue = MessageQueue.Create(queuePath, true);
                queue.Authenticate = false;
                queue.SetPermissions("EveryOne", MessageQueueAccessRights.FullControl);
            }
        }
    }
}
