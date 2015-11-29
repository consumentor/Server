using System;
using System.Linq;
using Consumentor.ShopGun.ApplicationService.Server;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using IntegrationTest.HelperClasses;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using ShopGunSpecBase;

namespace IntegrationTest.Database
{
    [TestFixture, Category("Integration")]
    public class BrandApplicationServiceSpec : DatabaseSpecBase
    {
        private IBrandApplicationService _brandApplicationService;

        protected override void Before_all_specs()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof(Base).Assembly);
            _brandApplicationService = new BrandApplicationService(new RepositoryFactory(_configuration.Container));
            
            base.Before_all_specs();
        }

        protected override void Before_each_spec()
        {
            var brands = _brandApplicationService.GetAllBrands();
            foreach (var brand in brands)
            {
                _brandApplicationService.DeleteBrand(brand.Id);
            }
            base.Before_each_spec();
        }

        [Test]
        public void ShouldCreateBrand()
        {
            var brand = BrandBuilder.BuildBrand("Brand1");
            brand = _brandApplicationService.CreateBrand(brand);

            brand.Id.ShouldBeGreaterThan(0);
            using (var dc = GetNewDataContext())
            {
                var dcBrand = dc.GetTable<Brand>().Single(x => x.BrandName == brand.BrandName);
                brand.Id.ShouldEqual(dcBrand.Id);
            }
        }

        [Test]
        public void ShouldNotBuildBrandWithAlreadyExistingName()
        {
            var brand1 = BrandBuilder.BuildBrand("BrandWithUniqueName");
            var brand2 = BrandBuilder.BuildBrand("BrandWithUniqueName");

            _brandApplicationService.CreateBrand(brand1);
            try
            {
                _brandApplicationService.CreateBrand(brand2);
                Assert.Fail("Should have thrown an exception when creating two brands with equal names.");
            }
            catch (ArgumentException)
            {
            }
        }

        [Test]
        public void ShouldFindOneBrandByName()
        {
            var brand1 = BrandBuilder.BuildBrand("Brand1");
            var brand2 = BrandBuilder.BuildBrand("Brand2");
            using (var context = GetNewDataContext())
            {
                var brands = context.GetTable<Brand>();
                brands.InsertOnSubmit(brand1);
                brands.InsertOnSubmit(brand2);
                context.SubmitChanges();
            }

            var brand = _brandApplicationService.FindBrand("Brand1", false);
            brand.BrandName.ShouldEqual(brand1.BrandName);
        }

        [Test]
        public void ShouldFindTwoBrandsByName()
        {
            var brand1 = BrandBuilder.BuildBrand("Brand1");
            var brand2 = BrandBuilder.BuildBrand("Brand2");
            var brand3 = BrandBuilder.BuildBrand("ShouldntFindThisBrand");
        }
    }
}
