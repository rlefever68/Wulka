namespace Wulka.Interfaces
{
    public interface IMapper<TServiceEntity , TBusinessEntity>
    {
        TServiceEntity MapFromBusinessToService(TBusinessEntity businessEntity);
        TBusinessEntity MapFromServiceToBusiness(TServiceEntity serviceEntity);
        
        TServiceEntity[] MapFromBusinessToService(TBusinessEntity[] businessEntity);
        TBusinessEntity[] MapFromServiceToBusiness(TServiceEntity[] serviceEntity);
    }
}
