using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IProductAdviceApplicationService
    {
        IList<Product> GetProductsWithAdvicesByMentor(int mentorId);
        Product GetProduct(int productId);
        Product GetProductForAdvice(int productAdviceId);
        bool SaveProductAdvice(ProductAdvice productAdvice);
        void PublishProductAdvice(int productAdviceId);
        void UnpublishProductAdvice(int productAdviceId);
        void DeleteProductAdvice(int productAdviceId);
    }
}
