using System;
using System.ServiceModel;
using Wulka.Configuration;
using Wulka.Interfaces;

namespace Wulka.Networking.Wcf
{
    public class DiscoHelper
    {

        /// <summary>
        /// Creates the reusable fault unwrapping disco client.
        /// </summary>
        /// <returns></returns>
        public static IDisco CreateReusableFaultUnwrappingDiscoClient(string cfg=null)
        {
            if(String.IsNullOrWhiteSpace(cfg))
                cfg = ConfigurationHelper.DiscoEndpoint;
            var address = new EndpointAddress(cfg);
            var uri = new Uri(cfg);
            if (uri.Scheme == Uri.UriSchemeNetPipe)
            {
                return WCFClientProxy<IDisco>.GetReusableFaultUnwrappingInstance(
                    BindingFactory.CreateBindingFromKey(BindingFactory.Key.NetNamedPipeBinding),
                    address);
            } else if (uri.Scheme == Uri.UriSchemeHttp)
            {
                return WCFClientProxy<IDisco>.GetReusableFaultUnwrappingInstance(
                    BindingFactory.CreateBindingFromKey(BindingFactory.Key.BasicHttpBindingNoSecurity),
                    address);
            }
            else if(uri.Scheme == Uri.UriSchemeHttps)
            {
                return WCFClientProxy<IDisco>.GetReusableFaultUnwrappingInstance(
                    BindingFactory.CreateBindingFromKey(BindingFactory.Key.BasicHttpBindingTransportSecurity),
                    address);
            }
            else
            {
                return WCFClientProxy<IDisco>.GetReusableFaultUnwrappingInstance(cfg);
            }
        }



        /// <summary>
        /// Creates the reusable disco client.
        /// </summary>
        /// <returns></returns>
        public static IDisco CreateReusableDiscoClient(string cfg=null)
        {
            if(String.IsNullOrWhiteSpace(cfg))
                cfg = ConfigurationHelper.DiscoEndpoint;
            var address = new EndpointAddress(cfg);
            var uri = new Uri(cfg);
            if (uri.Scheme == Uri.UriSchemeNetPipe)
            {
                return WCFClientProxy<IDisco>.GetReusableInstance(
                    BindingFactory.CreateBindingFromKey(BindingFactory.Key.NetNamedPipeBinding),
                    address);
            }
            else if (uri.Scheme == Uri.UriSchemeHttp)
            {
                return WCFClientProxy<IDisco>.GetReusableInstance(
                    BindingFactory.CreateBindingFromKey(BindingFactory.Key.BasicHttpBindingNoSecurity),
                    address);
            }
            else if (uri.Scheme == Uri.UriSchemeHttps)
            {
                return WCFClientProxy<IDisco>.GetReusableInstance(
                    BindingFactory.CreateBindingFromKey(BindingFactory.Key.BasicHttpBindingTransportSecurity),
                    address);
            }
            else
            {
                return WCFClientProxy<IDisco>.GetReusableInstance(cfg);
            }
        }


        /// <summary>
        /// Gets the CLT.
        /// </summary>
        /// <param name="inst">The inst.</param>
        /// <returns></returns>
        public static WCFClientProxy<IDisco> GetClt(IDisco inst)
        {
            return (inst as WCFClientProxy<IDisco>);
        }


    }
}
