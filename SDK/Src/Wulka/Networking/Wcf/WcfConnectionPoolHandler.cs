using System;
using System.Web;

namespace Wulka.Networking.Wcf
{
	public class WcfConnectionPoolHandler : IHttpModule
	{
		public WcfConnectionPoolHandler()
		{
			// Automatically start the connection pooling
			ProxyConnectionPool.EnableConnectionPool = true;
		}

		#region IHttpModule Members

		public void Dispose()
		{
			
		}

		public void Init(HttpApplication context)
		{
			context.EndRequest += new EventHandler(context_EndRequest);
		}

		void context_EndRequest(object sender, EventArgs e)
		{
			if (ProxyConnectionPool.EnableConnectionPool)
			{
				ProxyConnectionPool.Current.Dispose();
			}
		}

		#endregion
	}
}
