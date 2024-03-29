﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeploymentUpdate.Service.Client.Configuration {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Configuration.IConfigurationService")]
    public interface IConfigurationService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/GetRecipients", ReplyAction="http://tempuri.org/IConfigurationService/GetRecipientsResponse")]
        System.Collections.Generic.List<DeploymentUpdate.Service.UI.Interface.Configuration.MailAddressSurrogate> GetRecipients();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/UpdateRecipients", ReplyAction="http://tempuri.org/IConfigurationService/UpdateRecipientsResponse")]
        void UpdateRecipients(System.Collections.Generic.List<DeploymentUpdate.Service.UI.Interface.Configuration.MailAddressSurrogate> recipients);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/GetWatcherPath", ReplyAction="http://tempuri.org/IConfigurationService/GetWatcherPathResponse")]
        string GetWatcherPath();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/UpdateWatcherPath", ReplyAction="http://tempuri.org/IConfigurationService/UpdateWatcherPathResponse")]
        void UpdateWatcherPath(string watherPath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/GetUriToWatchPath", ReplyAction="http://tempuri.org/IConfigurationService/GetUriToWatchPathResponse")]
        string GetUriToWatchPath();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigurationService/UpdateUriToWatchPath", ReplyAction="http://tempuri.org/IConfigurationService/UpdateUriToWatchPathResponse")]
        void UpdateUriToWatchPath(string uriToWathPath);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IConfigurationServiceChannel : DeploymentUpdate.Service.Client.Configuration.IConfigurationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ConfigurationServiceClient : System.ServiceModel.ClientBase<DeploymentUpdate.Service.Client.Configuration.IConfigurationService>, DeploymentUpdate.Service.Client.Configuration.IConfigurationService {
        
        public ConfigurationServiceClient() {
        }
        
        public ConfigurationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConfigurationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigurationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigurationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Collections.Generic.List<DeploymentUpdate.Service.UI.Interface.Configuration.MailAddressSurrogate> GetRecipients() {
            return base.Channel.GetRecipients();
        }
        
        public void UpdateRecipients(System.Collections.Generic.List<DeploymentUpdate.Service.UI.Interface.Configuration.MailAddressSurrogate> recipients) {
            base.Channel.UpdateRecipients(recipients);
        }
        
        public string GetWatcherPath() {
            return base.Channel.GetWatcherPath();
        }
        
        public void UpdateWatcherPath(string watherPath) {
            base.Channel.UpdateWatcherPath(watherPath);
        }
        
        public string GetUriToWatchPath() {
            return base.Channel.GetUriToWatchPath();
        }
        
        public void UpdateUriToWatchPath(string uriToWathPath) {
            base.Channel.UpdateUriToWatchPath(uriToWathPath);
        }
    }
}
