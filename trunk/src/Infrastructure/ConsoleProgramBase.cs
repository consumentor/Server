using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using Castle.Core.Logging;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Services;

namespace Consumentor.ShopGun
{
    public abstract class ConsoleProgramBase : ServiceHostBase
    {
        private static readonly List<ServiceHostBase> _startedServices = new List<ServiceHostBase>();

        protected ConsoleProgramBase(string serviceName)
            : base(GetContainer(), serviceName)
        {
        }

        protected abstract IEnumerable<Type> ServicesToStart { get; }
        protected abstract void CreateDatabase();
        protected abstract void EncryptConfigSectionsToBeEncrypted();
        protected abstract void TestClient(string[] args);
        protected abstract void ShowHelp();

        public override void OnStartService(string[] args)
        {
            Log.Debug("Service started {0}", GetType().Name);

            var servicesToRun = new List<ServiceHostBase>();
            servicesToRun.AddRange(ResolveService(ServicesToStart));
            foreach (var service in servicesToRun)
            {
                StartService(service, args);
                StartedServices.Add(service);
            }
            Log.Info("All services started.");
        }

        public override void OnStopService()
        {
            foreach (var host in StartedServices)
            {
                if (host.ServiceName != ServiceName)
                {
                    Log.Debug("Stopping service: {0}", host.ServiceName);
                    host.OnStopService();
                }
            }
        }

        protected void Run(string[] args)
        {
            bool enterToQuit = true;

            try
            {
                Log = GetContainer().Resolve<ILoggerFactory>().Create(GetType().FullName);
                if (IsService())
                {
                    args = new[] { "StartServices" };
                }
                if (args.Length == 0 || args[0].Equals("Help"))
                {
                    args = new[] { "Help" };
                }
                else
                {
                    Log.Info(GetType().Namespace);
                    foreach (var arg in args)
                    {
                        Log.Debug("Argument passed: {0}", arg);
                    }
                }
                SetCultureInfo();

                for (int argNr = 0; argNr < args.Length; argNr++)
                {
                    var arg = args[argNr];
                    switch (arg)
                    {
                        case "EncryptAppConfig":
                            EncryptConfigSectionsToBeEncrypted();
                            enterToQuit = false;
                            break;
                        case "ReserveUri":
                            ReserveUriForUser(args[argNr + 1]);
                            args[argNr + 1] = "Skip"; // We dont want to end up in default case in next iteration
                            enterToQuit = false;
                            break;
                        case "CreateDatabase":
                            enterToQuit = false;
                            CreateDatabase();
                            Log.Info("Databases created.");
                            break;
                        case "SetTime":
                            ShopGunTime.SetNow(DateTime.Parse(args[argNr + 1], CultureInfo.CurrentCulture));
                            args[argNr + 1] = "Skip"; // We dont want to end up in default case in next iteration
                            break;
                        case "Skip":
                            break;
                        case "StartServices":
                            StartServices(args);
                            break;
                        case "TestMode":
                            TestClient(args);
                            break;
                        case "Help":
                            enterToQuit = false;
                            ShowHelp();
                            break;
                        default:
                            enterToQuit = false;
                            ShowHelp();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw;
            }
            finally
            {
                Quit(enterToQuit);
            }
            //Log.Debug("ConsoleProgram Main() exiting.");
        }

        private void ReserveUriForUser(string userName)
        {
            string currentUserName = "[Unknown]";
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            if (currentUser != null)
                currentUserName = currentUser.Name;
            Log.Debug("{0} is attempting to reserve Uris on all services", currentUserName);
            foreach (Type service in ServicesToStart)
            {
                var serviceInstance = IocContainer.Resolve(service) as IReserveUri;
                if (serviceInstance != null)
                    serviceInstance.ReserveUriForUser(userName);
            }
        }

        protected IList<ServiceHostBase> StartedServices
        {
            get { return _startedServices; }
        }

        private void StartServices(string[] args)
        {
            if (IsService())
            {
                Run(this);
            }
            else
            {
                HostInProgram(ResolveService(ServicesToStart), args);
            }
        }

        private void StartService(ServiceHostBase host, string[] args)
        {
            if (host.ServiceName != ServiceName) // Dont start the console host recursively, it will start the other hosts and stuff will crash.
            {
                Log.Debug("Starting sub service {0}...", host.ServiceName);
                host.OnStartService(args);
            }
        }

        private IEnumerable<ServiceHostBase> ResolveService(IEnumerable<Type> serviceTypes)
        {
            var container = GetContainer();
            var resolvedServices = new List<ServiceHostBase>(serviceTypes.Count());
            foreach (var type in serviceTypes)
            {
                Log.Debug("ResolveService: {0}", type.FullName);
                var service = (ServiceHostBase)container.Resolve(type);
                Log.Debug("ResolveService has name {0}", service.ServiceName);
                resolvedServices.Add(service);
            }
            return resolvedServices;
        }

        private void HostInProgram(IEnumerable<ServiceHostBase> services, string[] args)
        {
            foreach (var host in services)
            {
                StartService(host, args);
                StartedServices.Add(host);
            }
        }

        public const string PressEnterToQuit = "Press [Enter] to quit.";
        protected void Quit(bool pressEnterToQuit)
        {
            if ((pressEnterToQuit) && (IsService() == false))
            {
                Console.WriteLine(PressEnterToQuit);
                Console.ReadLine();
            }
            if (IsService() == false)
                OnStopService();
        }

        protected bool IsService()
        {
            var serviceController = GetServiceControllerStatus();
            //Log.Debug("Has instance of serviceController? {0}", serviceController != null);
            if (serviceController != null)
                Log.Debug("serviceController.Status: {0}", serviceController);
            //Log.Debug("Environment.UserInteractive? {0}", Environment.UserInteractive);

            if (serviceController == null)
                return false;
            var validStatus = new[] { ServiceControllerStatus.StartPending, ServiceControllerStatus.ContinuePending, ServiceControllerStatus.Running };
            return (Environment.UserInteractive == false) && (validStatus.Contains(serviceController.Value));
        }

        private ServiceControllerStatus? GetServiceControllerStatus()
        {
            try
            {
                var serviceController = new ServiceController(ServiceName);
                ServiceControllerStatus status = serviceController.Status;
                Log.Debug("ServiceControllerStatus Service name: {0}", ServiceName);
                return status;
            }
            //System.InvalidOperationException: Service Consumentor.ShopGun.Server.ConsoleProgram v1.0.0.0 was not found on computer '.'. ---> System.ComponentModel.Win32Exception: The specified service does not exist as an installed service
            catch (InvalidOperationException)
            {
                //Log.Debug("Logging error:" + e.GetType().FullName, e);
            }

            return null;
        }

        protected static IContainer GetContainer()
        {
            return Configuration.Container;
        }

        protected static IConfiguration Configuration
        {
            get { return new BasicConfiguration(); }
        }
    }
}