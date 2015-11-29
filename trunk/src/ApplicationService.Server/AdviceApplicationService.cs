using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class AdviceApplicationService : IAdviceApplicationService
    {
        private readonly IAdviceRepository _adviceRepository;
        private readonly RepositoryFactory _repositoryFactory;

        public AdviceApplicationService(RepositoryFactory repositoryFactory, IAdviceRepository adviceRepository)
        {
            _repositoryFactory = repositoryFactory;
            _adviceRepository = adviceRepository;
        }

        public ILogger Log { get; set; }

        #region IAdviceApplicationService Members
        public Mentor GetMentor(int id)
        {
            return _adviceRepository.FindMentor(id);
        }

        public AdviceBase GetAdvice(int adviceId)
        {
            return _adviceRepository.FindAdvice(adviceId);
        }

        public IList<AdviceTag> GetAllAdviceTags()
        {
            return _adviceRepository.GetAllAdviceTags();
        }

        public IList<Ingredient> GeIngredientsWithAdvicesByMentor(Mentor mentor)
        {
            return _adviceRepository.GetIngredientsWithAdvicesByMentor(mentor);
        }

        public IList<Concept> GetConeptsWithAdvicesByMentor(Mentor mentor)
        {
            return _adviceRepository.GetConeptsWithAdvicesByMentor(mentor);
        }

        public IList<Company> GetCompaniesWithAdvicesByMentor(Mentor mentor)
        {
            return _adviceRepository.GetCompaniesWithAdvicesByMentor(mentor);
        }

        public IList<Brand> GetBrandsWithAdvicesByMentor(Mentor mentor)
        {
            return _adviceRepository.GetBrandsWithAdvicesByMentor(mentor);
        }

        public IList<Country> GetCountriesWithAdvicesByMentor(Mentor mentor)
        {
            return _adviceRepository.GetCountriesWithAdvicesByMentor(mentor);
        }

        public IList<Product> GetProductsWithAdvicesByMentor(Mentor mentor)
        {
            return _adviceRepository.GetProductsWithAdvicesByMentor(mentor);
        }

        #region CRUD

        private TAdvice AddAdvice<TAdvice, TDomain>(Mentor mentor, TAdvice adviceToAdd, TDomain adviceableDomainObject)
            where TAdvice : AdviceBase
            where TDomain : class, IAdviceable<TAdvice>
        {
            SetMentor(mentor.Id, adviceToAdd);
            SetSemaphore(adviceToAdd.SemaphoreId.Value, adviceToAdd);
            SetTag(adviceToAdd.TagId, adviceToAdd);

            var adviceableObject = _adviceRepository.FindDomainObject(adviceableDomainObject);
            if (adviceableObject == null)
            {
                var exceptionMessage = "No adviceable object found when trying to add Advice";
                Log.Error(exceptionMessage);
                throw new NullReferenceException(exceptionMessage);
            }
            adviceableObject.AddAdvice(adviceToAdd);
            _adviceRepository.MergePersist();

            return adviceToAdd;
        }

        public IngredientAdvice AddIngredientAdvice(Mentor mentor, IngredientAdvice advice)
        {
            if (advice.IngredientsId == null)
            {
                throw new ArgumentException("IngredientsId cannot be null when adding IngredientAdvice");
            }
            using (var ingredientRepository = _repositoryFactory.Build<IRepository<Ingredient>, Ingredient>())
            {
                var ingredient = ingredientRepository.FindOne(x => x.Id == advice.IngredientsId);
                return AddAdvice(mentor, advice, ingredient);
            }
        }

        public ProductAdvice AddProductAdvice(Mentor mentor, ProductAdvice adviceToAdd)
        {
            if (adviceToAdd.ProductsId == null)
            {
                throw new ArgumentException("ProductsId cannot be null when adding ProductAdvice");
            }
            using (var productRepository = _repositoryFactory.Build<IRepository<Product>, Product>())
            {
                var product = productRepository.FindOne(x => x.Id == adviceToAdd.ProductsId);
                return AddAdvice(mentor, adviceToAdd, product);
            }
        }

        public BrandAdvice AddBrandAdvice(Mentor mentor, BrandAdvice adviceToAdd)
        {
            if (adviceToAdd.BrandsId == null)
            {
                throw new ArgumentException("BrandsId cannot be null when adding BrandAdvice");
            }
            using (var brandRepository = _repositoryFactory.Build<IRepository<Brand>, Brand>())
            {
                var brand = brandRepository.FindOne(x => x.Id == adviceToAdd.BrandsId);
                return AddAdvice(mentor, adviceToAdd, brand);
            }
        }

        public CompanyAdvice AddCompanyAdvice(Mentor mentor, CompanyAdvice adviceToAdd)
        {
            if (adviceToAdd.CompanysId == null)
            {
                throw new ArgumentException("CompanysId cannot be null when adding CompanyAdvice");
            }
            using (var companyRepository = _repositoryFactory.Build<IRepository<Company>, Company>())
            {
                var company = companyRepository.FindOne(x => x.Id == adviceToAdd.CompanysId);
                return AddAdvice(mentor, adviceToAdd, company);
            }
        }

        public CountryAdvice AddCountryAdvice(Mentor mentor, CountryAdvice adviceToAdd)
        {
            if (adviceToAdd.CountrysId == null)
            {
                throw new ArgumentException("CountrysId cannot be null when adding CountryAdvice");
            }
            using (var countryRepository = _repositoryFactory.Build<IRepository<Country>, Country>())
            {
                var country = countryRepository.FindOne(x => x.Id == adviceToAdd.CountrysId);
                return AddAdvice(mentor, adviceToAdd, country);
            }
        }

        public ConceptAdvice AddConceptAdvice(Mentor mentor, ConceptAdvice adviceToAdd)
        {
            if (adviceToAdd.ConceptsId == null)
            {
                throw new ArgumentException("ConceptsId cannot be null when adding ConceptAdvice");
            }
            using (var conceptRepository = _repositoryFactory.Build<IRepository<Concept>, Concept>())
            {
                var concept = conceptRepository.FindOne(x => x.Id == adviceToAdd.ConceptsId);
                return AddAdvice(mentor, adviceToAdd, concept);
            }
        }

        public BrandAdvice AddBrandAdvice(Mentor mentor, Brand brand, AdviceBase advice, bool publish)
        {
            return AddBrandAdvice(mentor.Id, brand.Id, advice.SemaphoreId.Value, advice.Label,
                                  advice.Introduction, advice.Advice, advice.KeyWords, publish);
        }

        public BrandAdvice AddBrandAdvice(int mentorId, int brandId, int semaphoreId, string label, string introduction, string adviceText, string keywords, bool publish)
        {
            var brandRepository = _repositoryFactory.Build<IRepository<Brand>, Brand>();

            var brand = brandRepository.FindOne(b => b.Id == brandId);
            var addedAdvice = AddAdvice<BrandAdvice, Brand>(mentorId, brand, semaphoreId, label, introduction,
                                                            adviceText, keywords, publish);

            return addedAdvice;
        }

        public CompanyAdvice AddCompanyAdvice(Mentor mentor, Company company, AdviceBase advice, bool publish)
        {
            return AddCompanyAdvice(mentor.Id, company.Id, advice.SemaphoreId.Value, advice.Label,
                                    advice.Introduction, advice.Advice, advice.KeyWords, publish);
        }

        public CompanyAdvice AddCompanyAdvice(int mentorId, int companyId, int semaphoreId, string label, string introduction, string adviceText, string keywords, bool publish)
        {
            var companyRepository = _repositoryFactory.Build<IRepository<Company>, Company>();

            var company = companyRepository.FindOne(c => c.Id == companyId);
            return AddAdvice<CompanyAdvice, Company>(mentorId, company, semaphoreId, label, introduction,
                                                     adviceText, keywords, publish);
        }


        public CountryAdvice AddCountryAdvice(Mentor mentor, Country country, AdviceBase advice, bool publish)
        {
            return AddCountryAdvice(mentor.Id, country.Id, advice.SemaphoreId.Value, advice.Label,
                                    advice.Introduction, advice.Advice, advice.KeyWords, publish);
        }

        public CountryAdvice AddCountryAdvice(int mentorId, int countryId, int semaphoreId, string label, string introduction, string adviceText, string keywords, bool publish)
        {
            var countryRepository = _repositoryFactory.Build<IRepository<Country>, Country>();

            var country = countryRepository.FindOne(c => c.Id == countryId);
            return AddAdvice<CountryAdvice, Country>(mentorId, country, semaphoreId, label, introduction,
                                                     adviceText, keywords, publish);
        }


        public ConceptAdvice AddConceptAdvice(Mentor mentor, Concept concept, AdviceBase advice, bool publish)
        {
            return AddConceptAdvice(mentor.Id, concept.Id, advice.SemaphoreId.Value, advice.Label,
                                    advice.Introduction, advice.Advice, advice.KeyWords, publish);
        }

        public ConceptAdvice AddConceptAdvice(int mentorId, int conceptId, int semaphoreId, string label, string introduction, string adviceText, string keywords, bool publish)
        {
            var conceptRepository = _repositoryFactory.Build<IRepository<Concept>, Concept>();

            var concept = conceptRepository.FindOne(c => c.Id == conceptId);
            return AddAdvice<ConceptAdvice, Concept>(mentorId, concept, semaphoreId, label, introduction,
                                                     adviceText, keywords, publish);
        }

        private TAdvice AddAdvice<TAdvice, TDomain>(int mentorId, TDomain adviceableDomainObject, int semaphoreId,
                                                string label, string introduction, string advice, string keyWords, bool publish)
            where TAdvice : AdviceBase, new()
            where TDomain : class, IAdviceable<TAdvice>
        {
            var adviceToAdd = new TAdvice
                                  {
                                      Label = label,
                                      Introduction = introduction,
                                      Advice = advice,
                                      KeyWords = keyWords ?? "",
                                      Published = publish
                                  };

            SetMentor(mentorId, adviceToAdd);
            SetSemaphore(semaphoreId, adviceToAdd);

            var adviceable = _adviceRepository.FindAdviceableDomainObject<TDomain, TAdvice>(adviceableDomainObject);
            if (adviceable == null)
            {
                var exceptionMessage = "No adviceable object found when trying to add Advice";
                Log.Error(exceptionMessage);
                throw new NullReferenceException(exceptionMessage);
            }
            adviceable.AddAdvice(adviceToAdd);
            _adviceRepository.MergePersist();


            return adviceToAdd;
        }

        public AdviceBase UpdateAdvice(AdviceBase updatedAdvice)
        {
            if (updatedAdvice.Id == null)
            {
                throw new ArgumentException("AdviceId cannot be null when updating");
            }

            var adviceToUpdate = _adviceRepository.FindAdvice(updatedAdvice.Id.Value);

            if (adviceToUpdate == null)
            {
                throw new NullReferenceException(string.Format("Could not find advice with id {0}.", updatedAdvice.Id));
            }

            SetSemaphore(updatedAdvice.SemaphoreId, adviceToUpdate);
            adviceToUpdate.CopyStringProperties(updatedAdvice);
            SetTag(updatedAdvice.TagId, adviceToUpdate);
            adviceToUpdate.KeyWords = updatedAdvice.KeyWords ?? "";
            adviceToUpdate.Published = updatedAdvice.Published;

            _adviceRepository.MergePersist();
            Log.Debug("Advice updated. {0} ", adviceToUpdate.ToString());

            return adviceToUpdate;
        }

        //public void UpdateAdvice(int adviceId, int semaphoreId, string label, string introduction, string adviceText, string keywords, bool publish)
        //{
        //    using (var adviceRepository = _repositoryFactory.Build())
        //    {
        //        var semaphore = adviceRepository.FindSemaphore(semaphoreId);

        //        if (semaphore == null)
        //        {
        //            var exceptionMessage = "No semaphore found when trying to update Advice";
        //            Log.Error(exceptionMessage);
        //            throw new NullReferenceException(exceptionMessage);
        //        }

        //        var advice = adviceRepository.FindAdvice<AdviceBase>(adviceId);

        //        if (advice == null)
        //        {
        //            var exceptionMessage = "Could not find the advice to update.";
        //            Log.Error(exceptionMessage);
        //            throw new NullReferenceException(exceptionMessage);
        //        }

        //        advice.Semaphore = semaphore;
        //        advice.Label = label;
        //        advice.Introduction = introduction;
        //        advice.Advice = adviceText;
        //        advice.KeyWords = keywords;
        //        advice.Published = publish;

        //        adviceRepository.MergePersist();
        //        Log.Debug("Advice updated. {0} ", advice.ToString());
        //    }
        //}

        public void PublishAdvice(int adviceId)
        {
            var advice = ToggleAdvicePublishFlag<AdviceBase>(adviceId, true);
            Log.Debug("Advice published. {0} ", advice.ToString());
        }

        public void UnpublishAdvice(int adviceId)
        {
            var advice = ToggleAdvicePublishFlag<AdviceBase>(adviceId, false);
            Log.Debug("Advice unpublished. {0} ", advice.ToString());
        }

        public void DeleteAdvice(int adviceId)
        {
            var advice = _adviceRepository.FindAdvice<AdviceBase>(adviceId);
            _adviceRepository.Delete(advice);
            _adviceRepository.Persist();
        }

        #endregion

        #endregion

        #region Helper methods

        private void SetSemaphore(int? semaphoreId, AdviceBase adviceToAdd)
        {
            if (!semaphoreId.HasValue)
            {
                throw new ArgumentException("Semaphore not set");
            }

            var semaphore = _adviceRepository.FindSemaphore(semaphoreId.Value);

            if (semaphore == null)
            {
                string exceptonMessage = "No Semaphore found when trying to add Advice";
                Log.Error(exceptonMessage);
                throw new NullReferenceException(exceptonMessage);
            }
            adviceToAdd.Semaphore = semaphore;
        }

        private void SetMentor(int mentorId, AdviceBase adviceToAdd)
        {
            var mentor = _adviceRepository.FindMentor(mentorId);
            if (mentor == null)
            {
                string exceptionMessage = "No Mentor founded when trying to add Advice";
                Log.Error(exceptionMessage);
                throw new NullReferenceException(exceptionMessage);
            }

            adviceToAdd.Mentor = mentor;
        }

        
        private void SetTag(int? tagId, AdviceBase adviceToAdd)
        {
            if (tagId == null)
            {
                adviceToAdd.Tag = null;
                return;
            }

            using (var adviceTagRepository = _repositoryFactory.Build<IRepository<AdviceTag>, AdviceTag>())
            {
                var tag = adviceTagRepository.FindOne(x => x.Id == tagId);
                tag = _adviceRepository.FindDomainObject(tag);
                adviceToAdd.Tag = tag;
            }
        }

        private TAdvice ToggleAdvicePublishFlag<TAdvice>(int adviceId, bool publishFlag) where TAdvice : AdviceBase
        {
            var advice = _adviceRepository.FindAdvice<TAdvice>(adviceId);

            if (advice == null)
            {
                string exceptionMessage = "Could not found the advice to publish/unpublish.";
                Log.Error(exceptionMessage);
                throw new NullReferenceException(exceptionMessage);
            }

            advice.Published = publishFlag;
            advice.PublishDate = DateTime.Now;
            _adviceRepository.Persist();

            return advice;
        }

        #endregion
    }
}
