using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server.Interfaces
{
    public interface IAdviceDomainService<TEntityType, TAdviceType> where TEntityType : class, IAdviceable<TAdviceType> where TAdviceType : AdviceBase, new()
    {
        TAdviceType AddAdvice(Mentor mentor, TEntityType entity, Semaphore semaphore, string label, string introduction, string advice, string keyWords, bool publish);
        TAdviceType UpdateAdvice(TAdviceType adviceToUpdate, Semaphore semaphore, string label, string introduction, string advice, string keyWords, bool publish);
        TAdviceType GetAdviceById(int id);
        void Publish(TAdviceType adviceToPublish);
        void Unpublish(TAdviceType adviceToUnpublish);
        void Delete(TAdviceType adviceToDelete);
        IList<TAdviceType> GetAdvicesByMentor(Mentor mentor);
    }
}