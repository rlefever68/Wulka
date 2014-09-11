using System;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// Represents a CouchDB HTTP 409 conflict.
    /// </summary>
    public class BigDConflictException : Exception
    {
        public BigDConflictException(string msg, Exception e) : base(msg, e)
        {
        }
    }
}