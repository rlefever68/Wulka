// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-11-2013
//
// Last Modified By : ON8RL
// Last Modified On : 08-05-2014
// ***********************************************************************
// <copyright file="ServiceHostHelper.cs" company="Broobu">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using NLog;
using Wulka.Agent;
using Wulka.Configuration;
using Wulka.Domain;
using Wulka.Exceptions;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class ServiceHostHelper.
    /// </summary>
    public static class SentryHostHelper
    {
        /// <summary>
        /// Prints the host information.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void PrintHostInfo(this ServiceHostBase host)
        {
            var T = host.GetType();
            var logger = LogManager.GetLogger(T.Name);
            var serviceAssembly = Assembly.GetExecutingAssembly();
            logger.Debug("");
            logger.Debug("************************************************************************************************");
            logger.Debug("\t{0}", ConfigurationHelper.ServiceCommonName);
            logger.Debug("\tVersion {0}", serviceAssembly.GetName().Version);
            logger.Debug("");
            logger.Debug("\tAnnouncing to cloud  @ {0}", ConfigurationHelper.CloudAnnounce);
            logger.Debug("\tProbing the cloud    @ {0}", ConfigurationHelper.DiscoEndpoint);
            logger.Debug("");
            logger.Debug("\tService Help Page       : {0}", ConfigurationHelper.ServiceHelpPage);
            logger.Debug("\tRegister Domain Objects : {0}", ConfigurationHelper.HasRegisterDomainObjects);
            logger.Debug("\tMEX Enabled             : {0}", ConfigurationHelper.EnableMex);
            logger.Debug("\tSecure MEX Enabled      : {0}", ConfigurationHelper.SecureMex);
            logger.Debug("\tSession Validation      : {0}", ConfigurationHelper.IsSessionValidationActive);
            logger.Debug("\tAnnouncement Delay      : {0}", ConfigurationHelper.AnnounceDelay);
            logger.Debug("\tRegister Metadata       : {0}", ConfigurationHelper.RegisterServiceMetadata);
            logger.Debug("\tRestful                 : {0}", ConfigurationHelper.IsRestful);
            if (BigD.Configuration.ConfigurationHelper.CouchDbDatabase != null)
            {
                logger.Debug("");
                logger.Debug("\tDatabase Info:");
                logger.Debug("\t\tProtocol : {0}", BigD.Configuration.ConfigurationHelper.CouchDbProtocol);
                logger.Debug("\t\tHost     : {0}", BigD.Configuration.ConfigurationHelper.CouchDbHost);
                logger.Debug("\t\tPort     : {0}", BigD.Configuration.ConfigurationHelper.CouchDbPort);
                logger.Debug("\t\tDatabase : {0}", BigD.Configuration.ConfigurationHelper.CouchDbDatabase);
                logger.Debug("\t\tUser     : {0}", BigD.Configuration.ConfigurationHelper.CouchDbUser);
            }
            logger.Debug("");
            logger.Debug("************************************************************************************************");
        }


        /// <summary>
        /// Prints the listening endpoints.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void PrintListeningEndpoints(this ServiceHostBase host)
        {
            var T = host.GetType();
            var logger = LogManager.GetLogger(T.Name);
            var serviceAssembly = Assembly.GetExecutingAssembly().GetName();
            var publisherName = serviceAssembly.Name;

            // Iterate through the endpoints contained in the ServiceDescription 
            logger.Debug("Active Service Endpoints:");
            var lst = new List<CloudContract>();
            foreach (var se in host.Description.Endpoints)
            {
                logger.Debug("Endpoint:\n");
                logger.Debug("\tAddress:\t{0}", se.Address);
                logger.Debug("\tBinding:\t{0}", se.Binding);
                logger.Debug("\tContract:\t{0}", se.Contract.Name);
                foreach (var behavior in se.Behaviors)
                {
                    logger.Debug("Behavior: {0}\n", behavior);
                }
                var newId = String.Format("{0}:{1}", se.Address, se.Contract.Name);
                lst.Add(new CloudContract()
                {

                    Id = newId,
                    ContractName = String.Format("{0}", se.Contract.Name),
                    Binding = String.Format("{0}", se.Binding),
                    Address = String.Format("{0}", se.Address),
                    Publisher = publisherName,
                    RegistrationTimeStamp = DateTime.UtcNow,
                    CommonName = ConfigurationHelper.ServiceCommonName,
                    AdditionalInfoUri = ConfigurationHelper.ServiceHelpPage
                });
            }
            host.RegisterServiceMetadatas(lst);
            logger.Debug("************************************************************************************************");
            logger.Debug("  {0} Listening...", ConfigurationHelper.ServiceCommonName);
            logger.Debug("************************************************************************************************");
        }


        /// <summary>
        /// The logger
        /// </summary>
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Registers the service metadatas.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="lst">The LST.</param>
        private static void RegisterServiceMetadatas(this ServiceHostBase host, IEnumerable<CloudContract> lst)
        {
            var T = host.GetType();
            var logger = LogManager.GetLogger(T.Name);
            if (!ConfigurationHelper.RegisterServiceMetadata) return;
            logger.Debug("Registering {0} Metadata...", ConfigurationHelper.ServiceCommonName);
            foreach (var data in lst)
            {
                try
                {
                    var res = DiscoPortal
                        .CloudContracts
                        .SaveCloudContract(data);
                    logger.Debug("=> Metadata registration succeeded for [{0}].", res.Id);
                }
                catch (Exception ex)
                {
                    logger.Debug("Error Registering {0}.", data.Id);
                    logger.Debug(ex.GetCombinedMessages());
                }
            }
        }

        /// <summary>
        /// Prints the termination.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void PrintTermination(this ServiceHostBase host)
        {
            var T = host.GetType();
            var logger = LogManager.GetLogger(T.Name);
            logger.Debug("");
            logger.Debug("************************************************************************************************");
            logger.Debug("  {0} Terminated.", ConfigurationHelper.ServiceCommonName);
            logger.Debug("************************************************************************************************");
            logger.Debug("");

        }


        /// <summary>
        /// Gets the service contracts.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>Type[][].</returns>
        public static Type[] GetServiceContracts(Type serviceType)
        {
            Type[] interfaces = serviceType.GetInterfaces();
            return interfaces.Where(ifx => ifx.IsDefined(typeof(ServiceContractAttribute), true)).ToArray();
        }


        /// <summary>
        /// Adds the endpoints.
        /// </summary>
        /// <param name="serviceHost">The service host.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="bindingFactoryKey">The binding factory key.</param>
        public static void AddEndpoints(this ServiceHostBase serviceHost, Type serviceType, string bindingFactoryKey)
        {
            var contracts = GetServiceContracts(serviceType);
            var bnd = BindingFactory.CreateBindingFromKey(bindingFactoryKey);
            foreach (var s in contracts.Select(contract => contract.ToString()))
            {
                serviceHost.AddServiceEndpoint(s, bnd, "");
            }
        }




        /// <summary>
        /// Makes the announcing service.
        /// </summary>
        /// <param name="serviceHost">The service host.</param>
        public static void MakeAnnouncingService(this ServiceHostBase serviceHost)
        {
            if (ConfigurationHelper.IsRestful)
                serviceHost.AddRestEndpoints();
            if(ConfigurationHelper.AddDefaultEndpoints)
                serviceHost.AddDefaultEndpoints();
            if (ConfigurationHelper.EnableMex)
                serviceHost.AddMexEndpoints();
            if (ConfigurationHelper.EnableDiscovery)
                serviceHost.EnableDiscovery();
            if (ConfigurationHelper.UpscaleBindings)
                UpscaleBindings(serviceHost);
        }

        /// <summary>
        /// Upscales the bindings.
        /// </summary>
        /// <param name="serviceHost">The service host.</param>
        public static void UpscaleBindings(this ServiceHostBase serviceHost)
        {
            foreach (var item in serviceHost.Description.Endpoints)
            {
                item.Binding.Upscale();
            }
        }



        /// <summary>
        /// Adds the rest endpoints.
        /// </summary>
        /// <param name="serviceHost">The service host.</param>
        public static void AddRestEndpoints(this ServiceHostBase serviceHost)
        {
            //serviceHost.AddServiceEndpoint(new WebHttpEndpoint(contract));
        }


        /// <summary>
        /// Adds the mex endpoints.
        /// </summary>
        /// <param name="host">The service host.</param>
        public static void AddMexEndpoints(this ServiceHostBase host)
        {
            Type T = host.GetType();
            var logger = LogManager.GetLogger(T.Name);
            var mexBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (mexBehavior == null)
            {
                mexBehavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(mexBehavior);
            }
            logger.Debug("Checking possible MEX endpoints...");
            foreach (Uri baseAddress in host.BaseAddresses)
            {
                logger.Debug("\tAdding endpoint {0}", baseAddress.AbsoluteUri);
                if (baseAddress.Scheme == Uri.UriSchemeNetPipe)
                {
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                            MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                                            "mex");
                }
                if (baseAddress.Scheme == Uri.UriSchemeHttps)
                {
                    if (ConfigurationHelper.SecureMex)
                    {
                        mexBehavior.HttpsGetEnabled = true;
                        mexBehavior.HttpsGetUrl = baseAddress;
                        host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                            MetadataExchangeBindings.CreateMexHttpsBinding(),
                            "mex");
                    }
                }
                if (baseAddress.Scheme == Uri.UriSchemeHttp)
                {
                    if (!ConfigurationHelper.SecureMex)
                    {
                        mexBehavior.HttpGetEnabled = true;
                        mexBehavior.HttpGetUrl = baseAddress;
                        host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                            MetadataExchangeBindings.CreateMexHttpBinding(),
                            "mex");
                    }
                }
                if (baseAddress.Scheme == Uri.UriSchemeNetTcp)
                {
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                            MetadataExchangeBindings.CreateMexTcpBinding(),
                                            "mex");
                }
            }

        }


      

        






    }
}
