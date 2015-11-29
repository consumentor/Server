using System;
using System.Collections.Generic;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Gateway.Configuration;
using Consumentor.ShopGun.Gateway.se.mediabanken.www;
using OPVProduct = Consumentor.ShopGun.Gateway.se.mediabanken.www.Product;

namespace Consumentor.ShopGun.Gateway.ExternalInformationProvider
{
    public class OPVWebServiceCaller : IOPVWebServiceCaller
    {
        public ProductInformation GetProductByGtin(string gtin)
        {
            var opvProductService = WebServiceFactory.CreateProductServiceProxy();

            OPVProduct[] opvResult;
            try
            {
                opvResult = opvProductService.GetProductData(new[]{gtin});

                if (opvResult == null)
                {
                    throw new Exception("Web Service returned null");
                }
            }
            catch (Exception)
            {
                // Log the exception


                // Tell the Web Service Factory that the call failed
                // Which will cause it to use the secondary server for a time
                WebServiceFactory.WebServiceCallFailed();

                // Redo the Web Service call, but now it will go to the secondary server
                opvProductService = WebServiceFactory.CreateProductServiceProxy();
                opvResult = opvProductService.GetProductData(new []{gtin});
            }

            if (opvResult.Length == 1 && !string.IsNullOrEmpty(opvResult[0].Name) )
            {
                var product = opvResult[0];
                var markings = GetMarkingNamesForProduct(product);
                return new ProductInformation
                           {
                               BrandName = product.Trademark,
                               CompanyName = product.Supplier,
                               TableOfContents = product.Content,
                               Gtin = product.EAN,
                               ImageUrlSmall = product.ImageURL_Jpg66px,
                               ImageUrlMedium = product.ImageURL_Jpg150px,
                               ImageUrlLarge = product.ImageURL_Jpg300px,
                               Markings = markings,
                               ProductName = product.Name,
                               WeightVolume = product.WeightVolume
                           };
            }

            return new ProductInformation{Gtin = gtin};
        }

        public ProductInformation GetFullProductDataByGtin(string gtin)
        {
            throw new NotImplementedException();
        }

        private static IList<string> GetMarkingNamesForProduct(OPVProduct product)
        {
            var markingNames = new List<string>(product.Markings.Length);
            markingNames.AddRange(product.Markings.Select(markInfo => markInfo.MarkName));
            return markingNames;
        }
    }

    class WebServiceFactory
    {

        protected static DateTime lastFailTime = DateTime.MinValue;


        public static ProductService CreateProductServiceProxy()
        {
            var wsConfig = new OPVWebServiceConfiguration();

            var ps = new ProductService();
            AuthHeader ah = new AuthHeader();

            //Todo get config to work...
            ah.username = "ConsumentorWSUser";
            ah.password = "GcbpTbNj03Lz";
            //ah.username = wsConfig.Username;
            //ah.password = wsConfig.Password;
            ps.AuthHeaderValue = ah;
            ps.Timeout = 10000;  // 10 seconds timeout
            ps.Url = "http://www.mediabanken.se/WS/Consumentor/ProductService.asmx";

            return ps;
        }

        protected static string webServiceURL
        {

            get
            {
                var wsConfig = new OPVWebServiceConfiguration();
                // If a call has failed within the last two minutes, use the secondary server
                // Otherwise, use the primary
                if (lastFailTime.AddMinutes(2).CompareTo(DateTime.Now) > 0)
                    return wsConfig.SecondaryUrl;    // http://www2.mediabanken.se/.....
                else
                    return wsConfig.PrimaryUrl;    // http://www.mediabanken.se/.....
            }
        }

        /// <summary>
        /// When called, it will log the time the error occurred, wich will cause the factory class to
        /// call the alternate site instead
        /// </summary>
        public static void WebServiceCallFailed()
        {
            lastFailTime = DateTime.Now;
        }


    }
}
