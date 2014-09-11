// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 11-30-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-01-2013
// ***********************************************************************
// <copyright file="CloudServiceHost.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Wulka.Networking.Wcf
{

    /// <summary>
    /// Class CloudServiceHost.
    /// </summary>
    public class CloudServiceHost : ServiceHost, IServiceBehavior
    {

        /// <summary>
        /// The _service type
        /// </summary>
        private readonly Type _serviceType;




        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ServiceHost" /> class with the type of service and its base addresses specified.
        /// </summary>
        /// <param name="serviceType">The type of hosted service.</param>
        /// <param name="baseAddresses">An array of type <see cref="T:System.Uri" /> that contains the base addresses for the hosted service.</param>
        public CloudServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses) 
        {
            _serviceType = serviceType;
        }



        //Overriding ApplyConfiguration() allows us to 
        //alter the ServiceDescription prior to opening
        //the service host. 
        /// <summary>
        /// Loads the service description information from the configuration file and applies it to the runtime being constructed.
        /// </summary>
        protected override void ApplyConfiguration()
        {
            //First, we call base.ApplyConfiguration()
            //to read any configuration that was provided for
            //the service we're hosting. After this call,
            //this.Description describes the service
            //as it was configured.
            base.ApplyConfiguration();
            //AddSecureEndpoints();
            //MakeAnnouncingService();
        }

        /// <summary>
        /// Adds the secure endpoints.
        /// </summary>
        public void AddSecureEndpoints()
        {
            var contracts = SentryHostHelper.GetServiceContracts(_serviceType);
            var bnd = BindingFactory.CreateBindingFromKey(BindingFactory.Key.WsHttpBindingTransportSecurity);
            foreach (var contract in contracts)
            {
                AddServiceEndpoint(contract, bnd, "");
            }
        }


        ///// <summary>
        ///// Enables the discovery.
        ///// </summary>
        ///// <exception cref="System.ApplicationException"></exception>
        //private void EnableDiscovery()
        //{
        //    string announcementEndpointUrl = ConfigurationManager.AppSettings[DiscoAppSettingsKey.Announce];
        //    if (String.IsNullOrWhiteSpace(announcementEndpointUrl))
        //    {
        //        string errorMessage = string.Format(
        //            "No value found for key '{0}' in configuration file"
        //            + ", please provide a key '{0}' in the section AppConfig and set its value to the appropriate announcement endpoint url",
        //            DiscoAppSettingsKey.Announce
        //            );
        //        throw new ApplicationException(errorMessage);
        //    }
        //    var announcementEndpoint = new AnnouncementEndpoint(BindingFactory.CreateBindingFromKey(BindingFactory.Key.WsHttpBindingNoSecurity),
        //        new EndpointAddress(announcementEndpointUrl));
        //    var discovery = new ServiceDiscoveryBehavior();
        //    discovery.AnnouncementEndpoints.Add(announcementEndpoint);
        //    Description.Behaviors.Add(discovery);
        //}





        ///// <summary>
        ///// Makes the announcing service.
        ///// </summary>
        //public void MakeAnnouncingService()
        //{
        //    if (ConfigurationHelper.EnableMex)
        //        AddMexEndpoints();
        //    if (ConfigurationHelper.EnableDiscovery)
        //        EnableDiscovery();
        //    if (ConfigurationHelper.UpscaleBindings)
        //        UpscaleBindings();
        //}

        ///// <summary>
        ///// Upscales the bindings.
        ///// </summary>
        //private void UpscaleBindings()
        //{
        //    foreach (var item in Description.Endpoints)
        //    {
        //        item.Binding.Upscale();
        //    }
        //}


        ///// <summary>
        ///// Adds the mex endpoints.
        ///// </summary>
        //private void AddMexEndpoints()
        //{
        //    var mexBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
        //    if (mexBehavior == null)
        //    {
        //        mexBehavior = new ServiceMetadataBehavior();
        //        Description.Behaviors.Add(mexBehavior);
        //    }
        //    foreach (Uri baseAddress in BaseAddresses)
        //    {
        //        if (baseAddress.Scheme == Uri.UriSchemeHttp)
        //        {
        //            mexBehavior.HttpGetEnabled = true;
        //            mexBehavior.HttpGetUrl = baseAddress;
        //            AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
        //                MetadataExchangeBindings.CreateMexHttpBinding(),
        //                "mex");
        //        }
        //        else if (baseAddress.Scheme == Uri.UriSchemeHttps)
        //        {
        //            mexBehavior.HttpsGetEnabled = true;
        //            mexBehavior.HttpsGetUrl = baseAddress;
        //            AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
        //                MetadataExchangeBindings.CreateMexHttpsBinding(),
        //                "mex");
        //        }
        //        else if (baseAddress.Scheme == Uri.UriSchemeNetPipe)
        //        {
        //            AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
        //                MetadataExchangeBindings.CreateMexNamedPipeBinding(),
        //                "mex");
        //        }
        //        else if (baseAddress.Scheme == Uri.UriSchemeNetTcp)
        //        {
        //            AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
        //                MetadataExchangeBindings.CreateMexTcpBinding(),
        //                "mex");
        //        }
        //    }
        //}






        


        #region Implementation of IServiceBehavior

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, 
            System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            //       serviceHostBase.EnableDiscovery(true);
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // Add custom error handler
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(new FaultErrorHandler());
            }
        }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // Nothing to do
        }

        #endregion
    }

}
