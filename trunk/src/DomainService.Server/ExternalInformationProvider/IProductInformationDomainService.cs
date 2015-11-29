using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server.ExternalInformationProvider
{
    public interface IProductInformationDomainService
    {
        Product GetProduct(string gtin);
        Company GetSupplier(string gtin);
    }
}