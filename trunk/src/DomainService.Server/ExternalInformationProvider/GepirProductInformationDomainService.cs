using System;
using System.Linq;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.Gateway;
using Consumentor.ShopGun.Gateway.se.gs1.gepir;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Server.ExternalInformationProvider
{
    [Interceptor(typeof(LogInterceptor))]
    public class GepirProductInformationDomainService : IGepirProductInformationDomainService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IMapper<Product, itemDataLineType> _gepirProductMapper;
        private readonly IMapper<Company, partyDataLineType> _gepirCompanyMapper;

        private readonly IRepository<Company> _companyRepository;



        public GepirProductInformationDomainService(IMapper<Product, itemDataLineType> gepirProductMapper, IMapper<Company, partyDataLineType> gepirCompanyMapper, RepositoryFactory repositoryFactory, IRepository<Company> companyRepository)
        {
            _repositoryFactory = repositoryFactory;
            _companyRepository = companyRepository;
            _gepirCompanyMapper = gepirCompanyMapper;
            _gepirProductMapper = gepirProductMapper;
        }

        public ILogger Log { get; set; }

        public Product GetProduct(string gtin)
        {
            var gepirProductService = WebServiceFactory.CreateProductServiceProxy();

            var mappedProduct = new Product
                                    {GlobalTradeItemNumber = gtin, LastUpdated = DateTime.Now};

            try
            {
                var paddedGtin = gtin.PadLeft(14, '0');
                var getItemByGtin = new GetItemByGTIN { requestedGtin = paddedGtin, versionSpecified = false };
                var itemByGtin = gepirProductService.GetItemByGTIN(getItemByGtin);

                // If request ok (return code 0) and product found
                if (gepirProductService.gepirResponseHeaderValue.returnCode == 0
                    && itemByGtin != null
                    && itemByGtin.itemDataLine != null
                    && itemByGtin.itemDataLine.Length > 0)
                {
                    var gepirProduct = itemByGtin.itemDataLine[0];

                    //TODO Sometimes leads to NotSupportedException - Adding/Attaching an item that is not new
                    //UpdateDomain(gepirProductService, gtin, gepirProduct);
                    //mappedProduct = _productDomainService.FindProductByGtin(gtin, true);

                    mappedProduct = _gepirProductMapper.Map(gepirProduct);
                }
                // If the product couldn't be found we can still get the GTIN owner
                else
                {
                    var getPartyByGtin = new GetPartyByGTIN() { requestedGtin = new[] { paddedGtin }, versionSpecified = false };
                    var partyByGtin = gepirProductService.GetPartyByGTIN(getPartyByGtin);
                    var brand = new Brand {LastUpdated = DateTime.Now};
                    if (gepirProductService.gepirResponseHeaderValue.returnCode == 0 
                        && partyByGtin != null 
                        && partyByGtin.partyDataLine != null 
                        && partyByGtin.partyDataLine.Length > 0)
                    {
                        //using (var companyRepository = _repositoryFactory.Build<IRepository<Company>, Company>())
                        //{
                            var gepirCompany = partyByGtin.partyDataLine[0];
                            Company company = null;
                            var matchingCompanies = _companyRepository.Find(x => x.CompanyName == gepirCompany.partyName);
                            if (!matchingCompanies.Any())
                            {
                                company = _gepirCompanyMapper.Map(gepirCompany);
                                _companyRepository.Add(company);
                                _companyRepository.Persist();
                            }
                            else if (matchingCompanies.Count() == 1)
                            {
                                company = matchingCompanies.First();
                            }
                            brand.Owner = company;
                            mappedProduct.Brand = brand;
                        //}
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Gepir router threw an exception.", exception);
                if (gepirProductService.gepirResponseHeaderValue != null)
                {
                    Log.Error("Gepir returnCode:", gepirProductService.gepirResponseHeaderValue.returnCode);
                }
                return null;
            }

            return mappedProduct;
        }

        public Company GetSupplier(string gtin)
        {
            throw new NotImplementedException();
        }

        private void UpdateDomain(router router, string gtin, itemDataLineType gepirProduct)
        {
            if (string.IsNullOrEmpty(gepirProduct.brandName))
            {
                return;
            }
            using (var brandRepository = _repositoryFactory.Build<IBrandRepository, Brand>())
            {
                Brand brand = null;
                var matchingBrands = brandRepository.Find(x => x.BrandName == gepirProduct.brandName);

                // If a brand with the given name doesn't exist yet, we create it.
                if (!matchingBrands.Any())
                {
                    brand = new Brand{BrandName = gepirProduct.brandName, LastUpdated = DateTime.Now};
                    brandRepository.Add(brand);
                }
                // Else if there exists exactly one brand with the given name we use that brand
                else if (matchingBrands.Count() == 1)
                {
                    brand = matchingBrands.First();

                    // If the brand we retrieved doesn't have an owner we can add it as well
                    if (brand.Owner == null)
                    {
                        var manufacturerGln = gepirProduct.manufacturerGln ?? gepirProduct.informationProviderGln;
                        var getPartyByGln = new GetPartyByGLN { requestedGln = new[] { manufacturerGln }, versionSpecified = false };
                        var partyByGln = router.GetPartyByGLN(getPartyByGln);
                        if (partyByGln != null && partyByGln.partyDataLine != null && partyByGln.partyDataLine.Length > 0)
                        {
                            using (var companyRepository = _repositoryFactory.Build<IRepository<Company>, Company>())
                            {

                                var manufacturer = partyByGln.partyDataLine[0];
                                Company company = null;
                                var matchingCompanies =
                                    companyRepository.Find(x => x.CompanyName == manufacturer.partyName);
                                if (!matchingCompanies.Any())
                                {
                                    company = _gepirCompanyMapper.Map(manufacturer);
                                    companyRepository.Add(company);
                                    companyRepository.Persist();
                                }
                                else if(matchingCompanies.Count() == 1)
                                {
                                    company = matchingCompanies.First();
                                }

                                // If we added or found the correct company
                                if (company != null)
                                {
                                    //Todo: This should actually be done within the repository
                                    var brandOwner = brandRepository.FindDomainObject(company);
                                    brand.Owner = brandOwner;
                                }

                                brandRepository.Persist();

                                using (var productRepository = _repositoryFactory.Build<IProductRepository, Product>())
                                {
                                    // If product doesn't exist in database yet - add it.
                                    if (!productRepository.Find(x => x.GlobalTradeItemNumber == gtin).Any())
                                    {
                                        gepirProduct.gtin = gtin;
                                        var newProduct = _gepirProductMapper.Map(gepirProduct);
                                        productRepository.Add(newProduct);
                                        productRepository.Persist();
                                    }
                                }
                            }
                        }
                    }
                }

                if (brand != null && brand.Owner == null)
                {
                    
                }
            }
        }
    }



    class WebServiceFactory
    {

        protected static DateTime lastFailTime = DateTime.MinValue;


        public static router CreateProductServiceProxy()
        {
            //var ps = new ProductSearchWebServiceGateway();
            //var wsConfig = new OPVWebServiceConfiguration();
            //var username = wsConfig.Username;
            //var password = wsConfig.Password;

            var header = new gepirRequestHeader { requesterGln = "7350050279998", cascade = 9, cascadeSpecified = true};
            var gepirRouter = new router
                                  {
                                      UseDefaultCredentials = true,
                                      gepirRequestHeaderValue = header,
                                      Timeout = 60000
                                  };
            return gepirRouter;
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
