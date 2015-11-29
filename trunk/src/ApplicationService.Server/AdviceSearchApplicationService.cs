using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Context;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Domain.Extensions;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class AdviceSearchApplicationService : IAdviceSearchApplicationService
    {
        public ILogger Log { get; set; }

        private readonly IProductApplicationService _productApplicationService;

        private readonly RepositoryFactory _repositoryFactory;
        private readonly IBrandApplicationService _brandApplicationService;
        private readonly ICompanyApplicationService _companyApplicationService;
        private readonly ICountryApplicationService _countryApplicationService;
        private readonly IIngredientApplicationService _ingredientApplicationService;
        private readonly IConceptApplicationService _conceptApplicationService;

        private readonly ISearchStatisticsDomainService _adviceSearchStatisticsDomainService;
        private readonly IStatisticsDomainService _statisticsDomainService;

        public AdviceSearchApplicationService(
            IProductApplicationService productApplicationService, 
            IBrandApplicationService brandApplicationService, 
            ICompanyApplicationService companyApplicationService, 
            ICountryApplicationService countryApplicationService, 
            IIngredientApplicationService ingredientApplicationService, 
            IConceptApplicationService conceptApplicationService,
            ISearchStatisticsDomainService adviceSearchStatisticsDomainService,
            IStatisticsDomainService statisticsDomainService, RepositoryFactory repositoryFactory)
        {
            _conceptApplicationService = conceptApplicationService;
            _repositoryFactory = repositoryFactory;
            _ingredientApplicationService = ingredientApplicationService;
            _countryApplicationService = countryApplicationService;
            _companyApplicationService = companyApplicationService;
            _brandApplicationService = brandApplicationService;

            _adviceSearchStatisticsDomainService = adviceSearchStatisticsDomainService;
            _statisticsDomainService = statisticsDomainService;

            _productApplicationService = productApplicationService;
        }

        public SearchResultMessageEntity Search(string userSubcriptionToken, string queryString, IShopgunWebOperationContext shopgunWebOperationContext)
        {
            //TODO identify User
            User user = null;

            ////TODO Check for Subscription token, is valid. Maybe it this shall been done the webservice?
            
            SearchResultMessageEntity result;
            bool productExistsInDatabase = false;

            if (queryString.IsGtin())
            {
                Log.Debug("GTIN search for {0}", queryString);
                var product = _productApplicationService.FindProductByGtin(queryString, true);
                //var product = _productApplicationService.FindProductByGtinInOwnDatabase(queryString, true);

                result = new SearchResultMessageEntity();
                if (!string.IsNullOrEmpty(product.ProductName))
                {
                    result.SearchType = SearchResultMessageEntity.GtinSearch;
                    result.Products = new List<Product> {product};
                }
                else if (product.Brand != null)
                {
                    if (!string.IsNullOrEmpty(product.Brand.BrandName))
                    {
                        result.SearchType = SearchResultMessageEntity.FreeSearch;
                        result.Brands = new List<Brand> {product.Brand};
                    }
                    else if (product.Brand.Owner != null)
                    {
                        result.SearchType = SearchResultMessageEntity.FreeSearch;
                        result.Companies = new List<Company> {product.Brand.Owner};
                    }
                }
                if (product.Id > 0)
                {
                    productExistsInDatabase = true;
                }
            }
            else
            {
                Log.Debug("Freetext search for \"{0}\"", queryString);
                result =
                    new SearchResultMessageEntity
                        {
                            SearchType = SearchResultMessageEntity.FreeSearch
                            ,
                            //Brands = _brandApplicationService.FindBrands(queryString, true)
                            //,
                            //Companies = _companyApplicationService.FindCompanies(queryString, true)
                            //,
                            //Concepts = _conceptApplicationService.FindConcepts(queryString, true)
                            //,
                            //Countries = _countryApplicationService.FindCountries(queryString, true)
                            //,
                            //Ingredients = _ingredientApplicationService.FindIngredients(queryString, true, true)
                    };
                //Only exact match for now...
                var ingredient = _ingredientApplicationService.FindIngredient(queryString, true, true);
                if (ingredient != null)
                {
                    result.Ingredients = new List<Ingredient> { ingredient };
                }
                var company = _companyApplicationService.FindCompany(queryString, true);
                if(ingredient == null && company != null)
                {
                    result.Companies = new List<Company> { company } ;
                    result.SearchType = SearchResultMessageEntity.FreeSearch;
                }

                //if (!result.HasResults())
                //{
                //    result.Products = _productDomainService.FindProducts(queryString, true);
                //}
            }
            var resultFound = result.HasResults();

            if (shopgunWebOperationContext != null)
            {
                var userAgent = shopgunWebOperationContext.UserAgent;
                var imei = shopgunWebOperationContext.IMEI;
                var model = shopgunWebOperationContext.Model;
                var osVersion = shopgunWebOperationContext.OsVersion;

                _adviceSearchStatisticsDomainService.AddAdviceSearch(user, queryString, userAgent, imei, model, resultFound, osVersion);
            }

            if (result.SearchType != SearchResultMessageEntity.GtinSearch || productExistsInDatabase)
            {
                //_statisticsDomainService.AddStatistics(user, result, userAgent, imei, model, osVersion);
            }

            return result;
        }

        public ItemInfoMessageEntity SearchItems(string userSubscriptionToken, string queryString, int[] maxHitsPerCategory, IShopgunWebOperationContext shopgunWebOperationContext)
        {
            var result = new ItemInfoMessageEntity();

            var queryStrings = queryString.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);

            using (var productRepository = _repositoryFactory.Build<IRepository<Product>, Product>())
            {
                result.Products =
                    (from p in
                         productRepository.Find(
                             x => queryStrings.All(q => (x.ProductName + " " + x.Brand.BrandName).ToLower().Contains(q)))
                    orderby p.ProductName
                                   select new ProductInfo
                                   {
                                       ProductId = p.Id,
                                       ProductName = p.ProductName,
                                       BrandName = p.Brand.BrandName,
                                       CompanyName = p.Brand.Owner != null ? p.Brand.Owner.CompanyName : ""
                                       ,
                                       NumberAdvices =
                                           p.ProductAdvices.Count(x => x.Published) +
                                           _ingredientApplicationService.FindIngredientsForTableOfContents(
                                               p.TableOfContent).SelectMany(
                                                   x => x.IngredientAdvices.Where(a => a.Published)).Count()
                                   }).Take(maxHitsPerCategory[0]).ToList();
            }

            return result;
        }

        public ItemInfoMessageEntity SearchListInfo(string userSubscriptionToken, string queryString, int maxNumHitsPerCategory, IShopgunWebOperationContext shopgunWebOperationContext)
        {
            var result = new ItemInfoMessageEntity();

            using (var productRepository = _repositoryFactory.Build<IRepository<Product>, Product>())
            {
                // Search by productname
                var productHits = (from p in productRepository.Find(x => x.ProductName.StartsWith(queryString))
                                   orderby p.ProductName
                                   select new ProductInfo
                                              {
                                                  ProductId = p.Id,
                                                  ProductName = p.ProductName,
                                                  GTIN = p.GlobalTradeItemNumber,
                                                  BrandName = p.Brand.BrandName,
                                                  CompanyName = p.Brand.Owner != null ? p.Brand.Owner.CompanyName : "",
                                                  NumberAdvices = 0,
                                                      //p.ProductAdvices.Count(x => x.Published) +
                                                      //_ingredientApplicationService.FindIngredientsForTableOfContents(
                                                      //    p.TableOfContent).SelectMany(
                                                      //        x => x.IngredientAdvices.Where(a => a.Published)).Count()
                                              }).Take(maxNumHitsPerCategory).ToList();

                // Search by GTIN
                productHits.AddRange(
                    (from p in productRepository.Find(x => x.GlobalTradeItemNumber.StartsWith(queryString))
                     orderby p.ProductName
                     select new ProductInfo
                                {
                                    ProductId = p.Id,
                                    ProductName = p.ProductName,
                                    GTIN = p.GlobalTradeItemNumber,
                                    BrandName = p.Brand.BrandName,
                                    CompanyName = p.Brand.Owner != null ? p.Brand.Owner.CompanyName : "",
                                    NumberAdvices = 0
                                }).Take(maxNumHitsPerCategory).ToList());

                result.Products = productHits.OrderBy(x => x.ProductName).ToList();
            }

            using (var ingredientRepository = _repositoryFactory.Build<IRepository<Ingredient>, Ingredient>())
            {
                // search in ingredients
                var ingredients = ingredientRepository.Find(x => x.IngredientName.StartsWith(queryString));
                var searchResult = (from i in ingredients
                                    orderby i.IngredientName
                                    select new IngredientInfo
                                               {
                                                   IngredientId = i.Id,
                                                   IngredientName = i.IngredientName,
                                                   NumberAdvices = 0,//CountIngredientAdvicesRecursively(ingredientRepository, i)
                                               }).Take(maxNumHitsPerCategory).ToList();

                using (var alternativeNameRepository = _repositoryFactory.Build<IRepository<AlternativeIngredientName>, AlternativeIngredientName>())
                {
                    //search in AlternativeIngredientNames but leave out those which were already added by ingredient search
                    var alternativeNameIngredients =
                        (from altNameIngredient in
                             alternativeNameRepository.Find(x => x.AlternativeName.StartsWith(queryString))
                         where !ingredients.Contains(altNameIngredient.Ingredient)
                         select new IngredientInfo
                                    {
                                        IngredientId = altNameIngredient.Ingredient.Id,
                                        IngredientName = altNameIngredient.AlternativeName,
                                        NumberAdvices = 0,//CountIngredientAdvicesRecursively(ingredientRepository, altNameIngredient.Ingredient)//altNameIngredient.Ingredient.IngredientAdvices.Where(x => x.Published).Count()
                                    }).Take(maxNumHitsPerCategory).ToList();
                    searchResult.AddRange(alternativeNameIngredients);
                }
                // reorder, take max amount and set result
                result.Ingredients = searchResult.OrderBy(x => x.IngredientName).Take(maxNumHitsPerCategory).ToList();
            }

            using (var brandRepository = _repositoryFactory.Build<IRepository<Brand>, Brand>())
            {
                result.Brands = (from b in brandRepository.Find(x => x.BrandName.StartsWith(queryString))
                                 orderby b.BrandName
                                 select
                                     new BrandInfo
                                         {
                                             BrandId = b.Id,
                                             BrandName = b.BrandName,
                                             CompanyName = b.Owner != null ? b.Owner.CompanyName : "",
                                             NumberAdvices = 0,//b.BrandAdvices.Count(x => x.Published)
                                         }).Take(maxNumHitsPerCategory).ToList();
            }

            using (var companyRepository = _repositoryFactory.Build<IRepository<Company>, Company>())
            {
                result.Companies = (from c in companyRepository.Find(x => x.CompanyName.StartsWith(queryString))
                                    orderby c.CompanyName
                                    select
                                        new CompanyInfo
                                            {
                                                CompanyId = c.Id,
                                                CompanyName = c.CompanyName,
                                                NumberAdvices = 0,//c.CompanyAdvices.Count(x => x.Published)
                                            }).Take(
                                                maxNumHitsPerCategory).ToList();
            }

            using (var countryRepository = _repositoryFactory.Build<IRepository<Country>, Country>())
            {
                result.Countries = (from c in countryRepository.Find(x => x.CountryCode.Name.StartsWith(queryString))
                                    orderby c.CountryCode.Name
                                    select
                                        new CountryInfo
                                            {
                                                CountryId = c.Id,
                                                CountryName = c.CountryCode.Name,
                                                NumberAdvices = 0,//c.CountryAdvices.Count(x => x.Published)
                                            }).Take(
                                                maxNumHitsPerCategory).ToList();
            }

            using (var conceptRepository = _repositoryFactory.Build<IRepository<Concept>, Concept>())
            {
                result.Concepts = (from c in conceptRepository.Find(x => x.ConceptTerm.StartsWith(queryString))
                                   orderby c.ConceptTerm
                                   select
                                       new ConceptInfo
                                           {
                                               ConceptId = c.Id,
                                               ConceptName = c.ConceptTerm,
                                               NumberAdvices = 0,//c.ConceptAdvices.Count(x => x.Published)
                                           }).Take(
                                               maxNumHitsPerCategory).ToList();
            }

            return result;    
        }

        private int CountIngredientAdvicesRecursively(IRepository<Ingredient> ingredientRepository, Ingredient ingredient)
        {
            var count = ingredient.IngredientAdvices.Count(x => x.Published);
            if (ingredient.ParentId.HasValue)
            {
                var parentIngredient = ingredientRepository.FindOne(x => x.Id == ingredient.ParentId);
                count +=
                    CountIngredientAdvicesRecursively(ingredientRepository, parentIngredient);
            }
            return count;
        }
    }
}
