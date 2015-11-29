using System.ComponentModel;
using System.ServiceProcess;
using Consumentor.ShopGun.Services;

namespace Consumentor.ShopGun.ApplicationService.Server.WinServiceInstaller
{
    [RunInstaller(true)]
    public class Installer : WindowsServiceInstallerBase
    {
        public Installer() 
            : base(ServiceStartMode.Automatic,
            ServiceAccount.NetworkService,
            ServiceName.GetName(typeof(ConsoleProgram)),
            "Host for " + ServiceName.GetName(typeof(ConsoleProgram)))
        {}
    }
}