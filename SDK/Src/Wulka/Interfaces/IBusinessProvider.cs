using System.ServiceModel;
using Wulka.Domain.Base;

namespace Wulka.Interfaces
{
    public interface IBusinessProvider
    {
        [OperationContract]
        Result RegisterDomainObjects();
    }
}