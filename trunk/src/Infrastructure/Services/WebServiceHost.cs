using System;
using System.ServiceModel;
using Consumentor.ShopGun.Component;

namespace Consumentor.ShopGun.Services
{
    public class WebServiceHost<T> : ServiceHostBase, IReserveUri where T : class
    {
        private ServiceHost _host;

        public WebServiceHost(IContainer iocContainer)
            : base(iocContainer, typeof(T).WindowsServiceName())
        {
        }

        private Uri Uri
        {
            get { return _host.Description.Endpoints[0].Address.Uri; }
        }

        public override void OnStartService(string[] args)
        {
            Log.Debug("starting host for {0} with ServiceName {1}", typeof(T).FullName, ServiceName);

            //_host = new ServiceHost(IocContainer.Resolve<T>());
            Host.Open();
            Log.Debug("Host {0} has URI {1}", typeof(T).FullName, Uri);
        }

        private ServiceHost Host
        {
            get
            {
                if (_host == null)
                    _host = new ServiceHost(typeof(T));
                return _host;
            }
        }

        public void ReserveUriForUser(string userName)
        {
            string uri = Host.Description.Endpoints[0].Address.Uri.ToString().Replace("localhost", "+");
            ServiceConfigurationManager.ModifyReservation(uri, userName, false);
            Log.Debug("ServiceConfiguration for {0} on {1} succeded!", userName, uri);
        }


        public override void OnStopService()
        {
            if (_host != null)
            {
                Log.Debug("Closing host for {0}", typeof(T).FullName);
                _host.Close();
                _host = null;
            }
        }
    }
}