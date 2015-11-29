using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface ISemaphoreApplicationService
    {
        IList<Semaphore> GetAllSemaphores();
        Semaphore GetSemaphoreById(int id);
    }
}