using System;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Gateway;
using Consumentor.ShopGun.Gateway.Opv;
using Consumentor.ShopGun.Log;

namespace Consumentor.ShopGun.DomainService.Server.ExternalInformationProvider
{
    [Interceptor(typeof(LogInterceptor))]
    public class OpvProductInformationDomainService : IOpvProductInformationDomainService
    {
        private readonly IMapper<Product, ProductGWO> _opvProductMapper;
        private readonly ICategoryInfoDomainService _categoryInfoDomainService;
        private readonly IOpvWebServiceFactory _opvWebServiceFactory;
        

        public OpvProductInformationDomainService(
            IMapper<Product, ProductGWO> opvProductMapper, 
            ICategoryInfoDomainService categoryInfoDomainService,
            IOpvWebServiceFactory opvWebServiceFactory)
        {
            _opvProductMapper = opvProductMapper;
            _opvWebServiceFactory = opvWebServiceFactory;
            _categoryInfoDomainService = categoryInfoDomainService;
        }

        public ILogger Log { get; set; }

        public Product GetProduct(string gtin)
        {
            var opvProductService = _opvWebServiceFactory.CreateWebServiceProxy();

            ProductGWO[] opvResult;
            try
            {
                opvResult = opvProductService.GetProductData(new[]{gtin});

                if (opvResult == null)
                {
                    throw new Exception("Web Service returned null");
                }
            }
            catch (Exception e)
            {
                Log.Debug("Exception when calling OPV", e);

                // Tell the Web Service Factory that the call failed
                // Which will cause it to use the secondary server for a time
                _opvWebServiceFactory.WebServiceCallFailed();

                // Redo the Web Service call, but now it will go to the secondary server
                opvProductService = _opvWebServiceFactory.CreateWebServiceProxy();
                opvResult = opvProductService.GetProductData(new []{gtin});
            }

            if (opvResult != null && opvResult.Length == 1 && !string.IsNullOrEmpty(opvResult[0].Name) )
            {
                var product = opvResult[0];
                var mappedProduct = _opvProductMapper.Map(product);
                var categoryInfo = _categoryInfoDomainService.GetRandomCategoryInfo(product.Category);
                mappedProduct.Description = categoryInfo == null ? null : categoryInfo.InfoText;
                return mappedProduct;
            }
            return null;
        }

        public Company GetSupplier(string gtin)
        {
            var product = GetProduct(gtin);
            if (product == null)
            {
                return null;
            }
            return product.Brand.Owner;
        }
    }
}
