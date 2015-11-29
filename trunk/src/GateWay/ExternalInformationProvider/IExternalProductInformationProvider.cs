using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.Gateway.ExternalInformationProvider
{
    public interface IExternalProductInformationProvider
    {
        ProductInformation GetProductByGtin(string gtin);
        ProductInformation GetFullProductDataByGtin(string gtin);
    }
}