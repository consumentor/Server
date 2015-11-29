using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using ShopgunDataContext = Consumentor.ShopGun.Repository.DataContext;

namespace Consumentor.ShopGun.DomainService.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ShopgunDataContext context)
            : base(context)
        {
            var loadOptions = new DataLoadOptions();
            loadOptions.LoadWith<Product>(p => p.CertificationMarks);
            context.LoadOptions = loadOptions;
        }
        public bool HasChanges
        {
            get
            {
                var changeSet = Context.GetChangeSet();
                return changeSet.Inserts.Count > 0 || changeSet.Updates.Count > 0 || changeSet.Deletes.Count > 0;
            }            
        }

        public Brand GetBrand(int brandId)
        {
            return Find<Brand>().Single(b => b.Id == brandId);
        }

        public Country GetCountry(int countryId)
        {
            return Find<Country>().Single(c => c.Id == countryId);
        }

        public CertificationMark GetCertificationMark(int certificationMarkId)
        {
            return Find<CertificationMark>().Single(cm => cm.Id == certificationMarkId);
        }

        public void DeleteProductCertificationMarkItem(int productId, int certificationMarkId)
        {
            var itemToRemove =
                Find<ProductCertificationMark>().Single(
                    x => x.ProductId == productId && x.CertificationMarkId == certificationMarkId);
            Delete(itemToRemove);
        }

        public Ingredient GetIngredient(int ingredientId)
        {
            return Find<Ingredient>().Single(i => i.Id == ingredientId);
        }

        public void DeleteProductIngredientItem(int productId, int ingredientId)
        {
            var itemToRemove =
                Find<ProductIngredient>().Single(
                    x => x.ProductId == productId && x.IngredientId == ingredientId);
            Delete(itemToRemove);
        }

        public IList<Product> GetProductsWithAdvicesByMentor(int mentorId)
        {
            var result = from product in Find<Product>()
                         where product.ProductAdvices.Any(a => a.Mentor.Id == mentorId)
                         select product;

            return result.ToList();
        }

        public ProductAdvice GetProductAdivce(int productAdviceId)
        {
            var adivce = Find<ProductAdvice>().FirstOrDefault(a => a.Id == productAdviceId);
            return adivce;
        }

        public void DeleteProductAdvice(ProductAdvice advice)
        {
            Delete(advice);
        }

        public void Refresh(Product product)
        {
            //Refresh regular members
            Context.Refresh(RefreshMode.KeepChanges, product);

            //Refresh EntityRefs - Do we need this:Boris E??
            //if (product.Order != null)
            //    Context.Refresh(RefreshMode.KeepChanges, product.Order);

            //Refresh EntitySets
            RefreshProductAdvices(product);
        }


        private void RefreshProductAdvices(Product product)
        {
            var productAdvices = (EntitySet<ProductAdvice>)product.ProductAdvices;
            if (productAdvices.HasLoadedOrAssignedValues)
            {
                 //Simon, find away how to get all ProductAdvices that contains the same product Id (product.Id)
                List<ProductAdvice> allProductAdvices = Context.GetTableForType(typeof(ProductAdvice)).OfType<ProductAdvice>().Where(pa => pa.ProductsId == product.Id).ToList(); 
                foreach (ProductAdvice productAdvice in allProductAdvices.Except(product.ProductAdvices))
                {
                    product.ProductAdvices.Add(productAdvice);
                }

                foreach (ProductAdvice productAdvice in product.ProductAdvices.Except(allProductAdvices))
                {
                    product.ProductAdvices.Remove(productAdvice);
                }

                //Refresh EntityRefs
                foreach (ProductAdvice productAdvice in product.ProductAdvices)
                {
                    Context.Refresh(RefreshMode.KeepChanges, productAdvice);
                    Context.Refresh(RefreshMode.KeepChanges, productAdvice.Mentor);
                    Context.Refresh(RefreshMode.KeepChanges, productAdvice.Semaphore);
                }
            }
        }

        

        public Semaphore FindSemaphore(int id)
        {
            var semaphores = from semaphore in Find<Semaphore>()
                             where semaphore.Id == id
                             select semaphore;
            return semaphores.FirstOrDefault();
        }

        public AdviceTag FindTag(int tagId)
        {
            var adviceTags = from adviceTag in Find<AdviceTag>()
                             where adviceTag.Id == tagId
                             select adviceTag;

            return adviceTags.FirstOrDefault();
        }

        //private void RefreshAreas(RollMap rollMap)
        //{
        //    var areas = (EntitySet<Area>)rollMap.Areas;
        //    if (areas.HasLoadedOrAssignedValues)
        //    {
        //        List<Area> allAreas = Context.GetTable<Area>().AsQueryable().Where(a => a.RollMap.Id == rollMap.Id).ToList();
        //        foreach (Area area in allAreas.Except(rollMap.Areas))
        //        {
        //            rollMap.Areas.Add(area);
        //        }
        //        foreach (Area area in rollMap.Areas.Except(allAreas))
        //        {
        //            rollMap.Areas.Remove(area);
        //        }

        //        //Refresh EntityRefs
        //        foreach (Area area in rollMap.Areas)
        //        {
        //            Context.Refresh(RefreshMode.KeepChanges, area);
        //            Context.Refresh(RefreshMode.KeepChanges, area.EndPosition);
        //            Context.Refresh(RefreshMode.KeepChanges, area.StartPosition);
        //        }
        //    }
        //}
    }
}
