using System;
using System.Net;

namespace Wulka.Networking
{
    /// <summary>
    /// AsyncTcpClientEventArgs.
    /// </summary>
    public class AsyncTcpClientEventArgs : EventArgs
    {

        #region Members

        #region Privates

        private IPEndPoint _remoteEndPoint;
        private SocketState _status;
        private Exception _exception;
        private byte[] _data;
        private int _dataLength;

        #endregion


        #region Publics

        // Publics members goes here.

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Remote Endpoint.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return (this._remoteEndPoint); }
        }

        /// <summary>
        /// Connection status.
        /// </summary>
        public SocketState Status
        {
            get { return (this._status); }
        }

        /// <summary>
        /// Exception when Status == Error.
        /// </summary>
        public Exception Exception
        {
            get { return (this._exception); }
            internal set { this._exception = value; }
        }

        /// <summary>
        /// Data received.
        /// </summary>
        public byte[] Data
        {
            get { return (this._data); }
            internal set { this._data = value; }
        }

        /// <summary>
        /// Size of data received.
        /// </summary>
        public int Length
        {
            get { return (this._dataLength); }
            internal set { this._dataLength = value; }
        }

        #endregion

        #region Events

        // Events and delegates goes here.

        #endregion

        #region Constructor(s) & Destructor

        /// <summary>
        /// Create a new AsyncTcpClientEventArgs (used in Connected event).
        /// </summary>
        /// <param name="remoteEndPoint">Remote host</param>
        /// <param name="status">Status</param>
        public AsyncTcpClientEventArgs(IPEndPoint remoteEndPoint, SocketState status)
        {
            this._remoteEndPoint = remoteEndPoint;
            this._status = status;
        }

        /// <summary>
        /// Create a new AsyncTcpClientEventArgs (used when data are received).
        /// </summary>
        /// <param name="remoteEndPoint">Remote host</param>
        /// <param name="length">Size of received data</param>
        /// <param name="data">Received data</param>
        public AsyncTcpClientEventArgs(IPEndPoint remoteEndPoint, int length, byte[] data)
        {
            this._remoteEndPoint = remoteEndPoint;
            this._status = SocketState.Listening;
            this._dataLength = length;
            this._data = data;
        }

        /// <summary>
        /// Create a new AsyncTcpClientEventArgs (used when exception occurred).
        /// </summary>
        /// <param name="remoteEndPoint">Remote host</param>
        /// <param name="ex">Exception</param>
        public AsyncTcpClientEventArgs(IPEndPoint remoteEndPoint, Exception ex)
        {
            this._remoteEndPoint = remoteEndPoint;
            this._status = SocketState.Error;
            this._exception = ex;
        }

        #endregion

        #region Methods

        #region Statics

        // Statics methods goes here.

        #endregion

        #region Publics

        // Publics methods goes here.

        #endregion

        #region Privates

        // Privates methods goes here.

        #endregion

        #endregion

    } 
}
