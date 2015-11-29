using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using DataContext = System.Data.Linq.DataContext;
using ShopgunDataContext = Consumentor.ShopGun.Repository.DataContext;

namespace Consumentor.ShopGun.DomainService.Repository
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(ShopgunDataContext context)
            : base(context)
        {

        }
        public bool HasChanges
        {
            get
            {
                var changeSet = Context.GetChangeSet();
                return changeSet.Inserts.Count > 0 || changeSet.Updates.Count > 0 || changeSet.Deletes.Count > 0;
            }
        }

        public void Refresh(Brand brand)
        {
            //Refresh regular members
            brand = Find<Brand>().Single(x => x.Id == brand.Id);
            Context.Refresh(RefreshMode.KeepChanges, brand);

            //Refresh EntityRefs - Do we need this:Boris E??
            //if (brand.Order != null)
            //    Context.Refresh(RefreshMode.KeepChanges, brand.Order);

            //Refresh EntitySets
            RefreshBrandAdvices(brand);
        }

        void IBrandRepository.Delete<T>(T item)
        {
            if (!Context.GetChangeSet().Inserts.Contains(item) && !Context.GetChangeSet().Updates.Contains(item))
                Delete(item);
        }


        private void RefreshBrandAdvices(Brand brand)
        {
            var brandAdvices = (EntitySet<BrandAdvice>) brand.BrandAdvices;
            var tempContext = new DataContext(Context.Connection);

            if (brandAdvices.HasLoadedOrAssignedValues)
            {
                List<BrandAdvice> allBrandAdvices =
                    tempContext.GetTable<AdviceBase>().OfType<BrandAdvice>().Where(x => x.BrandsId == brand.Id).ToList();
                //Context.GetTableForType(typeof (BrandAdvice)).OfType<BrandAdvice>().Where(
                //    a => a.BrandsId == brand.Id).ToList();))
                foreach (BrandAdvice brandAdvice in allBrandAdvices.Except(brand.BrandAdvices))
                {
                    brand.BrandAdvices.Add(brandAdvice);
                }

                foreach (BrandAdvice brandAdvice in brand.BrandAdvices.Except(allBrandAdvices))
                {
                    brand.BrandAdvices.Remove(brandAdvice);
                }

                //Refresh EntityRefs
                foreach (BrandAdvice brandAdvice in brand.BrandAdvices)
                {
                    Context.Refresh(RefreshMode.KeepChanges, brandAdvice);
                    Context.Refresh(RefreshMode.KeepChanges, brandAdvice.Mentor);
                    Context.Refresh(RefreshMode.KeepChanges, brandAdvice.Semaphore);
                }
            }
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
