// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 05-26-2014
// ***********************************************************************
// <copyright file="ProxyConnectionPool.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web;
using Wulka.Logging;
using Wulka.Networking.Wcf.Interfaces;

namespace Wulka.Networking.Wcf
{
	/// <summary>
	/// A connection pool that will pickup all created dynamic proxies and will release them
	/// once the HttpConnection is finished or the WCF operation is finished.
	/// </summary>
	public class ProxyConnectionPool : IDisposable
	{
		/// <summary>
		/// The HTTP context pool key
		/// </summary>
		private const string HttpContextPoolKey = "Wulka.ProxyConnectionPool";

		/// <summary>
		/// The _open proxies
		/// </summary>
		private readonly Dictionary<IClientBase, IClientBase> _openProxies = new Dictionary<IClientBase, IClientBase>();
		/// <summary>
		/// The _open proxies by type
		/// </summary>
		private readonly Dictionary<Type, IClientBase> _openProxiesByType = new Dictionary<Type, IClientBase>();

		/// <summary>
		/// The _enable connection pool
		/// </summary>
		private static bool _enableConnectionPool;




		/// <summary>
		/// Prevents a default instance of the <see cref="ProxyConnectionPool"/> class from being created.
		/// </summary>
		private ProxyConnectionPool()
		{
		}

		// there can be a single connection pool per call
		// where a call is either a HttpContext request or a WCF Operation
		/// <summary>
		/// Registers the specified connection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection">The connection.</param>
		/// <exception cref="System.InvalidOperationException">Connection is not of type IClientBase. Are you sure this connection was created by this dynamic proxy?</exception>
		public static void Register<T>(T connection)
		{
			if (EnableConnectionPool)
			{
				IClientBase clientBase = connection as IClientBase;
				if (clientBase == null)
				{
					throw new InvalidOperationException("Connection is not of type IClientBase. Are you sure this connection was created by this dynamic proxy?");
				}
				clientBase.ProxyCreated += new ProxyCreatedHandler(clientBase_ProxyCreated);
				
				Current.InternalRegister(clientBase);
				Current._openProxiesByType[typeof(T)] = clientBase;
			}
		}
		/// <summary>
		/// Requests from pool.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>T.</returns>
		internal static T RequestFromPool<T>()
			where T : class
		{
			if (EnableConnectionPool)
			{
				IClientBase instance;
				if (Current._openProxiesByType.TryGetValue(typeof(T), out instance))
				{
					FxLog<ProxyConnectionPool>.DebugFormat("Retrieving proxy {0} from ConnectionPool.", instance);
					return (T) instance;
				}
			}
			return null;
		}


		/// <summary>
		/// Clients the base_ proxy created.
		/// </summary>
		/// <param name="proxy">The proxy.</param>
		static void clientBase_ProxyCreated(IClientBase proxy)
		{	// we only register in the connection pool when the proxy is created 
			// so the original instance can be moved safely accross threads
			ProxyConnectionPool.Current.InternalRegister(proxy);
		}


		/// <summary>
		/// Internals the register.
		/// </summary>
		/// <param name="clientBase">The client base.</param>
		private void InternalRegister(IClientBase clientBase)
		{
			FxLog<ProxyConnectionPool>.DebugFormat("Registering client {0}", clientBase);
			_openProxies[clientBase] = clientBase;
		}

		/// <summary>
		/// Disposes all connections.
		/// </summary>
		private void DisposeAllConnections()
		{
			foreach(IClientBase client in _openProxies.Values)
			{
				try
				{
					client.Close();
				}
				catch (Exception)
				{					
				}
			}
			_openProxies.Clear();
			if ( _proxyConnectionPool == this )
				_proxyConnectionPool = null;
		}



		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>The current.</value>
		public static ProxyConnectionPool Current
		{
			get
			{
				if (HttpContext.Current != null)
				{
					// we are in a web call, use the HttpContext to pool the proxies
					ProxyConnectionPool current = HttpContext.Current.Items[HttpContextPoolKey] as ProxyConnectionPool;
					if (current == null)
					{
						current = new ProxyConnectionPool();
						HttpContext.Current.Items[HttpContextPoolKey] = current;
					}
					return current;
				}
				else
				{
					// we are in a WCF OperationContext
					ProxyConnectionPoolExtension extension = ProxyConnectionPoolExtension.Current;
					if (extension != null)
					{
						return extension.Pool;
					}
					// fall back to a thread static
					if (_proxyConnectionPool == null)
					{
						_proxyConnectionPool = new ProxyConnectionPool();
					}
					return _proxyConnectionPool;
				}
			}
		}

		/// <summary>
		/// The _proxy connection pool
		/// </summary>
		[ThreadStatic] private static ProxyConnectionPool _proxyConnectionPool;

		/// <summary>
		/// Whenever the connection pool is automatically enabled
		/// </summary>
		/// <value><c>true</c> if [enable connection pool]; otherwise, <c>false</c>.</value>
		public static bool EnableConnectionPool
		{
			get { return _enableConnectionPool; }
			set { _enableConnectionPool = value; }
		}

		#region WCF Extension
		/// <summary>
		/// Class ProxyConnectionPoolExtension.
		/// </summary>
		private class ProxyConnectionPoolExtension : IExtension<OperationContext>
		{
			/// <summary>
			/// The _pool
			/// </summary>
			private readonly ProxyConnectionPool _pool = new ProxyConnectionPool();

			/// <summary>
			/// Gets the current.
			/// </summary>
			/// <value>The current.</value>
			public static ProxyConnectionPoolExtension Current
			{
				get
				{
					if (OperationContext.Current == null)
						return null;

					ProxyConnectionPoolExtension extension = OperationContext.Current.Extensions.Find<ProxyConnectionPoolExtension>();
					if (extension == null)
					{
						extension = new ProxyConnectionPoolExtension();
						OperationContext.Current.Extensions.Add(extension);
					}
					return extension;
				}
			}

			#region IExtension<long> Members

			/// <summary>
			/// Attaches the specified owner.
			/// </summary>
			/// <param name="owner">The owner.</param>
			public void Attach(OperationContext owner)
			{
				
			}

			/// <summary>
			/// Detaches the specified owner.
			/// </summary>
			/// <param name="owner">The owner.</param>
			public void Detach(OperationContext owner)
			{
				
			}

			#endregion

			/// <summary>
			/// Gets the pool.
			/// </summary>
			/// <value>The pool.</value>
			public ProxyConnectionPool Pool
			{
				get { return _pool; }
			}
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Finalizes an instance of the <see cref="ProxyConnectionPool"/> class.
		/// </summary>
		~ProxyConnectionPool()
		{
			Dispose(false);
		}
		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if ( disposing && EnableConnectionPool )
			{
				DisposeAllConnections();
			}
		}
		#endregion
	}
}
