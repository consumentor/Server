using System.ServiceModel.Web;
using Consumentor.ShopGun.Configuration;

namespace Consumentor.ShopGun.Context
{
    public class ShopgunWebOperationContext : IShopgunWebOperationContext
    {
        private readonly IWebServiceConfiguration _webServiceConfiguration;
        private readonly WebOperationContext _context;
        public ShopgunWebOperationContext(IWebServiceConfiguration webServiceConfiguration)
        {
            _context = WebOperationContext.Current;
            if (_context == null) return;
            _webServiceConfiguration = webServiceConfiguration;
        }

        public string UserAgent
        {
            get
            {
                if (_context != null)
                {
                    return _context.IncomingRequest.Headers[_webServiceConfiguration.UserAgent] ?? string.Empty;
                }
                return string.Empty;
            }
        }

        public string IMEI
        {
            get
            {
                if (_context != null)
                {
                    return _context.IncomingRequest.Headers[_webServiceConfiguration.IMEI] ?? string.Empty;
                }
                return string.Empty;
            }
        }

        public string Model
        {
            get
            {
                if (_context != null)
                {
                    return _context.IncomingRequest.Headers[_webServiceConfiguration.Model] ?? string.Empty;
                }
                return string.Empty;
            }
        }

        public string OsVersion
        {
            get
            {
                if (_context != null)
                {
                    return _context.IncomingRequest.Headers[_webServiceConfiguration.OsVersion] ?? string.Empty;
                }
                return string.Empty;
            }
        }
    }
}
