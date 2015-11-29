using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using ShopGunDataContext = Consumentor.ShopGun.Repository.DataContext;

namespace Consumentor.ShopGun.DomainService.Server.Repository
{
    public class AdviceRepository : Repository<AdviceBase>, IAdviceRepository
    {
        public AdviceRepository(ShopGunDataContext context)
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

        public TDomainObject FindDomainObject<TDomainObject>(TDomainObject domainObject) where TDomainObject : class
        {
            var domainObjects = from dO in Find<TDomainObject>()
                                where dO == domainObject
                                select dO;
            return domainObjects.FirstOrDefault();
        }

        public IList<Ingredient> GeIngredientsWithAdvicesByMentor(Mentor mentor)
        {
            var result = from ingredient in Find<Ingredient>()
                         where ingredient.IngredientAdvices.Any(a => a.Mentor == mentor)
                         select ingredient;

            foreach (var ingredient in result)
            {
                var advicesFromMentor = from a in ingredient.IngredientAdvices
                                        where a.Mentor.Id == mentor.Id
                                        orderby a.Id descending
                                        select a;
                ingredient.IngredientAdvices = advicesFromMentor.ToList();
            }

            return result.ToList();
        }

        public TAdvice FindAdvice<TAdvice>(int id) where TAdvice : AdviceBase
        {
            var advices = from advice in Find<TAdvice>()
                          where advice.Id == id
                          select advice;

            return advices.FirstOrDefault();
        }
    }
}
