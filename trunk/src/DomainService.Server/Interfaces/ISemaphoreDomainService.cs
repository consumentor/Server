using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server.Interfaces
{
    public interface ISemaphoreDomainService
    {
        IList<Semaphore> GetAllSemaphores();
        Semaphore GetSemaphoreById(int id);
    }
}