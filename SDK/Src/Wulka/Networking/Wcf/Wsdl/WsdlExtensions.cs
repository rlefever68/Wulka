﻿using System;
using System.ServiceModel.Description;
using Wulka.Networking.Wcf.Wsdl.Documentation;

namespace Wulka.Networking.Wcf.Wsdl
{
    public class WsdlExtensions : IEndpointBehavior, IWsdlExportExtension
    {
        #region EndpointBehavior - Not used
        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        #endregion

        public Uri Location { get; set; }

        public XmlCommentFormat ExportXmlComments { get; set; }

        public bool SingleFile { get; set; }

        internal WsdlExtensions(WsdlExtensionsConfig config)
        {
            this.Location = config.Location;
            this.SingleFile = config.SingleFile;
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            if (SingleFile)
                SingleFileExporter.ExportEndpoint(exporter);

            if (Location != null)
            {
                LocationOverrideExporter.ExportEndpoint(exporter, context, Location);
            }
        }
    }
}
