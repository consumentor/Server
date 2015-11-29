using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IProductApplicationService
    {
        Product FindProductByGtin(string globalTradeItemNumber, bool onlyPublishedAdvices);
        Product AddIngredientToProduct(int productId, int ingredientId);
        Product RemoveIngredientFromProduct(int productId, int ingredientId);
        Product AddIngredientsToProduct(Product product, IList<Ingredient> ingredients);

        Product CreateProduct(Product productToCreate);
        Product GetProduct(int productId);
        Product GetMergedProduct(int productId);
        IList<Product> GetAllProducts();
        Product UpdateProduct(Product updatedProduct);
        bool DeleteProduct(int productId);

        Product FindProductByGtinInOwnDatabase(string gtin, bool onlyPublishedAdvices);
        IList<Product> FindProducts(string productName, bool onlyPublishedAdvices);
        IList<Product> GetProductsByBrand(string brand);
        Product FindProductAndMerge(string gtin, IList<Product> productsToMerge);

        Product AddCertificationMarkToProduct(int productId, int certificationMarkId);
        Product RemoveCertificationMarkFromProduct(int productId, int certificationMarkId);
        Product RemoveAllCertificationMarksFromProduct(int productId);

        /*******************
         * Advices
         ******************/
        Product GetProductForAdvice(int adviceId);
    }
}
