namespace Consumentor.ShopGun.Gateway
{
    public interface IMapper<TDomainObject, TGatewayObject>
    {
        TGatewayObject Map(TDomainObject source);
        TDomainObject Map(TGatewayObject source);
    }
}