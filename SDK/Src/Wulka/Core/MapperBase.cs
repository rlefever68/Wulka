using System.Linq;
using Wulka.Domain.Base;
using Wulka.Interfaces;

namespace Wulka.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBusiness">The type of the source.</typeparam>
    /// <typeparam name="TService">The type of the target.</typeparam>
    /// <remarks></remarks>
    public abstract class MapperBase<TService,TBusiness> : IMapper<TService,TBusiness>
        where TBusiness : DomainObject<TBusiness>
        where TService : DomainObject<TService>
{
    /// <summary>
    /// Maps from target to source internal.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="serviceEntity"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected abstract TBusiness MapFromServiceToBusinessInternal(TService serviceEntity);

    /// <summary>
    /// Maps from source to target internal.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="businessEntity"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected abstract TService MapFromBusinessToServiceInternal(TBusiness businessEntity);


    /// <summary>
    /// Maps from target to source.
    /// </summary>
    /// <param name="serviceEntity">The target.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TBusiness MapFromServiceToBusiness(TService serviceEntity)
    {
        var bus = MapFromServiceToBusinessInternal(serviceEntity);
        bus.Merge(serviceEntity);
        bus.ClearIsDirty();
        return bus;
    }


    /// <summary>
    /// Maps from source to target.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TService MapFromBusinessToService(TBusiness source)
    {
        var serv = MapFromBusinessToServiceInternal(source);
        serv.Merge(source);
        serv.ClearIsDirty();
        return serv;
    }



    /// <summary>
    /// Maps from target to source.
    /// </summary>
    /// <param name="businessEntities">The targets.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TService[] MapFromBusinessToService(TBusiness[] businessEntities)
    {
        return businessEntities.Select(MapFromBusinessToService).ToArray();
    }



    /// <summary>
    /// Maps from source to target.
    /// </summary>
    /// <param name="serviceEntities">The sources.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TBusiness[] MapFromServiceToBusiness(TService[] serviceEntities)
    {
        return serviceEntities.Select(MapFromServiceToBusiness).ToArray();
    }


}
}
