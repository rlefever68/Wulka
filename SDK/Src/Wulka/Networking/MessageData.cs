using System.Net;

namespace Wulka.Networking
{
    /// <summary>
    /// Class for exchanging data from the server (TCP or UDP) and the client
    /// application via event.
    /// </summary>
    public class MessageData
    {
        private TcpConnectionHandle _connectionHandle;

        /// <summary>
        /// Remote IP Address.
        /// </summary>
        public EndPoint RemoteEndPoint;

        /// <summary>
        /// Length of received data.
        /// </summary>
        public int MessageLength;

        /// <summary>
        /// Message data.
        /// </summary>
        public byte[] Message;

        /// <summary>
        /// TCP Connection handle.
        /// </summary>
        public TcpConnectionHandle ConnectionHandle
        {
            get { return (_connectionHandle); }
        }

        /// <summary>
        /// Create a new MessageData object.
        /// </summary>
        /// <param name="pMsg">Message content</param>
        /// <param name="pMsgLength">Message length</param>
        /// <param name="pRemoteEndPoint">Remote endpoint</param>
        /// <param name="pCnxHandle">Handle of the TCP connection</param>
        public MessageData(byte[] pMsg, int pMsgLength, EndPoint pRemoteEndPoint, TcpConnectionHandle pCnxHandle)
        {
            Message = pMsg;
            MessageLength = pMsgLength;
            RemoteEndPoint = pRemoteEndPoint;
            _connectionHandle = pCnxHandle;
        }

        /// <summary>
        /// Create a new MessageData object.
        /// </summary>
        public MessageData()
        {
        }

    } // end MessageData
}
