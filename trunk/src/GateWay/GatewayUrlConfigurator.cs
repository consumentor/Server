using System;
using System.Configuration;
using System.Globalization;
using System.Web.Services.Protocols;
using Consumentor.ShopGun.Configuration;

namespace Consumentor.ShopGun.Gateway
{
    public class GatewayUrlConfigurator : ConfigurationBase
    {
        private static readonly GatewayUrlConfigurator _config = new GatewayUrlConfigurator();

        private GatewayUrlConfigurator()
        { }

        public static void Configure(SoapHttpClientProtocol gateway)
        {
            if (gateway == null)
                throw new ArgumentNullException("gateway");
            string url = _config.GetValueFromAppConfig<string>(gateway.GetType().FullName, null);
            if (string.IsNullOrEmpty(url))
                throw new ConfigurationErrorsException(string.Format(CultureInfo.CurrentCulture, "The configuration key {0} for this gateway is missing", gateway.GetType().FullName));
            gateway.Url = url;
        }
    }
}