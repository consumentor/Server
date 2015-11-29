using System;
using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class BrandBuilder
    {

        public static Brand BuildBrand(string brandName)
        {
            return new Brand
            {
                BrandName = brandName,
                LastUpdated = DateTime.Now,
                Owner = CompanyBuilder.BuildCompany()
            };
        }

        public static Brand BuildBrand()
        {
            return BuildBrand("SomeBrand");
        }
    }
}
