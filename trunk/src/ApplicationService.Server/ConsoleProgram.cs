using System;
using System.Collections.Generic;
using Consumentor.ShopGun.ApplicationService.Server.WebService;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Repository;
using Consumentor.ShopGun.Services;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    public class ConsoleProgram : ConsoleProgramBase
    {
        private static readonly IEnumerable<Type> _myServices = new List<Type>
                                                                    {
                                                                        typeof(WebServiceHost<ShopgunAppWebService>),
                                                                        typeof(WebServiceHost<AdviceSearchWebService>),
                                                                        typeof(WebServiceHost<ShopgunMembershipWebService>)
                                                                    };

        public ConsoleProgram()
            : base(Services.ServiceName.GetName(typeof(ConsoleProgram)))
        {
        }

        public static void Main(string[] args)
        {
            var me = new ConsoleProgram();
            me.Run(args);
        }

        protected override IEnumerable<Type> ServicesToStart
        {
            get { return _myServices; }
        }

        protected override void CreateDatabase()
        {
            IConfiguration config = new BasicConfiguration();
            var container = config.Container;
            DataContext context = container.Resolve<DataContext>();

            if (context.DatabaseExists())
                context.DeleteDatabase();
            context.CreateDatabase();
        }

        protected override void EncryptConfigSectionsToBeEncrypted()
        {
            throw new NotImplementedException();
        }

        protected override void TestClient(string[] args)
        {
            throw new NotImplementedException();
        }

        protected override void ShowHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("");
            Console.WriteLine("Consumentor.ShopGun.ApplicationService.Server [Option]");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("");
            Console.WriteLine("StartServices [<time>]  Start application services like web services,");
            Console.WriteLine("                        prenumeration registrations etc.");
            Console.WriteLine("                        System time for the process can be set.");
            //Console.WriteLine("TestMode [<time>]       Start application in test mode.");
            //Console.WriteLine("                        Parsing of Lasor report without need for");
            //Console.WriteLine("                        PLC connection.");
            //Console.WriteLine("                        System time for the process can be set.");
            Console.WriteLine("CreateDatabase          Creates databases for client.");
            Console.WriteLine("                        Drops existing databases.");
            Console.WriteLine("Help                    Show this help text.");
            Console.WriteLine("");
            Console.WriteLine("Example:");
            Console.WriteLine("");
            Console.WriteLine("Consumentor.ShopGun.ApplicationService.Server StartServices");
            Console.WriteLine("");
        }
    }
}