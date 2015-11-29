using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.DomainService.Server;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class BrandApplicationService : IBrandApplicationService
    {
        public ILogger Log { get; set; }
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IBrandRepository _brandRepository;

        public BrandApplicationService(RepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _brandRepository = _repositoryFactory.Build<IBrandRepository, Brand>();
        }

        public Brand CreateBrand(Brand brandToCreate)
        {
            var existingBrand = FindBrand(brandToCreate.BrandName, false);
            if (existingBrand != null)
            {
                Log.Debug("Tried to create a brand that already exists: {0} with Id {1}", existingBrand.BrandName, existingBrand.Id);
                throw new ArgumentException("Can't create two brands with the same name");
            }
            brandToCreate.LastUpdated = DateTime.Now;
            SetOwner(brandToCreate, brandToCreate.CompanyId);
            _brandRepository.Add(brandToCreate);
            _brandRepository.Persist();
            Log.Debug("Brand {0} created with Id {1}", brandToCreate.BrandName, brandToCreate.Id);
            return brandToCreate;
        }

        public Brand GetBrand(int brandId)
        {
            return _brandRepository.FindOne(x => x.Id == brandId);
        }

        public Brand GetBrand(int brandId , bool onlyPublishedAdvices)
        {
            var brand = _brandRepository.FindOne(x => x.Id == brandId);
            if (onlyPublishedAdvices)
            {
                brand.BrandAdvices = brand.BrandAdvices.Where(a => a.Published).OrderBy(x => x.Semaphore.Value).ToList();
                if (brand.Owner != null)
                {
                    brand.Owner.CompanyAdvices = brand.Owner.CompanyAdvices.Where(a => a.Published).OrderBy(x => x.Semaphore.Value).ToList();
                }
            }
            return brand;
        }

        public IList<Brand> GetAllBrands()
        {
            return _brandRepository.Find(x => x != null).OrderBy(x => x.BrandName).ToList();
        }

        public IList<Brand> GetBrandsWithAdvicesByMentor(int mentorId)
        {
            var brands = from brand in _brandRepository.Find(x => x != null)
                         where brand.BrandAdvices.Any(x => x.Mentor.Id == mentorId)
                         select brand;

            return brands.OrderBy(x => x.BrandName).ToList();
        }

        public Brand UpdateBrand(Brand updatedBrand)
        {
            var brandToUpdate = _brandRepository.FindOne(b => b.Id == updatedBrand.Id);
            SetOwner(brandToUpdate, updatedBrand.CompanyId);
            brandToUpdate.BrandName = updatedBrand.BrandName;
            brandToUpdate.LastUpdated = DateTime.Now;
            _brandRepository.Persist();
            Log.Debug("Brand {0} with Id {1} updated", brandToUpdate.BrandName, brandToUpdate.Id);
            return brandToUpdate;
        }

        /// <summary>
        /// Only deletes the brand if there are no references that depend on it.
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        public bool DeleteBrand(int brandId)
        {
            try
            {
                var brandToDelete = _brandRepository.FindOne(x => x.Id == brandId);
                _brandRepository.Delete(brandToDelete);
                _brandRepository.Persist();
                Log.Debug("Brand '{0}' with Id '{1}' deleted.", brandToDelete.BrandName, brandToDelete.Id);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Failed to delete brand with Id '{0}'", brandId), e);
                return false;
            }
        }

        /// <summary>
        /// Deletes <code>Brand</code> with id <paramref name="brandId"/>.
        /// If <paramref name="substitutingBrandId"/> is not <code>0</code> all references that depend on the brand to be deleted will be associated with the substituting brand.
        /// In case such references exist and <paramref name="substitutingBrandId"/> is 0, <paramref name="brandId"/> can't be deleted.
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="substitutingBrandId"></param>
        /// <returns></returns>
        public bool DeleteBrand(int brandId, int? substitutingBrandId)
        {
            if (substitutingBrandId == null)
            {
                return DeleteBrand(brandId);
            }
            var brandToDelete = _brandRepository.FindOne(x => x.Id == brandId);
            var substitutingBrand = _brandRepository.FindOne(x => x.Id == substitutingBrandId);

            foreach (var product in brandToDelete.Products)
            {
                product.Brand = substitutingBrand;
            }

            foreach (var brandAdvice in brandToDelete.BrandAdvices)
            {
                substitutingBrand.AddAdvice(brandAdvice);
            }
            foreach (var brandStatistic in brandToDelete.BrandStatistics)
            {
                substitutingBrand.BrandStatistics.Add(brandStatistic);
            }

            _brandRepository.Delete(brandToDelete);
            _brandRepository.Persist();
            Log.Debug("Brand '{0}' with Id '{1}' deleted. Substitute brand: '{2}' - {3}", brandToDelete.BrandName,
                      brandToDelete.Id, substitutingBrand.Id, brandToDelete.BrandName);
            return true;
        }

        public Brand FindBrand(string brandName, bool onlyPublishedAdvices)
        {
            var result = _brandRepository.Find(b => b.BrandName == brandName).FirstOrDefault();

            if (result != null && onlyPublishedAdvices)
            {
                result.BrandAdvices = result.BrandAdvices.Where(a => a.Published).ToList();
            }

            return result;
        }

        public IList<Brand> FindBrands(string brandName, bool onlyPublishedAdvices)
        {
            var result = _brandRepository.Find(b => b.BrandName.Equals(brandName) ||
                                                    b.BrandName.StartsWith(brandName + " ") ||
                                                    b.BrandName.Contains(" " + brandName + " ") ||
                                                    b.BrandName.EndsWith(" " + brandName)).OrderBy(x => x.BrandName).ToList();

            if (onlyPublishedAdvices)
            {
                foreach (var brand in result)
                {
                    brand.BrandAdvices = brand.BrandAdvices.Where(a => a.Published).ToList();
                }
            }

            return result;
        }

        #region Helper methods
        private void SetOwner(Brand brand, int? companyId)
        {
            if (companyId == null)
            {
                brand.Owner = null;
                return;
            }
            var companyRepository = _repositoryFactory.Build<IRepository<Company>, Company>();

            var owner = companyRepository.FindOne(x => x.Id == companyId);
            brand.Owner = _brandRepository.FindDomainObject(owner);
        }

        #endregion
    }
}
