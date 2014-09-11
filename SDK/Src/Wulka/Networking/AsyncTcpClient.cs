using System;
using System.Net;
using System.Net.Sockets;
using Wulka.Logging;

namespace Wulka.Networking
{
    #region Enums

    /// <summary>
    /// Represent a state in socket
    /// </summary>
    public enum SocketState
    {
        /// <summary>
        /// No Socket State o socket is not initialized yet
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Socket is in Listen mode
        /// </summary>
        Listening,

        /// <summary>
        /// Resolving host address
        /// </summary>
        Resolving,

        /// <summary>
        /// connecting to host
        /// </summary>
        Connecting,

        /// <summary>
        /// connected to host
        /// </summary>
        Connected,

        /// <summary>
        /// closing connection
        /// </summary>
        Closing,

        /// <summary>
        /// connection closed
        /// </summary>
        Closed,

        /// <summary>
        /// Error Occur
        /// </summary>
        Error = -1
    }

    #endregion

    /// <summary>
    /// Asynchrone Tcp Client.
    /// </summary>
    /// <example>
    /// <code>
    /// AsyncTcpClient client = new AsyncTcpClient(&quot;localhost&quot;, 28500);
    /// client.Received += new EventHandler&lt;AsyncTcpClientEventArgs&gt;(client_Received);
    /// client.Connected += new EventHandler&lt;AsyncTcpClientEventArgs&gt;(client_Connected);
    /// client.Connect(); // Connect is Async too, so a Send() right after will certainly failed...
    /// ...
    /// client.Send(Encoding.ASCII.GetBytes(&quot;Test Message on TCP Connector&quot;));
    ///  
    /// 
    /// private void client_Connected(object sender, AsyncTcpClientEventArgs e)
    /// {
    ///   // Client is now connected, data could be received and sended.
    /// }
    ///
    ///
    /// private void client_Received(object sender, AsyncTcpClientEventArgs e)
    /// {
    ///   // Do something with e.Data.
    /// }
    /// </code>
    /// </example>
    public class AsyncTcpClient
    {

        #region Members

        #region Privates

        
        private Socket _client;
        private IPEndPoint _endpoint;
        private IPAddress _ip;
        private string _host;
        private int _port;
        private int _size = 2048;
        private byte[] _data = new byte[2048];
        //private int _receiveTimeout;
        private bool _stopping = false;
        
        #endregion


        #region Publics

        // Publics members goes here.

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Set the value of the timeout when sending data to remote client.
        /// </summary>
        /// <remarks>Must be set after the connection is connected</remarks>
        public int SendTimeout
        {
            get { return ((_client != null && _client.Connected) ? _client.SendTimeout : 0); }
            set { _client.SendTimeout = value; }
        }


        /// <summary>
        /// Gets or Set the value of the timeout when receiving data from remote client.
        /// </summary>
        /// <remarks>Must be set after the connection is connected</remarks>
        public int ReceiveTimeout
        {
            get { return ((_client != null && _client.Connected) ? _client.ReceiveTimeout : 0); }
            set { _client.ReceiveTimeout = value; }
        }

        /// <summary>
        /// Gets if the socket is connected to remote host.
        /// </summary>
        public bool IsConnected
        {
            get { return (_client != null && _client.Connected); }
        }

        /// <summary>
        /// Client socket
        /// </summary>
        public Socket Client
        {
            get
            {
                return _client;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Socket is Connected.
        /// </summary>
        public event EventHandler<AsyncTcpClientEventArgs> Connected;

        /// <summary>
        /// Occurs when the Socket is Connected.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Occurs when the Socket Received data from remote endpoint.
        /// </summary>
        public event EventHandler<AsyncTcpClientEventArgs> Received;

        /// <summary>
        /// Occurs when the Socket Send data to remote endpoint.
        /// </summary>
        public event EventHandler<AsyncTcpClientEventArgs> Sended;

        /// <summary>
        /// Occurs on any error in Socket.
        /// </summary>
        public event EventHandler<AsyncTcpClientEventArgs> Error;


        #endregion

        #region Constructor(s) & Destructor

        /// <summary>
        /// Create a new AsyncTcpClient instance.
        /// </summary>
        /// <param name="host">Remote host</param>
        /// <param name="port">Remote port</param>
        /// <example>
        /// <code>
        /// AsyncTcpClient client = new AsyncTcpClient(&quot;localhost&quot;, 28500);
        /// client.Received += new EventHandler&lt;AsyncTcpClientEventArgs&gt;(client_Received);
        /// client.Connected += new EventHandler&lt;AsyncTcpClientEventArgs&gt;(client_Connected);
        /// client.Connect(); // Connect is Async too, so a Send() right after will certainly failed...
        /// ...
        /// client.Send(Encoding.ASCII.GetBytes(&quot;Test Message on TCP Connector&quot;));
        ///  
        /// 
        /// private void client_Connected(object sender, AsyncTcpClientEventArgs e)
        /// {
        ///   // Client is now connected, data could be received and sended.
        /// }
        ///
        ///
        /// private void client_Received(object sender, AsyncTcpClientEventArgs e)
        /// {
        ///   // Do something with e.Data.
        /// }
        /// </code>
        /// </example>
        public AsyncTcpClient(string host, int port)
        {
            _host = host;
            _port = port;

            IPAddress address;
            if (IPAddress.TryParse(_host, out address))
            {
                _ip = IPAddress.Parse(_host);
                _endpoint = new IPEndPoint(_ip, _port);
            }
            else
            {
                // Resolve the hostname via the Dns.
                Resolve();
            }
        }

        /// <summary>
        /// Create a new AsyncTcpClient instance.
        /// </summary>
        /// <param name="ip">Remote IP address</param>
        /// <param name="port">Remote port</param>
        /// <example>
        /// <code>
        /// AsyncTcpClient client = new AsyncTcpClient(&quot;localhost&quot;, 28500);
        /// client.Received += new EventHandler&lt;AsyncTcpClientEventArgs&gt;(client_Received);
        /// client.Connected += new EventHandler&lt;AsyncTcpClientEventArgs&gt;(client_Connected);
        /// client.Connect(); // Connect is Async too, so a Send() right after will certainly failed...
        /// ...
        /// client.Send(Encoding.ASCII.GetBytes(&quot;Test Message on TCP Connector&quot;));
        ///  
        /// 
        /// private void client_Connected(object sender, AsyncTcpClientEventArgs e)
        /// {
        ///   // Client is now connected, data could be received and sended.
        /// }
        ///
        ///
        /// private void client_Received(object sender, AsyncTcpClientEventArgs e)
        /// {
        ///   // Do something with e.Data.
        /// }
        /// </code>
        /// </example>
        public AsyncTcpClient(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;

            // Resolve the hostname via the Dns.
            Resolve();
        }

        /// <summary>
        /// Create a new AsyncTcpClient instance.
        /// </summary>
        /// <param name="endpoint">Remote Endpoint</param>
        /// <example>
        /// <code>
        /// AsyncTcpClient client = new AsyncTcpClient(&quot;localhost&quot;, 28500);
        /// client.Received += new EventHandler&lt;AsyncTcpClientEventArgs&gt;(client_Received);
        /// client.Connected += new EventHandler&lt;AsyncTcpClientEventArgs&gt;(client_Connected);
        /// client.Connect(); // Connect is Async too, so a Send() right after will certainly failed...
        /// ...
        /// client.Send(Encoding.ASCII.GetBytes(&quot;Test Message on TCP Connector&quot;));
        ///  
        /// 
        /// private void client_Connected(object sender, AsyncTcpClientEventArgs e)
        /// {
        ///   // Client is now connected, data could be received and sended.
        /// }
        ///
        ///
        /// private void client_Received(object sender, AsyncTcpClientEventArgs e)
        /// {
        ///   // Do something with e.Data.
        /// }
        /// </code>
        /// </example>
        public AsyncTcpClient(IPEndPoint endpoint)
        {
            _endpoint = endpoint;
            _ip = endpoint.Address;
            _port = endpoint.Port;

            // Resolve the hostname via the Dns.
            Resolve();
        }

        #endregion

        #region Methods

        #region Statics

        // Statics methods goes here.

        #endregion

        #region Publics

        /// <summary>
        /// Resolves the DNS host name or IP address to an IPHostEntry instance.
        /// </summary>
        public void Resolve()
        {
            try
            {
                IPHostEntry ipHostInfo;

                if (String.IsNullOrEmpty(_host))
                {
                    // Get the Hostname given the IP Address.
                    ipHostInfo = Dns.GetHostEntry(_ip);
                    _host = ipHostInfo.HostName;

                }
                else
                {
                    // Get the IP Address given the Hostname.
                    ipHostInfo = Dns.GetHostEntry(_host);
                    _ip = ipHostInfo.AddressList[0];
                }

                _endpoint = new IPEndPoint(_ip, _port);
            }
            catch (Exception ex)
            {
                FxLog<AsyncTcpClient>
                    .Error("Exception raised trying to resolve the DNS host name or IP address to an IPHostEntry instance.", ex);
            }
        }




        /// <summary>
        /// Connect to the remote host.<br/>
        /// SendTimeout is set to 3000ms by default.
        /// </summary>
        /// <remarks>
        /// As the connection is made asynchronously a call to <see cref="Send"/> may failed if this call is
        /// made right after the Connect().<br />
        /// </remarks>
        public void Connect()
        {
            try
            {
                Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                newsock.SendTimeout = 3000;
                newsock.BeginConnect(_endpoint, new AsyncCallback(EndConnect), newsock);
            }
            catch (Exception ex)
            {
                FxLog<AsyncTcpClient>
                .Error("Exception raised trying to connect to the remote host.", ex);
            }
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public void Close()
        {
            try
            {
                if (_client != null && _client.Connected)
                {
                    // Disconnected Event are raised only if a "real" disconnection occurred (not when Stop() is called).
                    _stopping = true;
                    _client.Shutdown(SocketShutdown.Both);
                    _client.Disconnect(false);
                    _client.Close();
                    _client = null;
                    _stopping = false;
                }
            }
            catch (Exception ex)
            {
                FxLog<AsyncTcpClient>
                    .Error("Exception raised trying to close the connection.", ex);
            }
        }

        /// <summary>
        /// Send data to the remote host (async method).
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            try
            {
                if (_client != null && _client.Connected)
                {
                    _client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), _client);
                }
            }
            catch (Exception ex)
            {
                if (Error != null)
                {
                    Error(this, new AsyncTcpClientEventArgs(null, ex.GetBaseException()));
                }
                FxLog<AsyncTcpClient>
                    .Error("Exception raised trying to send message to the remote host.", ex);
            }
        }

        #endregion

        #region Privates

        /// <summary>
        /// Finish the connection initiate via Connect().
        /// </summary>
        /// <param name="iar"></param>
        private void EndConnect(IAsyncResult iar)
        {
            _client = (Socket)iar.AsyncState;

            try
            {
                _client.EndConnect(iar);
                if (Connected != null)
                {
                    Connected(this, new AsyncTcpClientEventArgs((IPEndPoint)_client.RemoteEndPoint, SocketState.Connected));
                }

                if (_client != null)
                {
                    // Start the listener thread.
                    _client.BeginReceive(_data, 0, _size, SocketFlags.None, new AsyncCallback(ReceiveData), _client);
                }
            }
            catch (Exception ex)
            {
                if (Error != null)
                {
                    Error(this, new AsyncTcpClientEventArgs(null, ex.GetBaseException()));
                }
            }
        }

        /// <summary>
        /// Receive data from the remote connection.
        /// </summary>
        /// <param name="iar"></param>
        private void ReceiveData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            byte[] data = null;
            byte[] message = null;
            int recv = 0;

            // Receive data and save it in a temporary variable.
            try
            {
                recv = remote.EndReceive(iar);
                data = _data;

                // 0 bytes received : remote client is disconnected.
                if (recv == 0)
                {
                    if ((Disconnected != null) && (!_stopping))
                    {
                        Disconnected(this, null);
                    }

                    return;
                }

                message = new byte[recv];
                Array.Copy(data, message, recv);

                // Handle next data packet.
                _client.BeginReceive(_data, 0, _size, SocketFlags.None, new AsyncCallback(ReceiveData), _client);

                if (Received != null)
                {
                    Received(this, new AsyncTcpClientEventArgs((IPEndPoint)_client.RemoteEndPoint, recv, message));
                }

                // TODO remove?
                data = null;
                message = null;
            }
            catch
            {
                if (remote == null || !remote.Connected)
                {
                    if ((Disconnected != null) && (!_stopping))
                    {
                        Disconnected(this, null);
                    }
                }
            }
            data = null;
            message = null;
        }

        /// <summary>
        /// Finish the send of data initiate in Send().
        /// </summary>
        /// <param name="iar"></param>
        private void SendData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            try
            {
                int sent = remote.EndSend(iar);

                if (Sended != null)
                {
                    Sended(this, new AsyncTcpClientEventArgs((IPEndPoint)_client.RemoteEndPoint, sent, null));
                }
            }
            catch (Exception ex)
            {
                if (Sended != null && _client != null)
                {
                    try
                    {
                        Sended(this, new AsyncTcpClientEventArgs((IPEndPoint)_client.RemoteEndPoint, ex));
                    }
                    catch
                    {
                    }
                }
            }

        }

        #endregion

        #endregion

    } // End AsyncTcpClient
}
