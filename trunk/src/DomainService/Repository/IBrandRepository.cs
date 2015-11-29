using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Repository
{
    public interface IBrandRepository : IRepository<Brand>
    {
        void Refresh(Brand brand);
        void Delete<T>(T item) where T : class;
        bool HasChanges { get; }
    }
}
