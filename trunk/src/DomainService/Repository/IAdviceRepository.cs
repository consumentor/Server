using System.Collections.Generic;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Repository
{
    public interface IAdviceRepository : IRepository<AdviceBase>
    {
        void Refresh(AdviceBase advice);
        Mentor FindMentor(int id);
        Semaphore FindSemaphore(int id);
        IList<AdviceTag> GetAllAdviceTags();

        IAdviceable<TAdvice> FindAdviceableDomainObject<TDomainObject, TAdvice>(TDomainObject domainObject)
            where TDomainObject : class, IAdviceable<TAdvice>
            where TAdvice : AdviceBase;

        IList<Ingredient> GetIngredientsWithAdvicesByMentor(Mentor mentor);
        IList<Concept> GetConeptsWithAdvicesByMentor(Mentor mentor);
        IList<Company> GetCompaniesWithAdvicesByMentor(Mentor mentor);
        IList<Brand> GetBrandsWithAdvicesByMentor(Mentor mentor);
        IList<Country> GetCountriesWithAdvicesByMentor(Mentor mentor);
        IList<Product> GetProductsWithAdvicesByMentor(Mentor mentor);

        TAdvice FindAdvice<TAdvice>(int id) where TAdvice : AdviceBase;
        AdviceBase FindAdvice(int id);
    }
}