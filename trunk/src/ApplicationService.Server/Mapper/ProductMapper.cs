using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Gateway;
using Consumentor.ShopGun.Log;

namespace Consumentor.ShopGun.ApplicationService.Server.Mapper
{
    [Interceptor(typeof(LogInterceptor))]
    public abstract class ProductMapper<TGateway> : IMapper<Product, TGateway>
    {
        protected readonly RepositoryFactory RepositoryFactory;
        protected readonly IBrandApplicationService BrandApplicationService;
        protected readonly ICompanyApplicationService CompanyApplicationService;
        protected readonly ICountryApplicationService CountryApplicationService;
        protected readonly IIngredientApplicationService IngredientApplicationService;

        internal ProductMapper(RepositoryFactory repositoryFactory, IIngredientApplicationService ingredientApplicationService, IBrandApplicationService brandApplicationService, ICompanyApplicationService companyApplicationService, ICountryApplicationService countryApplicationService)
        {
            RepositoryFactory = repositoryFactory;
            CountryApplicationService = countryApplicationService;
            CompanyApplicationService = companyApplicationService;
            BrandApplicationService = brandApplicationService;
            IngredientApplicationService = ingredientApplicationService;
        }

        public ILogger Log { get; set; }


        public abstract TGateway Map(Product source);
        public abstract Product Map(TGateway source);

        internal Company MapCompanyByName(string companyName)
        {
            var company = CompanyApplicationService.FindCompany(companyName, true);
            if (company == null)
            {
                Log.Debug("Company not found when mapping: {0}", companyName);
                company = new Company { CompanyName = companyName, LastUpdated = DateTime.Now };
            }
            return company;
        }

        protected Brand MapBrandByName(string brandName, bool createIfNotFound)
        {
            var brand = BrandApplicationService.FindBrand(brandName, true);
            if (brand == null)
            {
                Log.Debug("Brand not found when mapping: {0}", brandName);
                brand = new Brand { BrandName = brandName, LastUpdated = DateTime.Now };
            }
            return brand;
        }

        internal IList<Ingredient> MapTableOfContentsToIngredients(string tableOfContents)
        {
            return IngredientApplicationService.FindIngredientsForTableOfContents(tableOfContents);
        }



        internal Country MapOriginCountry(string countryName)
        {
            var country = CountryApplicationService.FindCountry(countryName, true);
            if (country == null)
            {
                Log.Debug("Country not found when mapping: {0}", countryName);
                country = new Country { CountryCode = new CountryCode { Name = countryName }, LastUpdated = DateTime.Now };
            }
            return country;
        }
    }
}
