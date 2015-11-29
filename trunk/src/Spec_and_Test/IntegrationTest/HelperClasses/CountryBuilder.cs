using System;
using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class CountryBuilder
    {
        public static Country BuildCountry()
        {
            return new Country
                       {
                           Capital = "Stockholm",
                           CountryCode = new CountryCode {GS1PrefixCode = "SE", Name = "Sweden"},
                           Latitude = "59.20N",
                           Longitude = "18.03E",
                           LastUpdated = DateTime.Now
                       };
        }
    }
}
