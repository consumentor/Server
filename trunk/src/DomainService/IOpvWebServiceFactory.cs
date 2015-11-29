using Consumentor.ShopGun.Gateway.Opv;

namespace Consumentor.ShopGun.DomainService
{
    public interface IOpvWebServiceFactory
    {
        IProductSearchWebServiceGateway CreateWebServiceProxy();
        void WebServiceCallFailed();
    }
}