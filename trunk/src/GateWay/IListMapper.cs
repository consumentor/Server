using System.Collections.Generic;

namespace Consumentor.ShopGun.Gateway
{
    public interface IListMapper<TDomain,TGateway> : IMapper<IEnumerable<TDomain>, IEnumerable<TGateway>>
    {
        IEnumerable<TGateway> Map<TDomainBase>(IEnumerable<TDomainBase> defects) where TDomainBase : class;
        IEnumerable<TDomainBase> Map<TDomainBase>(IEnumerable<TGateway> defects) where TDomainBase : class;
    }
}