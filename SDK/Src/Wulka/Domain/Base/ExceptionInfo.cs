using System;

namespace Wulka.Domain.Base
{
    public class ExceptionInfo 
    {
        public Exception TheException { get; set; }
        public string[] SystemInfo { get; set; }
    }
}
