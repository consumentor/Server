using System;
using Castle.Core.Logging;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Gateway.Configuration;
using Consumentor.ShopGun.Gateway.Opv;

namespace Consumentor.ShopGun.DomainService
{
    public class OpvWebServiceFactory : IOpvWebServiceFactory
    {
        private readonly IContainer _container;
        private static DateTime _lastFailTime = DateTime.MinValue;

        public OpvWebServiceFactory()
        {
            IConfiguration configuration = new BasicConfiguration();
            _container = configuration.Container;
            Log = _container.Resolve<ILoggerFactory>().Create(GetType().FullName);
        }

        protected ILogger Log { get; private set; }

        public IProductSearchWebServiceGateway CreateWebServiceProxy()
        {
            var webServiceProxy = _container.Resolve<ProductSearchWebServiceGateway>();
            var wsConfig = new OPVWebServiceConfiguration();

            var ah = new AuthHeaderGWO
            {
                Username = wsConfig.Username,
                Password = wsConfig.Password
            };

            webServiceProxy.Url = WebServiceUrl;
            webServiceProxy.AuthHeaderValue = ah;
            webServiceProxy.Timeout = 5000;

            return webServiceProxy;
        }

        private static string WebServiceUrl
        {
            get
            {
                var wsConfig = new OPVWebServiceConfiguration();
                // If a call has failed within the last two minutes, use the secondary server
                // Otherwise, use the primary
                if (_lastFailTime.AddMinutes(2).CompareTo(DateTime.Now) > 0)
                    return wsConfig.SecondaryUrl;    // http://www2.mediabanken.se/.....
                else
                    return wsConfig.PrimaryUrl;    // http://www.mediabanken.se/.....
            }
        }

        /// <summary>
        /// When called, it will log the time the error occurred, wich will cause the factory class to
        /// call the alternate site instead
        /// </summary>
        public void WebServiceCallFailed()
        {
            _lastFailTime = DateTime.Now;
        }
    }
}
