using System;
using System.IO;
using System.Reflection;
using Consumentor.ShopGun.ApplicationService.Server.WebService;
using Consumentor.ShopGun.Gateway.Server;
using NBehave.Spec.NUnit;
using DB = ShopGunSpecBase.Database;
using NUnit.Framework;
using ShopGunSpecBase;

namespace IntegrationTest.WebService.Server
{
    [TestFixture]
    public class ProductSearchServiceGateWaySpecification
    {


        [TestFixture, Category("Integration")]
        public class WhenCheckingAvailabilityOfDefectImageWebService : WebServiceSpec<ProductSearchWebServiceGateway>
                {
                    protected string AssemblyDirectory
                    {
                        get
                        {
                            //string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                            Assembly a = Assembly.GetAssembly(typeof(ProductSearchWebService));

                            UriBuilder uri = new UriBuilder(a.CodeBase);
                            string path = Uri.UnescapeDataString(uri.Path);
                            return Path.GetDirectoryName(path);
                        }
                    }                            

                    protected override void Setup_webService()
                    {
                        var ws = new ServiceUnderTest();
                        string path = AssemblyDirectory;
                        ws.Executable = Path.Combine(path, @"..\..\..\..\ApplicationService.Server\bin\debug\Consumentor.ShopGun.ApplicationService.Server.exe");
                        ws.Arguments = "StartServices";
                        ws.ModifyAppConfigConnectionString("ShopGun", DB.ShopGun.GetConnectionString());
                        AddServiceToStart(ws);
                    }

                    protected override void Wait_for_services_to_start()
                    {
                        WaitForServiceToStart(Sut.Url);
                    }

                    protected override ProductSearchWebServiceGateway Given_this_gateway()
                    {
                        return new ProductSearchWebServiceGateway();
                    }

                    private ProductGWO _result;
                    protected override void When_this_service_is_called()
                    {
                        string gtin = new Guid().ToString();

                        _result = Sut.Search(gtin);
                    }

                    [Test]
                    public void CanInvokeProductSearchWebService()
                    {
                        _result.ShouldNotBeNull();
                    }
                }
    }
}
