using System;
using System.Runtime.Serialization;

namespace Wulka.Networking.Wcf
{
    public class DiscoProxyException : Exception
    {
        public DiscoProxyException(SerializationInfo info, StreamingContext ctx)
            :base(info,ctx)
        { }


        public DiscoProxyException(string msg)
            :base(msg)
        { }

        public DiscoProxyException()
        { }

        public DiscoProxyException(string msg, Exception e)
            :base(msg,e)
        {}

    }
}