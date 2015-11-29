using System.Web.Services.Protocols;

namespace Consumentor.ShopGun.Gateway
{
    public abstract class WebServiceBase : SoapHttpClientProtocol, IWebServiceSettings
    {
        protected WebServiceBase()
        {
            GatewayUrlConfigurator.Configure(this);
        }

        string IWebServiceSettings.Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                base.Url = value;
            }
        }
    }
}