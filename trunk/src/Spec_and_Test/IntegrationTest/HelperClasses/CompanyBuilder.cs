using System;
using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class CompanyBuilder
    {
        public static Company BuildCompany()
        {
            return new Company
                       {
                           CompanyName = "Coca-Cola Company",
                           LastUpdated = DateTime.Now,
                           URLToHomePage = "http://www.thecoca-colacompany.com/"
                       };
        }
    }
}
