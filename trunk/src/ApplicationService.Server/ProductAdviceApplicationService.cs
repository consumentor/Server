using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.Log;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class ProductAdviceApplicationService : IProductAdviceApplicationService
    {
        private readonly IProductRepository _productRepository;

        public ILogger Log { get; set; }

        public ProductAdviceApplicationService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IList<Product> GetProductsWithAdvicesByMentor(int mentorId)
        {
            var products = _productRepository.Find(p => p.ProductAdvices.Any(a => a.MentorId == mentorId));
            return products.ToList();
        }

        public Product GetProduct(int productId)
        {
            var product = _productRepository.Find(p => p.Id == productId).FirstOrDefault();
            return product;
        }

        public Product GetProductForAdvice(int productAdviceId)
        {
            var product = _productRepository.Find(p => p.ProductAdvices.Any(a => a.Id == productAdviceId)).ToList().
                FirstOrDefault();

            return product;
        }

        public bool SaveProductAdvice(ProductAdvice productAdvice)
        {
            if (!productAdvice.Id.HasValue || productAdvice.Id == 0)
            {
                var product = _productRepository.FindOne(p => p.Id == productAdvice.ProductsId);
                product.ProductAdvices.Add(productAdvice);
                _productRepository.Persist();
                return true;
            }
            else
            {
                var product = GetProductForAdvice(productAdvice.Id.Value);

                if (product == null)
                {
                    return false;
                }
                var advice = product.ProductAdvices.Single(a => a.Id == productAdvice.Id);
                SetSemaphore(productAdvice.SemaphoreId, advice);
                SetTag(productAdvice.TagId, advice);
                advice.CopyStringProperties(productAdvice);
                advice.Published = productAdvice.Published;
                _productRepository.Persist();
                return true;
            }

        }

        public void PublishProductAdvice(int productAdviceId)
        {
            var advice = _productRepository.GetProductAdivce(productAdviceId);
            advice.Published = true;
            advice.PublishDate = DateTime.Now;
            _productRepository.Persist();
        }

        public void UnpublishProductAdvice(int productAdviceId)
        {
            var advice = _productRepository.GetProductAdivce(productAdviceId);
            advice.Published = false;
            advice.PublishDate = null;
            _productRepository.Persist();

        }

        public void DeleteProductAdvice(int productAdviceId)
        {
            var product =
                _productRepository.Find(p => p.ProductAdvices.Any(a => a.Id == productAdviceId)).FirstOrDefault();
            if (product != null)
            {
                var advice = product.ProductAdvices.Single(a => a.Id == productAdviceId);
                product.ProductAdvices.Remove(advice);
                _productRepository.Persist();
            }
        }

        private void SetSemaphore(int? semaphoreId, ProductAdvice adviceToAdd)
        {
            if (!semaphoreId.HasValue)
            {
                throw new ArgumentException("Semaphore not set");
            }

            var semaphore = _productRepository.FindSemaphore(semaphoreId.Value);

            if (semaphore == null)
            {
                string exceptonMessage = "No Semaphore found when trying to add Advice";
                Log.Error(exceptonMessage);
                throw new NullReferenceException(exceptonMessage);
            }
            adviceToAdd.Semaphore = semaphore;
        }

        private void SetTag(int? tagId, AdviceBase adviceToAdd)
        {
            if (!tagId.HasValue)
            {
                adviceToAdd.Tag = null;
                return;
            }

            adviceToAdd.Tag = _productRepository.FindTag(tagId.Value);
        }
    }
}
