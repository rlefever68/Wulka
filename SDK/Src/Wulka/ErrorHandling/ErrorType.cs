namespace Wulka.ErrorHandling
{
    /// <summary>
    /// Defines the error levels, used in the <see cref="ErrorInfo"/> class.
    /// </summary>
    public enum ErrorType : uint
    {
        /// <summary>
        /// 
        /// </summary>
        Success         = 0x00000000,
        /// <summary>
        /// 
        /// </summary>
        Informational   = 0x40000000,
        /// <summary>
        /// 
        /// </summary>
        Warning         = 0x80000000,
        /// <summary>
        /// 
        /// </summary>
        Error           = 0xC0000000,
    }
}
