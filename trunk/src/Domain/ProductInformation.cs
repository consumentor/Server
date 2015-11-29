using System.Collections.Generic;

namespace Consumentor.ShopGun.Domain
{
    public class ProductInformation
    {
        public string ProductName { get; set; }
        public string Gtin { get; set; }
        public string TableOfContents { get; set; }
        public IList<string> Markings { get; set; }
        public string ImageUrlSmall { get; set; }
        public string ImageUrlMedium { get; set; }
        public string ImageUrlLarge { get; set; }
        public string CompanyName { get; set; }
        public string BrandName { get; set; }
        public string WeightVolume { get; set; }

    }
}
