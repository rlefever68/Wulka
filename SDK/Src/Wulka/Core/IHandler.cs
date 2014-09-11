namespace Wulka.Core
{
    /// <summary>
    /// Exposes the provider interface
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Gets the Provider key that will uniquely
        /// identify a provider in a ControllerBase.
        /// </summary>
        /// <value>The Provider key.</value>
        string HandlerKey { get; }
    }
}