using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Wulka.Networking.Wcf
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class WCFReusableFaultWrapperChannelProxy<T> : WCFReusableFaultWrapperClientProxy<T> where T : class
	{
		private static readonly Dictionary<string, ChannelFactory<T>> factoryCache = new Dictionary<string, ChannelFactory<T>>();
		private ChannelFactory<T> factory;

		protected WCFReusableFaultWrapperChannelProxy(string configName)
			: base(configName)
		{
		}

		protected override T CreateProxyInstance()
		{
			AssureFactory();

			return factory.CreateChannel();
		}

		private void AssureFactory()
		{
			if (factory == null)
			{
				lock (factoryCache)
				{
					if (!factoryCache.TryGetValue(ConfigName, out factory))
					{
						factory = new ChannelFactory<T>(ConfigName);
						factoryCache[ConfigName] = factory;
					}
				}
			}
		}

		public override System.ServiceModel.Description.ClientCredentials ClientCredentials
		{
			get
			{
				AssureProxy();
				return base.ClientCredentials;
			}
		}

		public override ServiceEndpoint CurrentEndpoint
		{
			get
			{
				if (manuallyClosed)
					return null;
				else
				{
					if (factory == null)
						return null;
					else
						return factory.Endpoint;
				}
			}
		}

		public override ServiceEndpoint Endpoint
		{
			get
			{
				AssureFactory();
				return factory.Endpoint;
			}
		}

		public override IClientChannel InnerChannel
		{
			get { return null; }
		}
	}
}