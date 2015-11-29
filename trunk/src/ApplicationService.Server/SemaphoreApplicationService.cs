using System.Collections.Generic;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    public class SemaphoreApplicationService : ISemaphoreApplicationService
    {
        private readonly ISemaphoreDomainService _semaphoreDomainService;

        public SemaphoreApplicationService(ISemaphoreDomainService semaphoreDomainService)
        {
            _semaphoreDomainService = semaphoreDomainService;
        }

        public IList<Semaphore> GetAllSemaphores()
        {
            return _semaphoreDomainService.GetAllSemaphores();
        }

        public Semaphore GetSemaphoreById(int id)
        {
            return _semaphoreDomainService.GetSemaphoreById(id);
        }
    }
}
