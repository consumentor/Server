using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Repository
{
    public interface IIngredientRepository : IRepository<Ingredient>
    {
        bool IngredientNameAvailable(string name);
        void Refresh(Ingredient ingredient);
        void Delete<T>(T item) where T : class;
        bool HasChanges { get; }
    }
}

