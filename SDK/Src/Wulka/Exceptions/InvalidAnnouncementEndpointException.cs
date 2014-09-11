using System;

namespace Wulka.Exceptions
{
    public class InvalidAnnouncementEndpointException : Exception
    {

        private string _p;

        public InvalidAnnouncementEndpointException(string message)
        {
            _p = message;
        }
    }
}