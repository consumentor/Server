using System;
using Consumentor.ShopGun.Domain;
using IntegrationTest.Database;

namespace IntegrationTest.HelperClasses
{
    public class ProductBuilder
    {
        private static int gtin = 11111111;
        public static Product BuildProduct()
        {
            return BuildProduct(BrandBuilder.BuildBrand());
        }

        public static Product BuildProduct(Brand brand)
        {
            return BuildProduct(brand, CountryBuilder.BuildCountry());
        }

        public static Product BuildProduct(Brand brand, Country country)
        {
            return new Product()
            {
                ProductName = "Mer brik hallon blåbär m sugrör",
                Brand = brand,
                GlobalTradeItemNumber = gtin++.ToString(),
                Quantity = 20,
                QuantityUnit = "cl",
                OriginCountry = country,
                LastUpdated = DateTime.Now
            };
        }


        public ProductAdvice BuildProductAdvice(Mentor mentor, Semaphore semaphore)
        {
            var productAdvice = new ProductAdvice()
                                   {
                                       Advice = "Sveriges Konsumenter anmälde Coca Cola för vilseledande GDA-märkning av sina Coca Cola-flaskor. Att en flaska om 500 ml skulle utgöra två portioner framstår som vilseledande, likaså det faktum att företaget använder två olika portionsstorlekar när det beräknar GDA.",
                                       Introduction = "Coca Cola anmält för vilseledande GDA-märkning av dryckesflaskor.",
                                       KeyWords = "Coca-Cola, Mer",
                                       Label = "Svenska nyheter",
                                       Mentor = mentor,
                                       Semaphore = semaphore,
                                       Published = true,
                                       PublishDate = DateTime.Now
                                   };
            return productAdvice;
        }
    }
}