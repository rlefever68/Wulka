using System;

namespace Wulka.Exceptions
{
    public class InvalidDiscoEndpointException : Exception
    {
        private string p;

        public InvalidDiscoEndpointException(string p)
        {
            this.p = p;
        }
        
    }
}