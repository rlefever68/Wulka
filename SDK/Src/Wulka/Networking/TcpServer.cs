using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;

namespace Wulka.Networking
{
    /// <summary>
    /// Multithreaded Tcp Server.
    /// </summary>
    public class TcpServer : IDisposable
    { 
        


        #region Properties

        /// <summary>
        /// Buffer size for receiver.
        /// </summary>
        public static int Buffer_size = 2048;

        private int _port;
        private string _host;
        private bool _isRunning = false;

        private bool _useWhitelist;
        private bool _useBlacklist;
        private bool _usePermanentConnection;
        private bool _disposed = false;


        private IPAddress _ip;
        private IPEndPoint _localEndPoint;
        private Thread _listenerThread;
        private TcpListener _tcpListener;

        private List<IPRange> _whitelist;
        private List<IPRange> _blacklist;

        private TcpConnectionHandler _tcpConnectionHandler;
        private object _tag;

        private Exception _LastException = null;

        /// <summary>
        /// Gets the clients connected.
        /// </summary>
        /// <value>The clients connected.</value>
        public TcpConnectionHandler ClientsConnected
        {
            get { return (this._tcpConnectionHandler); }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag
        {
            get { return (this._tag); }
            set { this._tag = value; }
        }

        /// <summary>
        /// Connection will be closed (or not) after a transfer.
        /// (Default: false).
        /// </summary>
        public bool PermanentConnection
        {
            get { return (this._usePermanentConnection); }
            set { this._usePermanentConnection = value; }
        }

        /// <summary>
        /// Address and Port used by the Tcp Server.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return (this._localEndPoint); }
        }

        /// <summary>
        /// State of the server.
        /// </summary>
        public bool IsRunning
        {
            get { return (this._isRunning); }
        }

        /// <summary>
        /// Get or Set the use of the Remote IP Addresses whitelist.
        /// </summary>
        public bool UseWhitelist
        {
            get { return (this._useWhitelist); }
            set { this._useWhitelist = value; }
        }

        /// <summary>
        /// Get or Set the use of the Remote IP Addresses blacklist.
        /// </summary>
        public bool UseBlacklist
        {
            get { return (this._useBlacklist); }
            set { this._useBlacklist = value; }
        }

        /// <summary>
        /// Get last exception raised
        /// </summary>
        public Exception LastException
        {
            get { return (this._LastException); }
        }

        /// <summary>
        /// TCP Port.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Hostname.
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Connection (and thread) is kill after received a packet by default.
        /// </summary>
        public bool UsePermanentConnection
        {
            get { return _usePermanentConnection; }
            set { _usePermanentConnection = value; }
        }

        /// <summary>
        /// IDisposable status variable.
        /// </summary>
        public bool Disposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }

        /// <summary>
        /// Ip Address that server bind to.
        /// </summary>
        public IPAddress Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        /// <summary>
        /// Main thread of Tcp Listener.
        /// </summary>
        public Thread ListenerThread
        {
            get { return _listenerThread; }
            set { _listenerThread = value; }
        }

        /// <summary>
        /// Tcp Listener.
        /// </summary>
        public TcpListener TcpListener
        {
            get { return _tcpListener; }
            set { _tcpListener = value; }
        }

        /// <summary>
        /// Ip Addresses whitelist.
        /// </summary>
        public List<IPRange> Whitelist
        {
            get { return _whitelist; }
            set { _whitelist = value; }
        }

        /// <summary>
        /// Ip Addresses blacklist.
        /// </summary>
        public List<IPRange> Blacklist
        {
            get { return _blacklist; }
            set { _blacklist = value; }
        }

        /// <summary>
        /// Keep a list of all active thread (used with PermanentConnection).
        /// </summary>
        public TcpConnectionHandler TcpConnectionHandler
        {
            get { return _tcpConnectionHandler; }
            set { _tcpConnectionHandler = value; }
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Handler for event MessageReceivedEventHandler
        /// </summary>
        /// <param name="sender">Tcp Server from which is originated the event</param>
        /// <param name="message">Data attached to the event</param>
        public delegate void MessageReceivedEventHandler(TcpServer sender, MessageData message);

        /// <summary>
        /// Handler for event NewConnectionEventHandler
        /// </summary>
        /// <param name="sender">Tcp Server from which is originated the event</param>
        /// <param name="message">Data attached to the event</param>
        public delegate void NewConnectionEventHandler(TcpServer sender, MessageData message);

        /// <summary>
        /// Handler for event CloseConnectionEventHandler
        /// </summary>
        /// <param name="sender">Tcp Server from which is originated the event</param>
        /// <param name="message">Data attached to the event</param>
        public delegate void CloseConnectionEventHandler(TcpServer sender, MessageData message);

        #endregion

        #region Events

        /// <summary>
        /// Event occurred when a new messsge was received.
        /// </summary>
        public event MessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// Event occurred when a new connection is establish.
        /// </summary>
        public event NewConnectionEventHandler NewConnection;

        /// <summary>
        /// Event occurred when a connection is closed.
        /// </summary>
        public event CloseConnectionEventHandler CloseConnection;

        #endregion

        #region Constructor(s) & Destructor

        /// <summary>
        /// Create a new Tcp Server.
        /// </summary>
        /// <param name="pFrom">Valid IP adress</param>
        /// <param name="pPort">Port Number</param>
        public TcpServer(IPAddress ip, int pPort)
        {
            Logger = LogManager.GetCurrentClassLogger();
            _ip = ip;
            _port = pPort;

            // Resolve the hostname via the Dns.
            this.Resolve();

            this.Initialize();
        }

        public Logger Logger { get; set; }

        /// <summary>
        /// Create a new Tcp Server.
        /// </summary>
        /// <param name="pPort">Port number</param>
        public TcpServer(int pPort)
        {
            Logger = LogManager.GetCurrentClassLogger();
            _ip = IPAddress.Any;
            _port = pPort;
            _localEndPoint = new IPEndPoint(_ip, _port);

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="pPort">The p port.</param>
        public TcpServer(string host, int pPort)
        {
            Logger = LogManager.GetCurrentClassLogger();
            _host = host;
            _port = pPort;

            // Resolve the hostname via the Dns.
            this.Resolve();

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public TcpServer(IPEndPoint endpoint)
        {
            Logger = LogManager.GetCurrentClassLogger();
            _localEndPoint = endpoint;
            _ip = endpoint.Address;
            _port = endpoint.Port;

            this.Initialize();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TcpServer"/> is reclaimed by garbage collection.
        /// </summary>
        ~TcpServer()
        {
            Dispose(false);
        }

        #endregion Constructor(s) & Destructor

        #region Methods

        #region Statics

#if(TEST_ON)
    // Use #if here as event delegate are not supported where [Conditional("TEST_ON")] is used.
    private const int nbThread = 10;
    private static ManualResetEvent[] _doneEvents = new ManualResetEvent[nbThread];
    public static void TestMe()
    {
      TcpServer tcpServer = new TcpServer(3222);
      tcpServer.MessageReceived += new MessageReceivedEventHandler(TestMe_MessageReceived);
      tcpServer.Start();

      // ThreadPool must be used in MTAThread.
      Thread mtaThread = new Thread(new ParameterizedThreadStart(TestMe_StartTreads));
      mtaThread.SetApartmentState(ApartmentState.MTA);
      mtaThread.IsBackground = true;
      mtaThread.Start(tcpServer);
      
    }

    // Launch the thread inside a threadPool.
    private static void TestMe_StartTreads(object server) 
    {
      TcpServer tcpServer = (TcpServer)server;

      for(int i = 0; i < nbThread; i++) {
        TcpServer._doneEvents[i] = new ManualResetEvent(false);
        ThreadPool.QueueUserWorkItem(TestMe_SendMessage, i);
      }

      WaitHandle.WaitAll(TcpServer._doneEvents);

      tcpServer.Stop();
    }

    // Send a message to the server.
    private static void TestMe_SendMessage(Object threadContext)
    {
      int threadIndex = (int)threadContext; 
      Random rd = new Random();
      int waitTime = rd.Next(300);
      
      Thread.Sleep(waitTime);

      UTF8Encoding encoder = new UTF8Encoding();
      byte[] buffer = encoder.GetBytes(string.Format("Hello Server! (waited : {0} ms)", waitTime));
      TcpClient client = new TcpClient();
      IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3222);
      
      client.Connect(serverEndPoint);
      
      NetworkStream clientStream = client.GetStream();
      clientStream.Write(buffer, 0, buffer.Length);
      clientStream.Flush();
      clientStream.Close();

      TcpServer._doneEvents[threadIndex].Set();
    }

    // Dump the received message to the console.
    private static void TestMe_MessageReceived(MessageData message)
    {
      Console.WriteLine(string.Format("Message Received (TCP): {0} bytes length = \"{1}\" from {2}",
                        message.MessageLength, Encoding.UTF8.GetString(message.Message), message.RemoteEndPoint.ToString()));
    }

#endif
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
                    try
                    {
                        // Get the Hostname given the IP Address.
                        ipHostInfo = Dns.GetHostEntry(_ip);
                        _host = ipHostInfo.HostName;
                    }
                    catch
                    {
                        _host = "";
                    }
                }
                else
                {
                    // Get the IP Address given the Hostname.
                    ipHostInfo = Dns.GetHostEntry(_host);
                    _ip = ipHostInfo.AddressList[0];
                }
                _localEndPoint = new IPEndPoint(_ip, _port);
            }
            catch(Exception ex)
            {
                Logger.Error("Exception raised resolving the dns host name or ip address.", ex);
            }
        }

        /// <summary>
        /// Start the Tcp Server.
        /// </summary>
        public void Start()
        {
            if (!_isRunning)
            {
                _listenerThread.Start();
            }
        }

        /// <summary>
        /// Stop the Tcp Server.
        /// </summary>
        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;

                if (TcpConnectionHandler.Count > 0)
                {
                    for (int i = 0; i < TcpConnectionHandler.Count; i++)
                    {
                        try
                        {
                            // Stop tcpclient.
                            TcpConnectionHandler[i].ConnectionClient.Client.Shutdown(SocketShutdown.Both);
                            TcpConnectionHandler[i].ConnectionClient.Client.Close();
                            TcpConnectionHandler[i].ConnectionClient.Close();
                            // Kill the thread.
                            TcpConnectionHandler[i].ConnectionThread.Join();
                        }
                        catch
                        {
                        }
                    }
                }

                // Stop the listener.
                _tcpListener.Stop();

                if (_listenerThread.IsAlive)
                {
                    // Kill the listener thread.
                    _listenerThread.Join();
                }

                TcpConnectionHandler.Clear();
            }
        }


        /// <summary>
        /// Add an Ip address to the whitelist.
        /// </summary>
        /// <param name="pIp">Ip to add</param>
        public void AddToWhitelist(IPAddress pIp)
        {
            IPRange ipr = new IPRange(pIp, pIp);

            _whitelist.Add(ipr);
        }

        /// <summary>
        /// Add an Ip addresses range to the whitelist.
        /// </summary>
        /// <param name="pIp1">Start Ip to add</param>
        /// <param name="pIp2">End Ip to add</param>
        public void AddToWhitelist(IPAddress pIp1, IPAddress pIp2)
        {
            IPRange ipr = new IPRange(pIp1, pIp2);

            _whitelist.Add(ipr);
        }

        /// <summary>
        /// Remove an Ip address from the whitelist.
        /// </summary>
        /// <param name="pIp">Ip to remove</param>
        public void RemoveFromWhitelist(IPAddress pIp)
        {
            IPRange ipr = new IPRange(pIp, pIp);
            
            _whitelist.Remove(ipr);
        }

        /// <summary>
        /// Remove an Ip address range from the whitelist.
        /// </summary>
        /// <param name="pIp1">Start Ip to add</param>
        /// <param name="pIp2">End Ip to add</param>
        public void RemoveFromWhitelist(IPAddress pIp1, IPAddress pIp2)
        {
            IPRange ipr = new IPRange(pIp1, pIp2);

            _whitelist.Remove(ipr);
        }


        /// <summary>
        /// Add an Ip address to the blacklist.
        /// </summary>
        /// <param name="pIp">Ip to add</param>
        public void AddToBlacklist(IPAddress pIp)
        {
            IPRange ipr = new IPRange(pIp, pIp);

            _blacklist.Add(ipr);
        }

        /// <summary>
        /// Add an Ip addresses range to the blacklist.
        /// </summary>
        /// <param name="pIp1">Start Ip to add</param>
        /// <param name="pIp2">End Ip to add</param>
        public void AddToBlacklist(IPAddress pIp1, IPAddress pIp2)
        {
            IPRange ipr = new IPRange(pIp1, pIp2);

            _blacklist.Add(ipr);
        }

        /// <summary>
        /// Remove an Ip address from the blacklist.
        /// </summary>
        /// <param name="pIp">Ip to remove</param>
        public void RemoveFromBlacklist(IPAddress pIp)
        {
            IPRange ipr = new IPRange(pIp, pIp);

            _blacklist.Remove(ipr);
        }

        /// <summary>
        /// Remove an Ip address range from the blacklist.
        /// </summary>
        /// <param name="pIp1">Start Ip to remove</param>
        /// <param name="pIp2">End Ip to remove</param>
        public void RemoveFromBlacklist(IPAddress pIp1, IPAddress pIp2)
        {
            IPRange ipr = new IPRange(pIp1, pIp2);

            _blacklist.Remove(ipr);
        }

        /// <summary>
        /// Close a connection.
        /// </summary>
        /// <param name="pClient">Connection to close</param>
        public void KillClient(TcpClient pClient)
        {
            try
            {
                TcpConnectionHandle cnxHandle = TcpConnectionHandler.GetConnectionHandle(pClient);

                if (cnxHandle != null)
                {
                    OnCloseMessageReceived(new MessageData(null, 0, pClient.Client.RemoteEndPoint, cnxHandle));
                    cnxHandle.ConnectionClient.Client.Shutdown(SocketShutdown.Both);
                    cnxHandle.ConnectionClient.Client.Close();
                    cnxHandle.ConnectionClient.Close();
                    // Kill the thread.
                    cnxHandle.ConnectionThread.Join(50);
                    lock (TcpConnectionHandler)
                    {
                        TcpConnectionHandler.Remove(cnxHandle);
                    }
                    cnxHandle = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception raised killing tcpclient connection client.", ex);
            }
        }

        /// <summary>
        /// Close a connection.
        /// </summary>
        /// <param name="cnxHandle">Connection to close</param>
        public void KillClient(TcpConnectionHandle cnxHandle)
        {
            try
            {
                if (cnxHandle != null)
                {
                    OnCloseMessageReceived(new MessageData(null, 0, cnxHandle.ConnectionClient.Client.RemoteEndPoint, cnxHandle));                   
                    // Stop tcpclient.
                    cnxHandle.ConnectionClient.Client.Shutdown(SocketShutdown.Both);
                    cnxHandle.ConnectionClient.Client.Close();
                    cnxHandle.ConnectionClient.Close();
                    // Kill the thread.
                    cnxHandle.ConnectionThread.Join(50);
                    lock (TcpConnectionHandler)
                    {
                        TcpConnectionHandler.Remove(cnxHandle);
                    }
                    cnxHandle = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception raised killing tcpconnectionhandle.", ex);
            }
        }

        /// <summary>
        /// Return if a particular IP address is in the whitelist.
        /// </summary>
        /// <param name="pIp">IP address to check</param>
        /// <returns>Whitelisted or not</returns>
        public bool IsWhitelisted(IPAddress pIp)
        {
            for (int i = 0; i < _whitelist.Count; i++)
            {
                if (_whitelist[i].Contains(pIp) == 0)
                {
                    return (true);
                }
            }

            return (false);
        }

        /// <summary>
        /// Return if a particular IP address is in the blacklist.
        /// </summary>
        /// <param name="pIp">IP address to check</param>
        /// <returns>Whitelisted or not</returns>
        public bool IsBlacklisted(IPAddress pIp)
        {
            for (int i = 0; i < _blacklist.Count; i++)
            {
                if (_blacklist[i].Contains(pIp) == 0)
                {
                    return (true);
                }
            }

            return (false);
        }

        /// <summary>
        /// Send data.
        /// </summary>
        /// <param name="pConnectioHandle">Network connection to use</param>
        /// <param name="pMsg">Data to send</param>
        /// <returns>Send status</returns>
        public bool Send(TcpConnectionHandle pConnectioHandle, byte[] pMsg)
        {
            try
            {
                if (pConnectioHandle != null)
                {
                    TcpClient tcpClient = pConnectioHandle.ConnectionClient;
                    if(tcpClient != null & tcpClient.Connected)
                    {
                        NetworkStream clientStream = tcpClient.GetStream();

                        if (clientStream != null)
                        {
                            clientStream.Write(pMsg, 0, pMsg.Length);
                            clientStream.Flush();
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                return (false);
            }
            catch(Exception ex)
            {
                Logger.Error("Exception raised sending message.", ex);
                return false;
            }
        }

        /// <summary>
        /// Send data.
        /// </summary>
        /// <param name="pConnectioHandle">Network connection to use</param>
        /// <param name="pMsg">Single byte to send</param>
        /// <returns>Send Status</returns>
        public bool Send(TcpConnectionHandle pConnectioHandle, byte pMsg)
        {
            return (this.Send(pConnectioHandle, new byte[] { pMsg }));
        }

        /// <summary>
        /// Send data.
        /// </summary>
        /// <param name="pConnectioHandle">Network connection to use</param>
        /// <param name="pMsg">Data to send</param>
        /// <returns>Send status</returns>
        public bool Send(TcpConnectionHandle pConnectioHandle, string pMsg)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] buffer = encoder.GetBytes(pMsg);

            return (this.Send(pConnectioHandle, buffer));
        }

        #endregion

        #region Privates

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected void Initialize()
        {
            // Create the listener for selected endPoint interface (any by default).
            _tcpListener = new TcpListener(_localEndPoint);

            // Create the main thread of the listener.
            _listenerThread = new Thread(new ThreadStart(ClientsListener));
            _listenerThread.Name = "TcpListener";

            // Create the whitelist.
            _whitelist = new List<IPRange>();

            // Create the blacklist.
            _blacklist = new List<IPRange>();

            // Create the connection list.
            TcpConnectionHandler = new TcpConnectionHandler();

            _usePermanentConnection = false;
        }

        /// <summary>
        /// Event caller.
        /// </summary>
        /// <param name="message">Raw message received from network</param>
        protected void OnMessageReceived(MessageData message)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, message);
            }
        }

        /// <summary>
        /// Event caller.
        /// </summary>
        /// <param name="message">Raw message received from network</param>
        protected void OnNewConnection(MessageData message)
        {
            try
            {
                if (NewConnection != null)
                {
                    NewConnection(this, message);
                    lock (TcpConnectionHandler)
                    {
                        TcpConnectionHandler.Add(message.ConnectionHandle);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Error("Exception raised creating new connection.", ex);
            }
        }

        /// <summary>
        /// Event caller.
        /// </summary>
        /// <param name="message">Raw message received from network</param>
        protected void OnCloseMessageReceived(MessageData message)
        {
            try
            {
                if (CloseConnection != null)
                {
                    CloseConnection(this, message);
                    lock (TcpConnectionHandler)
                    {
                        TcpConnectionHandler.Remove(message.ConnectionHandle);
                    }
                }
            }
            catch(Exception)
            {
                Logger.Error("Exception raised closing connection handle and removing from connection list.");
            }
        }

        /// <summary>
        /// Thread that wait for a connection.
        /// </summary>
        protected void ClientsListener()
        {
            try
            {
                TcpConnectionHandle connectionHandle = null;

                try
                {
                    _tcpListener.Start();
                }
                catch (Exception ex)
                {
                    _LastException = ex;
                    _isRunning = false;
                    return;
                }

                _isRunning = true;

                while (true)
                {
                    TcpClient client;
                    try
                    {
                        // Blocks until a client has connected to the server.
                        client = _tcpListener.AcceptTcpClient();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Exception raised accepting tcp client connection.", ex);
                        break;
                    }

                    // If the whitelist is used only whitelisted IP address is allowed.
                    if (_useWhitelist && !this.IsWhitelisted(((IPEndPoint) client.Client.RemoteEndPoint).Address))
                    {
                        this.KillClient(client);
                        continue;
                    }

                    // If the blacklist is used blacklisted IP address is rejected.
                    if (_useBlacklist && this.IsBlacklisted(((IPEndPoint) client.Client.RemoteEndPoint).Address))
                    {
                        this.KillClient(client);
                        continue;
                    }

                    // Create a thread to handle communication with connected client.
                    Thread clientThread = new Thread(new ParameterizedThreadStart(ReceiveDataFromClient));
                    clientThread.Name = "TcpReceiver";

                    connectionHandle = new TcpConnectionHandle(clientThread, client);

                    //if (_usePermanentConnection)
                    //{
                    //    // Keep a track of active thread.
                    //    _connectionList.Add(connectionHandle);
                    //}

                    byte[] tmp = BitConverter.GetBytes(clientThread.ManagedThreadId);
                    OnNewConnection(new MessageData(tmp, tmp.Length, client.Client.RemoteEndPoint, connectionHandle));

                    clientThread.Start(connectionHandle);
                    client = null;
                    tmp = null;
                }
            }
            catch(Exception ex)
            {
                Logger.Error("Exception raised in client listener.", ex);
            }
        }

        /// <summary>
        /// Handle communication with remote client.
        /// Called by ClientsListener thread.
        /// </summary>
        /// <param name="handle">TcpClient Object</param>
        protected virtual void ReceiveDataFromClient(object handle)
        {
            byte[] buffer = new byte[Buffer_size];
            byte[] message;
            int bytesRead;
            MessageData msgData;
            TcpConnectionHandle connectionHandle = (TcpConnectionHandle)handle;

            // Create the tcpClient object to handle communication with the remote client.
            TcpClient tcpClient = connectionHandle.ConnectionClient;
            NetworkStream clientStream = tcpClient.GetStream();


            //NetworkStream stream = client.GetStream();
            //Int32 i;
            //Byte[] buffer;
            //String result = string.Empty;
            //do {
            //  buffer = new Byte[128];
            //  i = stream.Read(buffer, 0, buffer.Length);
            //  result += ASCIIEncoding.ASCII.GetString(buffer);
            //} while(stream.DataAvailable);



            while (true)
            {
                bytesRead = 0;

                try
                {
                    // Blocks until a client sends a message.
                    bytesRead = clientStream.Read(buffer, 0, Buffer_size);
                }
                catch
                {
                    // A socket error has occurred.
                    this.KillClient(tcpClient);
                    break;
                }

                if (bytesRead == 0)
                {
                    // The client has disconnected from the server.
                    this.KillClient(tcpClient);
                    break;
                }

                // NOTE : Single data buffer is supported here, better work with a second buffer (stringbuilder ?)
                //        to handle data longuest than 4Kb...

                // Copy message data to correctly sized array.
                message = new byte[bytesRead];
                Array.Copy(buffer, message, bytesRead);
                msgData = new MessageData(message, bytesRead, tcpClient.Client.RemoteEndPoint, connectionHandle);

                // Send event to dispatch the new message.
                this.OnMessageReceived(msgData);
                msgData = null;
            }

            if (!_usePermanentConnection)
            {
                // Close the connection, exit the thread.
                this.KillClient(tcpClient);
            }
        }


        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.Stop();
                    _disposed = true;
                }
            }
        }

        #endregion
    } 
}
