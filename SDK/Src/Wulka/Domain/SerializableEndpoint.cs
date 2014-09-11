using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Wulka.Domain.Base;
using Wulka.Validation;

namespace Wulka.Domain
{
    [DataContract]
    public class SerializableEndpoint : DomainObject<SerializableEndpoint>

    {

        ServiceEndpoint endpoint;

        int priority;



        public SerializableEndpoint(ServiceEndpoint endpoint, int priority)
        {
            this.endpoint = SanitizeContractInEndpoint(endpoint);
            this.priority = priority;
        }



        public EndpointAddress Address
        {
            get { return this.endpoint.Address; }
        }



        public Binding Binding
        {
            get { return this.endpoint.Binding; }
        }



        public string ContractName
        {
            get { return this.endpoint.Contract.Name; }
        }



        public string ContractNamespace
        {
            get { return this.endpoint.Contract.Namespace; }
        }



        public string Name
        {
            get { return this.endpoint.Name; }
        }



     



        [DataMember]
        private MetadataSet Metadata
        {
            get
            {

                WsdlExporter exporter = new WsdlExporter();
                exporter.ExportEndpoint(this.endpoint);
                return exporter.GetGeneratedMetadata();
            }
            set
            {
                WsdlImporter importer = new WsdlImporter(value);
                ServiceEndpointCollection endpoints = importer.ImportAllEndpoints();
                if (endpoints.Count != 1)
                {
                    throw new ArgumentException("MetadataBundle must contain exactly one endpoint.");
                }
                this.endpoint = SanitizeContractInEndpoint(endpoints[0]);
            }
        }



        /// <summary>
        /// Sanitizes the contract in endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns></returns>
        private ServiceEndpoint SanitizeContractInEndpoint(ServiceEndpoint endpoint)
        {

            ServiceEndpoint newEndpoint = new ServiceEndpoint(new ContractDescription(endpoint.Contract.Name, endpoint.Contract.Namespace));
            newEndpoint.Address = endpoint.Address;
            newEndpoint.Binding = endpoint.Binding;
            newEndpoint.Name = endpoint.Name;
            return newEndpoint;

        }


        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<SerializableEndpoint>.Validate(this, columnName);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<SerializableEndpoint>.Validate(this);
        }
    }
}
