using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using DataContext = Consumentor.ShopGun.Repository.DataContext;

namespace Consumentor.ShopGun.DomainService.Repository
{
    public class AdviceRepository : Repository<AdviceBase>, IAdviceRepository
    {
        public AdviceRepository(DataContext context)
            : base(context)
        {
        }

        void IAdviceRepository.Refresh(AdviceBase advice)
        {
            Context.Refresh(RefreshMode.KeepChanges, advice);
        }

        Mentor IAdviceRepository.FindMentor(int id)
        {
            var mentors = from mentor in Find<Mentor>()
                             where mentor.Id == id
                             select mentor;
            return mentors.FirstOrDefault();
        }

        Semaphore IAdviceRepository.FindSemaphore(int id)
        {
            var semaphores = from semaphore in Find<Semaphore>()
                          where semaphore.Id == id
                          select semaphore;
            return semaphores.FirstOrDefault();
        }

        public IList<AdviceTag> GetAllAdviceTags()
        {
            var adviceTags = from adviceTag in Find<AdviceTag>()
                             select adviceTag;

            return adviceTags.ToList();
        }

        public IAdviceable<TAdvice> FindAdviceableDomainObject<TDomainObject, TAdvice>(TDomainObject domainObject) 
            where TDomainObject : class, IAdviceable<TAdvice> 
            where TAdvice : AdviceBase
        {
            var domainObjects = from dO in Find<TDomainObject>()
                                where dO == domainObject
                                select dO;
            return domainObjects.FirstOrDefault();
        }

        public IList<Ingredient> GetIngredientsWithAdvicesByMentor(Mentor mentor)
        {
            var result = from ingredient in Find<Ingredient>()
                         where ingredient.IngredientAdvices.Any(a => a.Mentor.Id == mentor.Id)
                         select ingredient;

            //foreach (var ingredient in result)
            //{
            //    var advicesFromMentor = from a in ingredient.IngredientAdvices
            //                            where a.Mentor.Id == mentor.Id
            //                            orderby a.Id descending
            //                            select a;
            //    ingredient.IngredientAdvices = advicesFromMentor.ToList();
            //}

            return result.ToList();
        }

        public IList<Concept> GetConeptsWithAdvicesByMentor(Mentor mentor)
        {
            var result = from concept in Find<Concept>()
                         where concept.ConceptAdvices.Any(a => a.Mentor == mentor)
                         select concept;

            //foreach (var concept in result)
            //{
            //    concept.ConceptAdvices = GetAdvicesFromMentor(mentor, concept.ConceptAdvices);
            //}

            return result.ToList();
        }

        public IList<Company> GetCompaniesWithAdvicesByMentor(Mentor mentor)
        {
            var result = from company in Find<Company>()
                         where company.CompanyAdvices.Any(a => a.Mentor == mentor)
                         select company;

            //foreach (var company in result)
            //{
            //    company.CompanyAdvices = GetAdvicesFromMentor(mentor, company.CompanyAdvices);
            //}

            return result.ToList();
        }

        public IList<Brand> GetBrandsWithAdvicesByMentor(Mentor mentor)
        {
            var result = from brand in Find<Brand>()
                         where brand.BrandAdvices.Any(a => a.Mentor == mentor)
                         select brand;

            //foreach (var brand in result)
            //{
            //    brand.BrandAdvices = GetAdvicesFromMentor(mentor, brand.BrandAdvices);
            //}

            return result.ToList();
        }

        public IList<Country> GetCountriesWithAdvicesByMentor(Mentor mentor)
        {
            var result = from country in Find<Country>()
                         where country.CountryAdvices.Any(a => a.Mentor == mentor)
                         select country;

            //foreach (var country in result)
            //{
            //    country.CountryAdvices = GetAdvicesFromMentor(mentor, country.CountryAdvices);
            //}

            return result.ToList();
        }

        public IList<Product> GetProductsWithAdvicesByMentor(Mentor mentor)
        {
            var result = from product in Find<Product>()
                         where product.ProductAdvices.Any(a => a.Mentor == mentor)
                         select product;

            //foreach (var product in result)
            //{
            //    product.ProductAdvices = GetAdvicesFromMentor(mentor, product.ProductAdvices);
            //}

            return result.ToList();
        }

        private static IList<TAdvice> GetAdvicesFromMentor<TAdvice>(Mentor mentor, IEnumerable<TAdvice> advices) where TAdvice : AdviceBase
        {
            var advicesFromMentor = from a in advices
                                    where a.Mentor.Id == mentor.Id
                                    orderby a.Id descending
                                    select a;
            return advicesFromMentor.ToList();
        }

        public TAdvice FindAdvice<TAdvice>(int id) where TAdvice : AdviceBase
        {
            var advices = from advice in Find<TAdvice>()
                          where advice.Id == id
                          select advice;

            return advices.FirstOrDefault();
        }

        public AdviceBase FindAdvice(int id)
        {
            var advices = from advice in Find<AdviceBase>()
                          where advice.Id == id
                          select advice;

            return advices.FirstOrDefault();
        }
    }
}
