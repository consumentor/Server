using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    public interface IExternalProductInformationProviderApplicationService
    {
        Product FindProductByGtin(string gtin);
    }
}