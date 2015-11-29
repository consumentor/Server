using System.Collections.Generic;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Server
{
    public class SemaphoreDomainService : ISemaphoreDomainService
    {
        private readonly IRepository<Semaphore> _semaphoreRepository;

        public SemaphoreDomainService(IRepository<Semaphore> semaphoreRepository)
        {
            _semaphoreRepository = semaphoreRepository;
        }

        public IList<Semaphore> GetAllSemaphores()
        {
            return _semaphoreRepository.Find(s => s != null).ToList();
        }

        public Semaphore GetSemaphoreById(int id)
        {
            return _semaphoreRepository.FindOne(s => s.Id == id);
        }
    }
}
