using Castle.Core.Logging;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;

namespace Consumentor.ShopGun.ApplicationService.WebService
{
    public abstract class WebServiceBase
    {
        protected ILogger Log { get; private set; }

        protected IContainer Container { get; private set; }

        protected WebServiceBase()
        {
            IConfiguration configuration = new BasicConfiguration();
            Container = configuration.Container;
            Log = Container.Resolve<ILoggerFactory>().Create(GetType().FullName);

        }
    }
}
