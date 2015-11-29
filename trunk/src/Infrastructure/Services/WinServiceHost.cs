using System;
using Consumentor.ShopGun.Component;

namespace Consumentor.ShopGun.Services
{
    public class WinServiceHost<T> : ServiceHostBase where T : class, IDisposable
    {
        private T _hostedService;

        public WinServiceHost(IContainer iocContainer)
            : base(iocContainer, typeof(T).WindowsServiceName())
        { }

        public override void OnStartService(string[] args)
        {
            Log.Debug("starting host for {0} with ServiceName {1}", typeof(T).FullName, ServiceName);
            try
            {
                _hostedService = IocContainer.Resolve<T>();
            }
            catch (Exception e)
            {
                Log.Fatal(e.Message, e);
                throw;
            }
        }

        public override void OnStopService()
        {
            if (_hostedService != null)
            {
                Log.Debug("Closing host for {0}", typeof(T).FullName);
                _hostedService.Dispose();
                _hostedService = null;
            }
        }
    }
}