// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 11-30-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-01-2013
// ***********************************************************************
// <copyright file="MetadataHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class MetadataHelper.
    /// </summary>
    public static class MetadataHelper
    {

        /// <summary>
        /// Queries the mex endpoint.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="bindingElement">The binding element.</param>
        /// <returns>ServiceEndpointCollection.</returns>
        static ServiceEndpointCollection QueryMexEndpoint(string mexAddress, BindingElement bindingElement)
        {
            var binding = new CustomBinding(bindingElement);
            var mexClient = new MetadataExchangeClient(binding);
            var metadata = mexClient.GetMetadata(new EndpointAddress(mexAddress));
            MetadataImporter importer = new WsdlImporter(metadata);
            return importer.ImportAllEndpoints();
        }


        /// <summary>
        /// Queries the mex contracts.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="bindingElement">The binding element.</param>
        /// <returns>IEnumerable{ContractDescription}.</returns>
        static IEnumerable<ContractDescription> QueryMexContracts(string mexAddress, BindingElement bindingElement)
        {
            var binding = new CustomBinding(bindingElement);
            var mexClient = new MetadataExchangeClient(binding);
            var metadata = mexClient.GetMetadata(new EndpointAddress(mexAddress));
            MetadataImporter importer = new WsdlImporter(metadata);
            return importer.ImportAllContracts();
        }





        /// <summary>
        /// Gets the endpoints.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>ServiceEndpoint[][].</returns>
        public static ServiceEndpoint[] GetEndpoints(string mexAddress, Type contractType)
        {
            var endpoints = GetEndpoints(mexAddress);
            var description = ContractDescription.GetContract(contractType);
            return endpoints
                .Where((endpoint) => (endpoint.Contract.Name == description.Name) && 
                                     (endpoint.Contract.Namespace == description.Namespace))
                .ToArray();
        }




        /// <summary>
        /// Gets the endpoints.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <returns>ServiceEndpoint[][].</returns>
        public static ServiceEndpoint[] GetEndpoints(string mexAddress)
        {
            if (String.IsNullOrEmpty(mexAddress))
            {
                Debug.Assert(false, "Empty address");
                return null;
            }
            var address = new Uri(mexAddress);
            ServiceEndpointCollection endpoints = null;
            if (address.Scheme == Uri.UriSchemeHttp)
            {
                var be = new HttpTransportBindingElement {MaxReceivedMessageSize = BindingHelper.BindingConst.MaxReceivedMessageSize};
                endpoints = GetEndpointsViaHttpMex(mexAddress,  be);
            }
            else if (address.Scheme == Uri.UriSchemeHttps)
            {
                var be = new HttpsTransportBindingElement {MaxReceivedMessageSize = BindingHelper.BindingConst.MaxReceivedMessageSize};
                endpoints = GetEndpointsViaHttpMex(mexAddress, be);
            } else if (address.Scheme == Uri.UriSchemeNetTcp)
            {
                var be = new TcpTransportBindingElement {MaxReceivedMessageSize = BindingHelper.BindingConst.MaxReceivedMessageSize};
                endpoints = QueryMexEndpoint(mexAddress, be);
            } else if (address.Scheme == Uri.UriSchemeNetPipe)
            {
                var be = new NamedPipeTransportBindingElement {MaxReceivedMessageSize = BindingHelper.BindingConst.MaxReceivedMessageSize};
                endpoints = QueryMexEndpoint(mexAddress, be);
            }
            return endpoints.ToArray();
        }

        /// <summary>
        /// Gets the HTTP endpoints.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="bindingElement">The binding element.</param>
        /// <returns>ServiceEndpointCollection.</returns>
        private static ServiceEndpointCollection GetEndpointsViaHttpMex(string mexAddress, TransportBindingElement bindingElement)
        {
            ServiceEndpointCollection endpoints = null;
            try
            {
                endpoints = QueryMexEndpoint(mexAddress, bindingElement);
            }
            catch(Exception exc)
            {
                Debug.Print(exc.Message);
                try
                {
                    endpoints = QueryMexEndpointViaHttpGet(mexAddress, bindingElement);
                }
                catch (Exception exception)
                {
                    Debug.Print(exception.Message);
                    throw;
                }
            }
            return endpoints;
        }

        /// <summary>
        /// Queries the mex endpoint via HTTP get.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="bindingElement">The binding element.</param>
        /// <returns>ServiceEndpointCollection.</returns>
        private static ServiceEndpointCollection QueryMexEndpointViaHttpGet(string mexAddress, TransportBindingElement bindingElement)
        {
            ServiceEndpointCollection endpoints = null;
            if (mexAddress.EndsWith("?wsdl") == false)
            {
                mexAddress += "?wsdl";
            }
            var binding = new CustomBinding(bindingElement);
            var mexClient = new MetadataExchangeClient(binding);
            var metadata = mexClient.GetMetadata(new Uri(mexAddress), MetadataExchangeClientMode.HttpGet);
            var importer = new WsdlImporter(metadata);
            endpoints = importer.ImportAllEndpoints();
            return endpoints;
        }



        /// <summary>
        /// Gets the callback contract.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>Type.</returns>
        public static Type GetCallbackContract(string mexAddress, Type contractType)
        {
            if (contractType.IsInterface == false)
            {
                Debug.Assert(false, contractType + " is not an interface");
                return null;
            }
            var attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false);
            if (attributes.Length == 0)
            {
                Debug.Assert(false, "Interface " + contractType + " does not have the ServiceContractAttribute");
                return null;
            }
            var description = ContractDescription.GetContract(contractType);
            return GetCallbackContract(mexAddress, description.Namespace, description.Name);
        }

        /// <summary>
        /// Gets the callback contract.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractNamespace">The contract namespace.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>Type.</returns>
        public static Type GetCallbackContract(string mexAddress, string contractNamespace, string contractName)
        {
            if (String.IsNullOrEmpty(contractNamespace))
            {
                Debug.Assert(false, "Empty namespace");
                return null;
            }
            if (String.IsNullOrEmpty(contractName))
            {
                Debug.Assert(false, "Empty name");
                return null;
            }
            try
            {
                ServiceEndpoint[] endpoints = GetEndpoints(mexAddress);
                foreach (ServiceEndpoint endpoint in endpoints)
                {
                    if (endpoint.Contract.Namespace == contractNamespace && endpoint.Contract.Name == contractName)
                    {
                        return endpoint.Contract.CallbackContractType;
                    }
                }
            }
            catch
            { }
            return null;
        }

        /// <summary>
        /// Queries the contract.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool QueryContract(string mexAddress, Type contractType)
        {
            if (contractType.IsInterface == false)
            {
                Debug.Assert(false, contractType + " is not an interface");
                return false;
            }

            var attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false);
            if (attributes.Length == 0)
            {
                Debug.Assert(false, "Interface " + contractType + " does not have the ServiceContractAttribute");
                return false;
            }

            var description = ContractDescription.GetContract(contractType);

            return QueryContract(mexAddress, description.Namespace, description.Name);
        }
        /// <summary>
        /// Queries the contract.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractNamespace">The contract namespace.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool QueryContract(string mexAddress, string contractNamespace, string contractName)
        {
            if (String.IsNullOrEmpty(contractNamespace))
            {
                Debug.Assert(false, "Empty namespace");
                return false;
            }
            if (String.IsNullOrEmpty(contractName))
            {
                Debug.Assert(false, "Empty name");
                return false;
            }
            try
            {
                var endpoints = GetEndpoints(mexAddress);
                return endpoints.Any(endpoint => endpoint.Contract.Namespace == contractNamespace && endpoint.Contract.Name == contractName);
            }

            catch
            { }
            return false;
        }
        /// <summary>
        /// Gets the contracts.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <returns>ContractDescription[][].</returns>
        public static ContractDescription[] GetContracts(string mexAddress)
        {
            return GetContracts(typeof(Binding), mexAddress);
        }
        /// <summary>
        /// Gets the contracts.
        /// </summary>
        /// <param name="bindingType">Type of the binding.</param>
        /// <param name="mexAddress">The mex address.</param>
        /// <returns>ContractDescription[][].</returns>
        public static ContractDescription[] GetContracts(Type bindingType, string mexAddress)
        {
            Debug.Assert(bindingType.IsSubclassOf(typeof(Binding)) || bindingType == typeof(Binding));
            var endpoints = GetEndpoints(mexAddress);
            var contracts = new List<ContractDescription>();
            ContractDescription contract;
            foreach (var endpoint in endpoints.Where(endpoint => bindingType.IsInstanceOfType(endpoint.Binding)))
            {
                contract = new ContractDescription(endpoint.Contract.Name, endpoint.Contract.Namespace);
                if (contracts.Any((item) => item.Name == contract.Name && item.Namespace == contract.Namespace) == false)
                {
                    contracts.Add(contract);
                }
            }
            return contracts.ToArray();
        }
        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>System.String[][].</returns>
        public static string[] GetAddresses(string mexAddress, Type contractType)
        {
            if (contractType.IsInterface == false)
            {
                Debug.Assert(false, contractType + " is not an interface");
                return new string[] { };
            }

            var attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false);
            if (attributes.Length == 0)
            {
                Debug.Assert(false, "Interface " + contractType + " does not have the ServiceContractAttribute");
                return new string[] { };
            }
            ContractDescription description = ContractDescription.GetContract(contractType);

            return GetAddresses(mexAddress, description.Namespace, description.Name);
        }
        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractNamespace">The contract namespace.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>System.String[][].</returns>
        public static string[] GetAddresses(string mexAddress, string contractNamespace, string contractName)
        {
            var endpoints = GetEndpoints(mexAddress);
            var addresses = new List<string>();
            foreach (var endpoint in endpoints
                .Where(endpoint => endpoint.Contract.Namespace == contractNamespace && endpoint.Contract.Name == contractName))
            {
                Debug.Assert(addresses.Contains(endpoint.Address.Uri.AbsoluteUri) == false);
                addresses.Add(endpoint.Address.Uri.AbsoluteUri);
            }
            return addresses.ToArray();
        }
        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <param name="bindingType">Type of the binding.</param>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>System.String[][].</returns>
        public static string[] GetAddresses(Type bindingType, string mexAddress, Type contractType)
        {
            Debug.Assert(bindingType.IsSubclassOf(typeof(Binding)) || bindingType == typeof(Binding));
            if (contractType.IsInterface == false)
            {
                Debug.Assert(false, contractType + " is not an interface");
                return new string[] { };
            }
            var attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false);
            if (attributes.Length == 0)
            {
                Debug.Assert(false, "Interface " + contractType + " does not have the ServiceContractAttribute");
                return new string[] { };
            }
            var description = ContractDescription.GetContract(contractType);

            return GetAddresses(bindingType, mexAddress, description.Namespace, description.Name);
        }
        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <param name="bindingType">Type of the binding.</param>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractNamespace">The contract namespace.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>System.String[][].</returns>
        public static string[] GetAddresses(Type bindingType, string mexAddress, string contractNamespace, string contractName)
        {
            Debug.Assert(bindingType.IsSubclassOf(typeof(Binding)) || bindingType == typeof(Binding));

            var endpoints = GetEndpoints(mexAddress);

            var addresses = new List<string>();

            foreach (ServiceEndpoint endpoint in endpoints
                .Where(endpoint => bindingType.IsInstanceOfType(endpoint.Binding)).Where(endpoint => endpoint.Contract.Namespace == contractNamespace && endpoint.Contract.Name == contractName))
            {
                Debug.Assert(addresses.Contains(endpoint.Address.Uri.AbsoluteUri) == false);
                addresses.Add(endpoint.Address.Uri.AbsoluteUri);
            }
            return addresses.ToArray();
        }
        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>System.String[][].</returns>
        public static string[] GetOperations(string mexAddress, Type contractType)
        {
            if (contractType.IsInterface == false)
            {
                Debug.Assert(false, contractType + " is not an interface");
                return new string[] { };
            }

            var attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false);
            if (attributes.Length == 0)
            {
                Debug.Assert(false, "Interface " + contractType + " does not have the ServiceContractAttribute");
                return new string[] { };
            }
            var description = ContractDescription.GetContract(contractType);

            return GetOperations(mexAddress, description.Namespace, description.Name);
        }
        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <param name="contractNamespace">The contract namespace.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>System.String[][].</returns>
        public static string[] GetOperations(string mexAddress, string contractNamespace, string contractName)
        {
            var endpoints = GetEndpoints(mexAddress);

            var operations = new List<string>();

            foreach (ServiceEndpoint endpoint in endpoints
                .Where(endpoint => endpoint.Contract.Namespace == contractNamespace && endpoint.Contract.Name == contractName))
            {
                foreach (var operation in endpoint.Contract.Operations)
                {
                    Debug.Assert(operations.Contains(operation.Name) == false);
                    operations.Add(operation.Name);
                }
                break;
            }
            return operations.ToArray();
        }



        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <param name="mexAddress">The mex address.</param>
        /// <returns>OperationDescription[][].</returns>
        public static OperationDescription[] GetOperationDescriptions(string mexAddress)
        {
            var res = new List<OperationDescription>();
            var endpoints = GetEndpoints(mexAddress);

            var operations = new List<string>();

            foreach (var endpoint in endpoints)
            {
                res.AddRange(endpoint.Contract.Operations);
            }
            return res.ToArray();
        }




        


    }
}
