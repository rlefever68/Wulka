using System;
using Wulka.Domain.Interfaces;

namespace Wulka.Interfaces
{
    public interface IAppContractAgent 
    {
        IAppContract RegisterAppUsage(IAppContract item);
        void RegisterAppUsageAsync(IAppContract item, Action<IAppContract> act=null);
    }
}
