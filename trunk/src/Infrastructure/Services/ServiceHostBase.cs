using System.Globalization;
using System.ServiceProcess;
using System.Threading;
using Castle.Core.Logging;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;

namespace Consumentor.ShopGun.Services
{
    public abstract class ServiceHostBase : ServiceBase
    {
        public abstract void OnStartService(string[] args);
        public abstract void OnStopService();
        protected IContainer IocContainer { get; private set; }

        protected ServiceHostBase(IContainer iocContainer, string serviceName)
        {
            IocContainer = iocContainer;
            ServiceName = serviceName;
        }

        public ILogger Log { get; set; }

        protected override void OnStart(string[] args)
        {
            Log.Debug("Service OnStart");
            SetCultureInfo();

            OnStartService(args);
            base.OnStart(args);
        }

        protected void SetCultureInfo()
        {
            var cultureConfiguration = IocContainer.Resolve<IServiceCultureConfiguration>();
            Thread.CurrentThread.CurrentCulture = cultureConfiguration.CultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureConfiguration.UICulture;
            Log.Debug("Service {0} started with CurrentCulture: {1}", ServiceName, CultureInfo.CurrentCulture.ToString());
            Log.Debug("Service {0} started with CurrentUICulture: {1}", ServiceName, CultureInfo.CurrentUICulture.ToString());
        }

        protected override void OnStop()
        {
            Log.Debug("Service OnStop");
            OnStopService();
            base.OnStop();
        }
    }
}