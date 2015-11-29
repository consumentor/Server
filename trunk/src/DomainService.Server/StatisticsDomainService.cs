using System;
using Castle.Core;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class StatisticsDomainService : IStatisticsDomainService
    {
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Ingredient> _ingredientRepository;
        private readonly IRepository<Concept> _conceptRepository;
        private readonly IRepository<AdviceBase> _adviceRepository; 

        public StatisticsDomainService(IRepository<Brand> brandRepository,
                IRepository<Company> companyRepository,
                IRepository<Country> countryRepository,
                IRepository<Product> productRepository,
                IRepository<Ingredient> ingredientRepository,
                IRepository<Concept> conceptRepository, 
                IRepository<AdviceBase> adviceRepository)
        {
            _brandRepository = brandRepository;
            _companyRepository = companyRepository;
            _countryRepository = countryRepository;
            _productRepository = productRepository;
            _ingredientRepository = ingredientRepository;
            _conceptRepository = conceptRepository;
            _adviceRepository = adviceRepository;
        }

        #region IAdviceStatisticsDomainService Members

        public void AddStatistics(User user, SearchResultMessageEntity messageEntity, string userAgent, string imei, string model, string osVersion)
        {
            var insertTime = DateTime.Now;

            int? userId = user != null ? user.Id : (int?) null;

            if (messageEntity.Products != null)
            {
                
                foreach (var product in messageEntity.Products)
                {
                    AddProductRequest(userId, product.Id, userAgent, imei, model, osVersion);   
                }
            }

            if (messageEntity.Ingredients != null)
            {
                
                foreach (var ingredient in messageEntity.Ingredients)
                {
                    AddIngredienRequest(userId, ingredient.Id, userAgent, imei, model, osVersion);
                }
            }

            if (messageEntity.Brands != null)
            {
                foreach (var brand in messageEntity.Brands)
                {
                    AddBrandRequest(userId, brand.Id, userAgent, imei, model, osVersion);
                }
            }

            if (messageEntity.Companies != null)
            {
                
                foreach (var company in messageEntity.Companies)
                {
                    AddCompanyRequest(userId, company.Id, userAgent, imei, model, osVersion);
                }
            }

            if (messageEntity.Countries != null)
            {

                foreach (var country in messageEntity.Countries)
                {
                    AddCountryRequest(userId, country.Id, userAgent, imei, model, osVersion);
                }
            }

            if (messageEntity.Concepts != null)
            {

                foreach (var concept in messageEntity.Concepts)
                {
                    AddConceptRequest(userId, concept.Id, userAgent, imei, model, osVersion);
                }
            }
        }

        public void AddAdviceRequest(User user, AdviceBase advice, string userAgent, string imei, string model, string osVersion)
        {
            throw new NotImplementedException();
        }

        public void AddIngredienRequest(int? userId, int ingredientId, string userAgent, string imei, string model, string osVersion)
        {
            var ingredient = _ingredientRepository.FindOne(i => i.Id == ingredientId);
            ingredient.IngredientStatistics.Add(new IngredientStatistic{UserId = userId, Imei = imei, Model = model, OsVersion = osVersion, UserAgent = userAgent, Timestamp = DateTime.Now});
            _ingredientRepository.Persist();
        }

        public void AddProductRequest(int? userId, int productId, string userAgent, string imei, string model, string osVersion)
        {
            var product = _productRepository.FindOne(i => i.Id == productId);
            product.ProductStatistics.Add(new ProductStatistic { UserId = userId, Imei = imei, Model = model, OsVersion = osVersion, UserAgent = userAgent, Timestamp = DateTime.Now });
            _productRepository.Persist();
        }

        public void AddBrandRequest(int? userId, int brandId, string userAgent, string imei, string model, string osVersion)
        {
            var brand = _brandRepository.FindOne(i => i.Id == brandId);
            brand.BrandStatistics.Add(new BrandStatistic { UserId = userId, Imei = imei, Model = model, OsVersion = osVersion, UserAgent = userAgent, Timestamp = DateTime.Now });
            _brandRepository.Persist();
        }

        public void AddCompanyRequest(int? userId, int companyId, string userAgent, string imei, string model, string osVersion)
        {
            var company = _companyRepository.FindOne(i => i.Id == companyId);
            company.CompanyStatistics.Add(new CompanyStatistic { UserId = userId, Imei = imei, Model = model, OsVersion = osVersion, UserAgent = userAgent, Timestamp = DateTime.Now });
            _companyRepository.Persist();
        }

        public void AddCountryRequest(int? userId, int countryId, string userAgent, string imei, string model, string osVersion)
        {
            var country = _countryRepository.FindOne(i => i.Id == countryId);
            country.CountryStatistics.Add(new CountryStatistic { UserId = userId, Imei = imei, Model = model, OsVersion = osVersion, UserAgent = userAgent, Timestamp = DateTime.Now });
            _countryRepository.Persist();
        }

        public void AddConceptRequest(int? userId, int conceptId, string userAgent, string imei, string model, string osVersion)
        {
            var concept = _conceptRepository.FindOne(i => i.Id == conceptId);
            concept.ConceptStatistics.Add(new ConceptStatistic { UserId = userId, Imei = imei, Model = model, OsVersion = osVersion, UserAgent = userAgent, Timestamp = DateTime.Now });
            _conceptRepository.Persist();
        }

        public void AddAdviceRequest(int? userId, int adviceId, string userAgent, string imei, string model, string osVersion)
        {
            var advice = _adviceRepository.FindOne(x => x.Id == adviceId);
            advice.AdviceRequestStatistics.Add(new AdviceRequestStatistic { UserId = userId, Imei = imei, Model = model, OsVersion = osVersion, UserAgent = userAgent, Timestamp = DateTime.Now });
            _adviceRepository.Persist();
        }

        #endregion
    }
}
