namespace Wulka.Core
{
    /// <summary>
    /// Exposes the context in which a framework application can work.
    /// 
    /// You can extend this Context via extension methods and add general
    /// methods to it that can be accessed on multiple levels in an application
    ///
    /// e.g.: Retrieval of the Database Connection String can be added to the
    /// context in the Dao layer and can be used further on in the repository service.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Singleton creation of the Context.
        /// </summary>
        public static Context Instance = new Context();

        



    }
}