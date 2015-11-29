using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Context;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Domain.DataTransferObject;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using DataContext = Consumentor.ShopGun.Repository.DataContext;
using ProductInfo = Consumentor.ShopGun.Domain.DataTransferObject.ProductInfo;
using WebServiceBase = Consumentor.ShopGun.ApplicationService.WebService.WebServiceBase;

namespace Consumentor.ShopGun.ApplicationService.Server.WebService
{
    [ServiceBehavior(Namespace = Base.DataContractNamespace)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ShopgunAppWebService : WebServiceBase, IShopgunAppContract
    {
        private readonly ICertificationMarkDomainService _certificationMarkDomainService;
        private readonly IShopgunWebOperationContext _shopgunWebOperationContext;
        private readonly IStatisticsDomainService _statisticsDomainService;
        private readonly IAdviceSearchApplicationService _adviceSearchApplicationService;
        private readonly IMembershipProviderApplicationService _membershipProviderApplicationService;

        public ShopgunAppWebService()
        {
            _certificationMarkDomainService = Container.Resolve<ICertificationMarkDomainService>();
            _shopgunWebOperationContext = Container.Resolve<IShopgunWebOperationContext>();
            _statisticsDomainService = Container.Resolve<IStatisticsDomainService>();
            _adviceSearchApplicationService = Container.Resolve<IAdviceSearchApplicationService>();
            _membershipProviderApplicationService = Container.Resolve<IMembershipProviderApplicationService>();
        }

        public ItemInfoMessageEntity GetItemInfo(string query, string maxResults)
        {
            int max;

            if (!int.TryParse(maxResults, out max))
            {
                max = 5;
            }


            var result = _adviceSearchApplicationService.SearchListInfo("", query, max,
                                                                  _shopgunWebOperationContext);

            return result;
        }

        public ItemInfoMessageEntity GetItemInfoJson(string query, string maxResults)
        {
            int max;

            if (!int.TryParse(maxResults, out max))
            {
                max = 5;
            }

            return _adviceSearchApplicationService.SearchListInfo("", query, max,
                                                                  _shopgunWebOperationContext);
        }

        public CertificationMark GetCertificationMark(string certificationMarkId)
        {
            int id;
            if (int.TryParse(certificationMarkId, out id))
            {
                var certificationMark = _certificationMarkDomainService.GetCertificationMarkById(id);
                return certificationMark;
            }
            return null;
        }

        public CertificationMark GetCertificationMarkJson(string certificationMarkId)
        {
            return GetCertificationMark(certificationMarkId);
        }

        public IList<CertificationMark> GetCertificationMarks()
        {
            var companyApplicationService = Container.Resolve<Repository.IRepository<CertificationMark>>();
            var companies = companyApplicationService.Find(x => x != null).OrderBy(c => c.CertificationName).AsQueryable();

            return
                companies.Select(
                    c =>
                    new CertificationMark
                    {
                        Id = c.Id,
                        CertificationName = c.CertificationName,
                        CertificationMarkImageUrlSmall = c.CertificationMarkImageUrlSmall,
                        CertificationMarkImageUrlMedium = c.CertificationMarkImageUrlMedium,
                        CertificationMarkImageUrlLarge = c.CertificationMarkImageUrlLarge
                    }).ToList();
        }

        public IList<CertificationMark> GetCertificationMarksJson()
        {
            return GetCertificationMarks();
        }

        public CertificationMark GetCertificationMark2(string certificationMarkId)
        {
            return GetCertificationMark(certificationMarkId);
        }

        public CertificationMark GetCertificationMarkJson2(string certificationMarkId)
        {
            return GetCertificationMark(certificationMarkId);
        }

        public CertificationMark GetCertificationMarkJsonPost(string userSubcriptionToken, string certificationMarkId)
        {
            return GetCertificationMark(certificationMarkId);
        }

        public Mentor GetMentor(string mentorId)
        {
            int id;
            if (int.TryParse(mentorId, out id))
            {
                var mentorApplicationService = Container.Resolve<IMentorApplicationService>();
                var mentor = mentorApplicationService.GetMentorById(id);

                return mentor;
            }
            return null;
        }

        public Mentor GetMentorJson(string mentorId)
        {
            return GetMentor(mentorId);
        }

        public Mentor GetAdvisor(string mentorId)
        {
            return GetMentor(mentorId);
        }

        public Mentor GetAdvisorJson(string mentorId)
        {
            return GetMentor(mentorId);
        }

        public IList<Mentor> GetAdvisors()
        {
            var mentorApplicationService = Container.Resolve<IMentorApplicationService>();
            var mentors = mentorApplicationService.GetAllMentors().Where(m => m.IsActive).OrderBy(m => m.MentorName).ToList();

            return mentors;
        }

        public IList<Mentor> GetAdvisorsJson()
        {
            return GetAdvisors();
        }

        public Mentor GetMentorJsonPost(string userSubcriptionToken, string mentorId)
        {
            return GetMentor(mentorId);
        }

        [AddStatistics(typeof(Ingredient))]
        public Ingredient GetIngredient(string ingredientId)
        {
            int id;
            if (int.TryParse(ingredientId, out id))
            {
                var ingredientApplicationService = Container.Resolve<IIngredientApplicationService>();
                var ingredient = ingredientApplicationService.GetIngredient(id, true);
                //Todo Get AddStatisticsAttribute to work instead...
                if (ingredient != null)
                {
                    _statisticsDomainService.AddIngredienRequest(null, id, _shopgunWebOperationContext.UserAgent, _shopgunWebOperationContext.IMEI, _shopgunWebOperationContext.Model, _shopgunWebOperationContext.OsVersion);
                }
                return ingredient;
            }
            return null;
        }

        public Ingredient GetIngredientJson(string ingredientId)
        {
            return GetIngredient(ingredientId);
        }

        public IList<Product> GetProducts(string resultsPerPage, string pageIndex, string categoryId, string hasCertificationMarks, string brandId)
        {
            var dataContext = Container.Resolve<DataContext>();
            var productTable = dataContext.GetTable<Product>().OrderBy(p => p.ProductName).AsQueryable();
            var productRepository = Container.Resolve<IProductRepository>();
            var products = productRepository.Find(x => x != null).OrderBy(c => c.ProductName).AsQueryable();

            int brandIdInt;
            if (int.TryParse(brandId, out brandIdInt))
            {
                productTable = productTable.Where(p => p.BrandId == brandIdInt);
            }

            int productCategoryId;
            if (int.TryParse(categoryId, out productCategoryId))
            {
                productTable = productTable.Join(dataContext.GetTable<ProductCategory>().Where(c => c.Id == productCategoryId), p => p.ProductCategoryId,
                                                 c => c.Id, (x, y) => x);
                products = products.Where(x => x.ProductCategoryId == productCategoryId);
            }
            bool hasAnyCertificationMarks;
            if (bool.TryParse(hasCertificationMarks, out hasAnyCertificationMarks))
            {
                var productsWithCertificationMarks = productTable.Join(dataContext.GetTable<ProductCertificationMark>(), p => p.Id, cm => cm.ProductId, (x, y) => x).Distinct().OrderBy(p => p.ProductName);
                productTable = hasAnyCertificationMarks
                                   ? productsWithCertificationMarks
                                   : productTable.Except(productsWithCertificationMarks);
                products = products.Where(x => x.CertificationMarks.Any() == hasAnyCertificationMarks).Distinct();
            }
            int numItems;
            int page;
            if (int.TryParse(resultsPerPage, out numItems) && int.TryParse(pageIndex, out page))
            {
                products = products.Skip((page - 1) * numItems).Take(numItems);
                productTable = productTable.Skip((page - 1) * numItems).Take(numItems);
            }

            var result = from c in productTable.ToList()
                         select new Product
                                    {
                                        Id = c.Id,
                                        ProductName = c.ProductName,
                                        ImageUrlSmall = c.ImageUrlSmall,
                                        ImageUrlMedium = c.ImageUrlMedium,
                                        ImageUrlLarge = c.ImageUrlLarge,
                                        LastUpdated = c.LastUpdated,
                                        Quantity = c.Quantity,
                                        QuantityUnit = c.QuantityUnit,
                                        CertificationMarks =
                                            c.CertificationMarks.Select(
                                                x =>
                                                new CertificationMark
                                                    {
                                                        Id = x.Id,
                                                        CertificationMarkImageUrlMedium =
                                                            x.CertificationMarkImageUrlMedium,
                                                        CertificationName = x.CertificationName
                                                    }).ToList(),
                                        Brand =
                                            c.Brand != null
                                                ? new Brand
                                                      {
                                                          Id = c.Brand.Id,
                                                          BrandName = c.Brand.BrandName,
                                                          LastUpdated = c.LastUpdated
                                                      }
                                                : null
                                    };

            return result.ToList();
            

            //return
            //    products.Select(
            //        c =>
            //        new Product
            //            {
            //                Id = c.Id,
            //                ProductName = c.ProductName,
            //                ImageUrlSmall = c.ImageUrlSmall,
            //                ImageUrlMedium = c.ImageUrlMedium,
            //                ImageUrlLarge = c.ImageUrlLarge,
            //                LastUpdated = c.LastUpdated,
            //                CertificationMarks =
            //                    c.CertificationMarks.Select(
            //                        x =>
            //                        new CertificationMark
            //                            {
            //                                Id = x.Id,
            //                                CertificationMarkImageUrlMedium = x.CertificationMarkImageUrlMedium,
            //                                CertificationName = x.CertificationName
            //                            }).ToList()
            //            }).ToList();
        }

        public IList<Product> GetProductsJson(string resultsPerPage, string pageIndex, string categoryId, string hasCertificationMarks, string brandId)
        {
            return GetProducts(resultsPerPage, pageIndex, categoryId, hasCertificationMarks, brandId);
        }

        public Product GetProduct(string productId)
        {
            int id;
            if (int.TryParse(productId, out id))
            {
                var productApplicationService = Container.Resolve<IProductApplicationService>();
                var product = productApplicationService.GetMergedProduct(id);
                if (product != null)
                {
                    _statisticsDomainService.AddProductRequest(null, id, _shopgunWebOperationContext.UserAgent, _shopgunWebOperationContext.IMEI, _shopgunWebOperationContext.Model, _shopgunWebOperationContext.OsVersion);
                }
                return product;
            }
            return null;
        }

        public Product GetProductJson(string productId)
        {
            return GetProduct(productId);
        }

        public Product GetProductByGtin(string gtin)
        {
            var productApplicationService = Container.Resolve<IProductApplicationService>();
            var product = productApplicationService.FindProductByGtin(gtin, true);
            if (product != null && product.Id != 0)
            {
                _statisticsDomainService.AddProductRequest(null, product.Id, _shopgunWebOperationContext.UserAgent, _shopgunWebOperationContext.IMEI, _shopgunWebOperationContext.Model, _shopgunWebOperationContext.OsVersion);
            }
            return product;
        }

        public Product GetProductGtinJson(string gtin)
        {
            return GetProductByGtin(gtin);
        }

        public ProductInfo GetProductInfoByGtin(string gtin)
        {
            var productApplicationService = Container.Resolve<IProductApplicationService>();
            var product = productApplicationService.FindProductByGtin(gtin, true);
            if (product != null && product.Id != 0)
            {
                _statisticsDomainService.AddProductRequest(null, product.Id, _shopgunWebOperationContext.UserAgent, _shopgunWebOperationContext.IMEI, _shopgunWebOperationContext.Model, _shopgunWebOperationContext.OsVersion);
                var productInfo = new ProductInfo
                                      {
                                          Id = product.Id,
                                          Name = product.ProductName,
                                          GTIN = product.GlobalTradeItemNumber,
                                          BrandId = product.Brand != null ? product.Brand.Id : 0,
                                          BrandName = product.Brand != null ? product.Brand.BrandName : "",
                                          CompanyId =
                                              product.Brand != null ? product.Brand.CompanyId.GetValueOrDefault() : 0,
                                          CompanyName =
                                              product.Brand != null && product.Brand.Owner != null
                                                  ? product.Brand.Owner.CompanyName
                                                  : "",
                                          AdviceInfos = product.ProductAdvices.Select(x => new AdviceInfo
                                                                                               {
                                                                                                   AdviceId = x.Id.Value,
                                                                                                   AdviceIntroduction = x.Introduction,
                                                                                                   AdviceTag = x.Tag.Name,
                                                                                                   AdviceType = "ProductAdvice",
                                                                                                   AdviceableEntityId = x.ProductsId.Value,
                                                                                                   AdviceableEntityName = x.ItemName,
                                                                                                   AdvisorId = x.MentorId.Value,
                                                                                                   AdvisorName = x.Mentor.MentorName,
                                                                                                   Semaphore = x.Semaphore.ColorName
                                                                                               }).ToList()
                                      };
                if (product.Brand != null)
                {
                    productInfo.AdviceInfos.AddRange(product.Brand.BrandAdvices.Select(x => new AdviceInfo
                                                                                                {
                                                                                                    AdviceId = x.Id.Value,
                                                                                                    AdviceIntroduction =
                                                                                                        x.Introduction,
                                                                                                    AdviceTag = x.Tag.Name,
                                                                                                    AdviceType =
                                                                                                        "BrandAdvice",
                                                                                                    AdviceableEntityId = x.BrandsId.Value,
                                                                                                    AdviceableEntityName = x.ItemName,
                                                                                                    AdvisorId = x.MentorId.Value,
                                                                                                    AdvisorName = x.Mentor.MentorName,
                                                                                                    Semaphore = x.Semaphore.ColorName
                                                                                                }));
                    if (product.Brand.Owner != null)
                    {
                        productInfo
                            .AdviceInfos
                            .AddRange(product.Brand.Owner.CompanyAdvices
                                          .Select(x => new AdviceInfo
                                                           {
                                                               AdviceId = x.Id.Value,
                                                               AdviceIntroduction =
                                                                   x.Introduction,
                                                               AdviceTag = x.Tag.Name,
                                                               AdviceType =
                                                                   "CompanyAdvice",
                                                               AdviceableEntityId =
                                                                   x.CompanysId.Value,
                                                               AdviceableEntityName =
                                                                   x.ItemName,
                                                               AdvisorId =
                                                                   x.MentorId.Value,
                                                               AdvisorName =
                                                                   x.Mentor.MentorName,
                                                               Semaphore =
                                                                   x.Semaphore.ColorName
                                                           }));
                    }
                }
                productInfo.AdviceInfos.AddRange(
                    product.Ingredients.SelectMany(x => x.IngredientAdvices).Select(x => new AdviceInfo
                                                                                             {
                                                                                                 AdviceId =
                                                                                                     x.Id.Value,
                                                                                                 AdviceIntroduction
                                                                                                     =
                                                                                                     x.
                                                                                                     Introduction,
                                                                                                 AdviceTag =
                                                                                                     x.Tag.Name,
                                                                                                 AdviceType =
                                                                                                     "IngredientAdvice",
                                                                                                 AdviceableEntityId = x.IngredientsId.Value,
                                                                                                 AdviceableEntityName = x.ItemName,
                                                                                                 AdvisorId = x.MentorId.Value,
                                                                                                 AdvisorName = x.Mentor.MentorName,
                                                                                                 Semaphore = x.Semaphore.ColorName
                                                                                             }));
                return productInfo;
            }
            return null;
        }

        public ProductInfo GetProductInfoGtinJson(string gtin)
        {
            return GetProductInfoByGtin(gtin);
        }

        public IList<Brand> GetBrands(string resultsPerPage, string pageIndex)
        {
            return GetBrands("", resultsPerPage, pageIndex);
        }

        public IList<Brand> GetBrandsJson(string resultsPerPage, string pageIndex)
        {
            return GetBrands("", resultsPerPage, pageIndex);
        }

        public IList<Brand> GetBrands(string companyId, string resultsPerPage, string pageIndex)
        {
            var brandRepository = Container.Resolve<Repository.IRepository<Brand>>();
            var brands = brandRepository.Find(x => x != null).OrderBy(c => c.BrandName).AsQueryable();
            int compId;
            if (int.TryParse(companyId, out compId))
            {
                brands = brands.Where(x => x.CompanyId == compId);
            }
            int pages;
            int page;
            if (int.TryParse(resultsPerPage, out pages) && int.TryParse(pageIndex, out page))
            {
                brands = brands.Skip((page - 1) * pages).Take(pages);
            }

            return
                brands.Select(
                    c =>
                    new Brand
                    {
                        Id = c.Id,
                        BrandName = c.BrandName,
                        LogotypeUrl = c.LogotypeUrl,
                        LastUpdated = c.LastUpdated,
                        IsMember = c.IsMember
                    }).ToList();
        }

        public IList<Brand> GetBrandsJson(string companyId, string resultsPerPage, string pageIndex)
        {
            return GetBrands(companyId, resultsPerPage, pageIndex);
        }

        public Brand GetBrand(string brandId)
        {
            int id;
            if (int.TryParse(brandId, out id))
            {
                var brandApplicationService = Container.Resolve<IBrandApplicationService>();
                var brand = brandApplicationService.GetBrand(id, true);
                if (brand != null)
                {
                    _statisticsDomainService.AddBrandRequest(null, id, _shopgunWebOperationContext.UserAgent, _shopgunWebOperationContext.IMEI, _shopgunWebOperationContext.Model, _shopgunWebOperationContext.OsVersion);
                }
                return brand;
            }
            return null;
        }

        public Brand GetBrandJson(string brandId)
        {
            return GetBrand(brandId);
        }

        public IList<Company> GetCompanies(string isMember, string resultsPerPage, string pageIndex)
        {

            var companyApplicationService = Container.Resolve<Repository.IRepository<Company>>();
            var companies = companyApplicationService.Find(x => x != null).OrderBy(c => c.CompanyName).AsQueryable();
            bool isMemberBool;
            if (bool.TryParse(isMember, out isMemberBool))
            {
                companies = companies.Where(x => x.IsMember == isMemberBool);
            }
            int pages;
            int page;
            if (int.TryParse(resultsPerPage, out pages) && int.TryParse(pageIndex, out page))
            {
                companies = companies.Skip((page - 1) * pages).Take(pages);
            }

            return
                companies.Select(
                    c =>
                    new Company
                        {
                            Id = c.Id,
                            CompanyName = c.CompanyName,
                            ImageUrlLarge = c.ImageUrlLarge,
                            ImageUrlMedium = c.ImageUrlMedium,
                            IsMember = c.IsMember,
                            LastUpdated = c.LastUpdated
                        }).ToList();
        }

        public IList<Company> GetCompaniesJson(string isMember, string resultsPerPage, string pageIndex)
        {
            return GetCompanies(isMember, resultsPerPage, pageIndex);
        }

        public Company GetCompany(string companyId)
        {
            int id;
            if (int.TryParse(companyId, out id))
            {
                var companyApplicationService = Container.Resolve<ICompanyApplicationService>();
                var company = companyApplicationService.GetCompany(id, true);
                if (company != null)
                {
                    _statisticsDomainService.AddCompanyRequest(null, id, _shopgunWebOperationContext.UserAgent, _shopgunWebOperationContext.IMEI, _shopgunWebOperationContext.Model, _shopgunWebOperationContext.OsVersion);
                }
                return company;
            }
            return null;
        }

        public Company GetCompanyJson(string companyId)
        {
            return GetCompany(companyId);
        }

        public CompanyDetails GetCompanyDetails(string companyId)
        {
            var company = GetCompany(companyId);
            return company != null ? company.ToCompanyDto() : null;
        }

        public CompanyDetails GetCompanyDetailsJson(string companyId)
        {
            return GetCompanyDetails(companyId);
        }

        public Tip GetRandomTip()
        {
            var tipApplicationService = Container.Resolve<ITipApplicationService>();
            var tip = tipApplicationService.GetRandomTip();
            return tip;
        }

        public Tip GetRandomTipJson()
        {
            return GetRandomTip();
        }

        public AdviceBase GetAdvice(string adviceId)
        {   
            int id;
            if (int.TryParse(adviceId, out id))
            {
                var adviceApplicationService = Container.Resolve<IAdviceApplicationService>();
                var advice = adviceApplicationService.GetAdvice(id);
                _statisticsDomainService.AddAdviceRequest(null, id, _shopgunWebOperationContext.UserAgent, _shopgunWebOperationContext.IMEI, _shopgunWebOperationContext.Model, _shopgunWebOperationContext.OsVersion);
                return advice;
            }
            return null;
        }

        public AdviceBase GetAdviceJson(string adviceId)
        {
            return GetAdvice(adviceId);
        }

        public IList<UserAdviceRating> GetUserAdviceRatings(string adviceId)
        {
            var parameters = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;
            User user = null;
            if (parameters.AllKeys.Contains("authtoken"))
            {
                var authToken = parameters["authtoken"];
                user = _membershipProviderApplicationService.GetUserForToken(authToken);
                if (user == null)
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
                }
            }

            var advice = GetAdvice(adviceId);
            var adviceRatings = advice.UserAdviceRatings.AsQueryable();
            if (user != null)
            {
                adviceRatings = adviceRatings.Where(x => x.UserId == user.Id);
            }

            return adviceRatings.ToList();
        }

        public IList<UserAdviceRating> GetUserAdviceRatingsJson(string adviceId)
        {
            return GetUserAdviceRatings(adviceId);
        }

        public UserAdviceRating AddUserAdviceRating(UserAdviceRating userAdviceRating)
        {
            var parameters = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;
            if (parameters.AllKeys.Contains("authtoken"))
            {
                var authToken = parameters["authtoken"];
                var user = _membershipProviderApplicationService.GetUserForToken(authToken);

                if (user != null)
                {
                    var newRating = userAdviceRating.Rating;
                    var adviceRepository = Container.Resolve<IAdviceRepository>();
                    var advice = adviceRepository.FindOne(x => x.Id == userAdviceRating.AdviceId);
                    if (advice.UserAdviceRatings.Any(x => x.UserId == user.Id))
                    {
                        var existingUserAdvcieRating = advice.UserAdviceRatings.First(x => x.UserId == user.Id);
                        existingUserAdvcieRating.Rating = newRating;
                    }
                    else
                    {
                        userAdviceRating.UserId = user.Id;
                        advice.UserAdviceRatings.Add(userAdviceRating);
                    }
                    adviceRepository.Persist();
                }
                else
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
                }
            }
            return userAdviceRating;
        }

        public int GetAdviceRatingJson(string adviceId, string deviceId)
        {
            var advice = GetAdvice(adviceId);
            var adviceRatings = advice.UserAdviceRatings.AsQueryable();
            adviceRatings = adviceRatings.Where(x => x.UserId == 1);
            if (adviceRatings.Any(x => x.DeviceId == deviceId))
            {
                return adviceRatings.First(x => x.DeviceId == deviceId).Rating;
            }
            return 0;
        }

        public void SetAdviceRating(string adviceId, string deviceId, string rating)
        {
            var newRating = int.Parse(rating);
            var adviceRepository = Container.Resolve<IAdviceRepository>();
            var advice = adviceRepository.FindOne(x => x.Id == int.Parse(adviceId));
            if (advice.UserAdviceRatings.Any(x => x.UserId == 1))
            {
                var existingUserAdvcieRating = advice.UserAdviceRatings.First(x => x.UserId == 1 && x.DeviceId == deviceId);
                existingUserAdvcieRating.Rating = newRating;
            }
            else
            {
                advice.UserAdviceRatings.Add(new UserAdviceRating{UserId= 1, DeviceId = deviceId, Rating = newRating});
            }
            adviceRepository.Persist();
        }
    }
}
