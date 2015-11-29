using System.Collections.Generic;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Server.Repository
{
    public interface IAdviceRepository : IRepository<AdviceBase>
    {
        void Refresh(AdviceBase advice);
        Mentor FindMentor(int id);
        Semaphore FindSemaphore(int id);
        TDomainObject FindDomainObject<TDomainObject>(TDomainObject domainObject) where TDomainObject : class;
        IList<Ingredient> GeIngredientsWithAdvicesByMentor(Mentor mentor);
        TAdvice FindAdvice<TAdvice>(int id) where TAdvice : AdviceBase;
    }
}