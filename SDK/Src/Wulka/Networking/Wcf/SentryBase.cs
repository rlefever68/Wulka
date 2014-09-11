using System.ServiceModel;
using NLog;

namespace Wulka.Networking.Wcf
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, MaxItemsInObjectGraph = 2147483647)]
    public abstract class SentryBase : ContextServiceBase
    {



        protected SentryBase()
        {

        }




        // Registration of Domain Objects has been put in ServiceHost Factory Code

       /// <summary>
       /// You MUST override this method, but you cannot use 
       /// Initializing code in the constructor that references itself (since the object is not yet created) - Obsolete remark
       /// REMARK: since the code has been moved to the onOpen method of the servicehost; you can be certain now that 
       /// the object has been created.
       /// </summary>
        protected abstract void RegisterRequiredDomainObjects();



        internal void RegisterDomainObjects()
        {
            _logger.Debug("Registering Domain Objects.");
            RegisterRequiredDomainObjects();
        }


        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    }
}
