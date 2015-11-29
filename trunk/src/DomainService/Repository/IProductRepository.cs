
using System.Collections.Generic;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Refresh(Product product);
        bool HasChanges { get; }
        Brand GetBrand(int brandId);
        Country GetCountry(int countryId);
        CertificationMark GetCertificationMark(int certificationMarkId);
        void DeleteProductCertificationMarkItem(int productId, int certificationMarkId);
        Ingredient GetIngredient(int ingredientId);
        void DeleteProductIngredientItem(int productId, int ingredientId);

        IList<Product> GetProductsWithAdvicesByMentor(int mentorId);

        ProductAdvice GetProductAdivce(int productAdviceId);
        void DeleteProductAdvice(ProductAdvice advice);

        Semaphore FindSemaphore(int id);
        AdviceTag FindTag(int tagId);
    }
}
