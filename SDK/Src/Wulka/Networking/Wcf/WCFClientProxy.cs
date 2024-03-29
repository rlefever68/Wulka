using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Wulka.Core;

namespace Wulka.Networking.Wcf
{
	/// <summary>
	/// Dynamic generator for a WCF ClientBase proxies
	/// </summary>
	/// <typeparam name="TInterface">The Type of Service Contract on which you want to build a Dynamic Client Proxy</typeparam>
	public  class WCFClientProxy<TInterface> where TInterface : class
	{
		//private static readonly IDictionary<Type, string> registeredContracts = new Dictionary<Type, string>();

		///// <summary>
		///// Registers a specific interface with a specific configuration.
		///// </summary>
		///// <param name="configName"></param>
		//public static void RegisterEndpoint(string configName)
		//{
		//    lock (registeredContracts)
		//    {
		//        registeredContracts[typeof (TInterface)] = configName;

		//        // force the Class builder to generate it's type
		//        ProxyBuilder.BuildType<TInterface, WCFProxyClassBuilder<TInterface>>();
		//    }
		//}

		///// <summary>
		///// Returns a configured registered instance.
		///// </summary>
		///// <returns></returns>
		//public static TInterface GetRegisteredInstance()
		//{
		//    lock (registeredContracts)
		//    {
		//        string configName;
		//        if (registeredContracts.TryGetValue(typeof (TInterface), out configName))
		//        {
		//            return GetReusableInstance(configName);
		//        }
		//    }
		//    throw new ApplicationException("Could not find registered contract:" + typeof (TInterface).FullName);
		//}

		/// <summary>
		/// Returns a new instance for a client proxy over the specified interface with the specified config name used for initialization.
		/// This is a simple instance of a ClientBase derived class.
		/// </summary>
		public static TInterface GetInstance(string configName)
		{
			TInterface pooledInstance = ProxyConnectionPool.RequestFromPool<TInterface>();

			if ( pooledInstance != null )
				return pooledInstance;

			// Build the type
			Type type = ProxyBuilder.BuildType<TInterface, WCFProxyClassBuilder<TInterface>>();

			// Create new instance
			TInterface instance = (TInterface) Activator.CreateInstance(type, new object[] {configName});

			ProxyConnectionPool.Register(instance);

			return instance;
		}



        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public static TInterface GetInstance(Binding binding, EndpointAddress endpoint, ClientCredentials credentials)
        {
            TInterface pooledInstance = ProxyConnectionPool.RequestFromPool<TInterface>();

            if (pooledInstance != null)
                return pooledInstance;

            // Build the type
            Type type = ProxyBuilder.BuildType<TInterface, WCFProxyClassBuilder<TInterface>>();

            // Create new instance
            TInterface instance = (TInterface)Activator.CreateInstance(type, new object[] { binding, endpoint, credentials });

            ProxyConnectionPool.Register(instance);

            return instance;
        }




		/// <summary>
		/// Returns the type of class that represents a proxy over the specified interface with the specified config name used for initialization.
		/// This is a simple instance of a ClientBase derived class.
		/// </summary>
		public static Type GetInstanceType()
		{
			// Build the type
			Type type = ProxyBuilder.BuildType<TInterface, WCFProxyClassBuilder<TInterface>>();

			return type;
		}

		/// <summary>
		/// Returns a new instance for a client proxy over the specified interface with the specified config name used for initialization.
		/// This instance of the proxy can be reused as it will always "clean" itself
		/// if the channel is faulted.
		/// </summary>
		public static TInterface GetReusableInstance(string configName)
		{
			TInterface pooledInstance = ProxyConnectionPool.RequestFromPool<TInterface>();

			if (pooledInstance != null)
				return pooledInstance;

			// Build the type
			Type type = ProxyBuilder.BuildType<TInterface, WCFReusableProxyClassBuilder<TInterface>>();

			// Create new instance
			TInterface instance = (TInterface) Activator.CreateInstance(type, new object[] {configName});

			ProxyConnectionPool.Register(instance);

			return instance;
		}



        /// <summary>
        /// Returns a new instance for a client proxy over the specified interface with the specified config name used for initialization.
        /// This instance of the proxy can be reused as it will always "clean" itself
        /// if the channel is faulted.
        /// </summary>
        public static TInterface GetReusableInstance(Binding binding, EndpointAddress address)
        {
            TInterface pooledInstance = ProxyConnectionPool.RequestFromPool<TInterface>();

            if (pooledInstance != null)
                return pooledInstance;

            // Build the type
            Type type = ProxyBuilder.BuildType<TInterface, WCFReusableProxyClassBuilder<TInterface>>();

            // Create new instance
            TInterface instance = (TInterface)Activator.CreateInstance(type, new object[] { binding, address });

            ProxyConnectionPool.Register(instance);

            return instance;
        }





		private static Type _reusableFaultUnwrappingInstance;

		/// <summary>
		/// Returns a new instance for a client proxy over the specified interface with the specified config name used for initialization.
		/// This instance of the proxy can be reused as it will always "clean" itself
		/// if the channel is faulted.
		/// The class will also unwrap any FaultException and throw the original Exception.
		/// </summary>
		public static TInterface GetReusableFaultUnwrappingInstance(string configName)
		{
			TInterface pooledInstance = ProxyConnectionPool.RequestFromPool<TInterface>();

			if (pooledInstance != null)
				return pooledInstance;

			if (_reusableFaultUnwrappingInstance == null)
			{
				// no need to lock as the ProxyBuilder is thread safe and will always return the same stuff
				// Build the type
				_reusableFaultUnwrappingInstance = ProxyBuilder.BuildType<TInterface, WCFReusableFaultWrapperProxyClassBuilder<TInterface>>();
			}

			// Create new instance
			TInterface instance = (TInterface) Activator.CreateInstance(_reusableFaultUnwrappingInstance, new object[] {configName});

			ProxyConnectionPool.Register(instance);

			return instance;
		}


        /// <summary>
        /// Returns a new instance for a client proxy over the specified interface with the specified config name used for initialization.
        /// This instance of the proxy can be reused as it will always "clean" itself
        /// if the channel is faulted.
        /// The class will also unwrap any FaultException and throw the original Exception.
        /// </summary>
        public static TInterface GetReusableFaultUnwrappingInstance(Binding binding, EndpointAddress address)
        {
            TInterface pooledInstance = ProxyConnectionPool.RequestFromPool<TInterface>();

            if (pooledInstance != null)
                return pooledInstance;

            if (_reusableFaultUnwrappingInstance == null)
            {
                // no need to lock as the ProxyBuilder is thread safe and will always return the same stuff
                // Build the type
                _reusableFaultUnwrappingInstance = ProxyBuilder.BuildType<TInterface, WCFReusableFaultWrapperProxyClassBuilder<TInterface>>();
            }

            // Create new instance
            TInterface instance = (TInterface)Activator.CreateInstance(_reusableFaultUnwrappingInstance, new object[] { binding, address });

            ProxyConnectionPool.Register(instance);

            return instance;
        }




		///// <summary>
		///// Returns a new instance for a client proxy over the specified interface with the specified config name used for initialization.
		///// </summary>
		//public static TInterface GetPolicyInstance(string configName)
		//{
		//    // Create new instance
		//    TInterface instance = GetReusableInstance(configName);

		//    return PolicyInjection.Wrap<TInterface>(instance);
		//}
	}
}