using System.Configuration.Install;
using System.Globalization;
using System.ServiceProcess;

namespace Consumentor.ShopGun.Services
{
    public abstract  class WindowsServiceInstallerBase : Installer
    {
        private readonly ServiceProcessInstaller _processInstaller = new ServiceProcessInstaller();
        private readonly ServiceInstaller _installer = new ServiceInstaller();

        protected WindowsServiceInstallerBase(ServiceStartMode startMode, ServiceAccount account, string serviceName, string description)
        {
            _installer.StartType = startMode;
            _processInstaller.Account = account;
            _installer.Description = description;
            _installer.ServiceName = serviceName;

            Installers.Add(_installer);
            Installers.Add(_processInstaller);
        }


        public string GetContextParameter(string key)
        {
            string value = "";
            if (Context.Parameters.ContainsKey(key))
                value = Context.Parameters[key].Trim();
            return value;
        }

        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            var serviceDomain = GetContextParameter("ServiceDomain");
            var serviceUser = GetContextParameter("ServiceUser");
            var password = GetContextParameter("ServicePassword");
            if (serviceUser.Length > 0)
            {
                _processInstaller.Account = ServiceAccount.User;
                _processInstaller.Username = string.Format(CultureInfo.CurrentCulture, @"{0}\{1}", serviceDomain, serviceUser);
                _processInstaller.Password = password;
            }
            base.OnBeforeInstall(savedState);
        }

    }
}