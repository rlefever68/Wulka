using System;
using System.Net.Sockets;
using System.Threading;

namespace Wulka.Networking
{
    /// <summary>
    /// Handle a single connection (TcpClient and associated Thread).
    /// </summary>
    public class TcpConnectionHandle
    {
        private int _id;   // "Session" ident.
        private Guid _uid; // Another "session" id. Usinf Guid to be more versatile with others applications.
        private Thread _connectionThread;    // Thread that running the TcpClient.
        private TcpClient _connectionClient; // TcpClient.

        /// <summary>
        /// Gets or sets the session ident (default is the ManagedThreadId when ConnectionThread is set).
        /// </summary>
        public int Id
        {
            get { return (_id); }
            set { _id = value; }
        }

        /// <summary>
        /// Gets the uid. Same goal of <see cref="Id"/> but using a Guid type.
        /// </summary>
        /// <value>The uid.</value>
        public Guid Uid
        {
            get { return (_uid); }
        }

        /// <summary>
        /// Thread in which ConnectioClient is running.
        /// </summary>
        public Thread ConnectionThread
        {
            get { return (_connectionThread); }
            set
            {
                _connectionThread = value;
                _id = _connectionThread.ManagedThreadId;
            }
        }

        /// <summary>
        /// TcpClient used to handle communication with remote client.
        /// </summary>
        public TcpClient ConnectionClient
        {
            get { return (_connectionClient); }
            set { _connectionClient = value; }
        }

        /// <summary>
        /// Create an empty TcpConnectionHandle.
        /// </summary>
        public TcpConnectionHandle()
        {
            _uid = Guid.NewGuid();
        }

        /// <summary>
        /// Create a new TcpConnectionHandle.
        /// </summary>
        /// <param name="pTh">Thread in which ConnectionClient is running</param>
        public TcpConnectionHandle(Thread pTh)
        {
            _connectionThread = pTh;
            _id = _connectionThread.ManagedThreadId;
            _uid = Guid.NewGuid();
        }

        /// <summary>
        /// Create a new TcpConnectionHandle.
        /// </summary>
        /// <param name="pTh">Thread in which ConnectionClient is running</param>
        /// <param name="pTc">TcpClient used to handle communication between client end server</param>
        public TcpConnectionHandle(Thread pTh, TcpClient pTc)
        {
            _connectionThread = pTh;
            _connectionClient = pTc;
            _id = _connectionThread.ManagedThreadId;
            _uid = Guid.NewGuid();
        }

        /// <summary>
        /// Create a new TcpConnectionHandle.
        /// </summary>
        /// <param name="pTh">Thread in which ConnectionClient is running</param>
        /// <param name="pTc">TcpClient used to handle communication between client end server</param>
        /// <param name="pId">Ident of a session</param>
        public TcpConnectionHandle(Thread pTh, TcpClient pTc, int pId)
        {
            _connectionThread = pTh;
            _connectionClient = pTc;
            _id = pId;
            _uid = Guid.NewGuid();
        }

    } // End TcpConnectionHandle
}
