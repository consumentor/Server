using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.ExternalInformationProvider;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    public class ExternalProductInformationProviderApplicationService : IExternalProductInformationProviderApplicationService
    {
        private readonly IOpvProductInformationDomainService _opvProductInformationDomainService;

        public ExternalProductInformationProviderApplicationService(IOpvProductInformationDomainService opvProductInformationDomainService)
        {
            _opvProductInformationDomainService = opvProductInformationDomainService;
        }

        public Product FindProductByGtin(string gtin)
        {
            var product = _opvProductInformationDomainService.GetProduct(gtin);
            return product;
        }
    }
}
