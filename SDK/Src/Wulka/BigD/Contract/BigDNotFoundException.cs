using System;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// Represents a HttpStatusCode of 404, document not found.
    /// </summary>
    public class BigDNotFoundException : Exception
    {
        public BigDNotFoundException(string msg, Exception e) : base(msg, e)
        {
        }
    }
}