using System.Linq;
using Consumentor.ShopGun.ApplicationService;
using Consumentor.ShopGun.ApplicationService.Server;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.Repository;
using IntegrationTest.HelperClasses;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using ShopGunSpecBase;


namespace IntegrationTest.Database
{
    [TestFixture, Category("Integration")]
    public class AdviceApplicationServiceSpec : DatabaseSpecBase
    {
        protected RepositoryFactory<IAdviceRepository, AdviceBase> RepositoryFactory;

        private IAdviceApplicationService _adviceApplicationService;

        private Mentor _mentor;
        private Semaphore _redSemaphore;
        private Semaphore _greenSemaphore;
        private Product _product;

        protected override void Before_all_specs()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof (Base).Assembly);

            RepositoryFactory = CreateStub<RepositoryFactory<IAdviceRepository, AdviceBase>>();

            _adviceApplicationService = new AdviceApplicationService(null, null);

            var mentorRepository = RepositoryFactory.Build<IRepository<Mentor>, Mentor>();

            _mentor = new Mentor
                          {
                              MentorName = "Consumentor"
                          };
            mentorRepository.Add(_mentor);
            mentorRepository.Persist();


            var semaphoreRepository = RepositoryFactory.Build<IRepository<Semaphore>, Semaphore>();

            _redSemaphore = new Semaphore
                                {
                                    ColorName = "Red",
                                    Value = -1
                                };
            semaphoreRepository.Add(_redSemaphore);
            _greenSemaphore = new Semaphore
                                  {
                                      ColorName = "Green",
                                      Value = 1
                                  };
            semaphoreRepository.Add(_greenSemaphore);
            semaphoreRepository.Persist();


            var productRepository = RepositoryFactory.Build<IRepository<Product>, Product>();

            _product = ProductBuilder.BuildProduct();
            productRepository.Add(_product);
            productRepository.Persist();

        }

        protected override void Before_each_spec()
        {
            using (var adviceRepository = RepositoryFactory.Build<IRepository<AdviceBase>, AdviceBase>())
            {
                var advices = adviceRepository.Find(x => x != null);
                adviceRepository.Delete(advices);
                adviceRepository.Persist();
            }
        }

        [Test (Description = "IntegrationTest")]
        public void ShouldAddNewPurchaseAdviceWithNoDuplicateMentorsAndSemaphores()
        {
            //var addedAdvice = _adviceApplicationService.AddProductAdvice(_mentor.Id, _product.Id, _redSemaphore.Id, "some label", "some intro",
            //                                             "Advice 1", "some keywords", false);
            //addedAdvice.Published.ShouldBeFalse();

            //var addedAdvice2 = _adviceApplicationService.AddProductAdvice(_mentor.Id, _product.Id, _redSemaphore.Id, "some label", "some intro",
            //                                             "Advice 2", "some keywords", true);
            //addedAdvice2.Published.ShouldBeTrue();

            //addedAdvice.Id.ShouldNotEqual(addedAdvice2.Id);

            //using (var productRepository = RepositoryFactory.Build<IProductRepository, Product>())
            //{
            //    var product = productRepository.FindOne(p => p.Id != 0);
            //    product.ProductAdvices.Count.ShouldEqual(2);
            //}

            //using (var mentorRepository = RepositoryFactory.Build<IRepository<Mentor>, Mentor>())
            //{
            //    var mentor = mentorRepository.FindOne(m => m != null);
            //}

            //using (var semaphoreRepository = RepositoryFactory.Build<IRepository<Semaphore>, Semaphore>())
            //{
            //    var semaphores = semaphoreRepository.Find(s => s != null);
            //    semaphores.Count().ShouldEqual(2);
            //}
        }

        [Test]
        public void ShouldPublishAdvice()
        {
            //var addedAdvice = _adviceApplicationService.AddProductAdvice(_mentor.Id, _product.Id, _redSemaphore.Id, "some label", "some intro",
            //                                             "Advice 1", "some keywords", false);

            //using (var productRepository = RepositoryFactory.Build<IProductRepository, Product>())
            //{
            //    var product = productRepository.FindOne(p => p != null);
            //    product.ProductAdvices.Count.ShouldEqual(1);
            //    product.ProductAdvices[0].Published.ShouldBeFalse();
            //}

            //_adviceApplicationService.PublishAdvice(addedAdvice.Id.Value);

            //using (var productRepository = RepositoryFactory.Build<IProductRepository, Product>())
            //{
            //    var product = productRepository.FindOne(p => p != null);
            //    product.ProductAdvices.Count.ShouldEqual(1);
            //    product.ProductAdvices[0].Published.ShouldBeTrue();
            //    product.ProductAdvices[0].PublishDate.ShouldNotBeNull();
            //}
        }

        [Test]
        public void ShouldUnpublishAdvice()
        {
            //var addedAdvice = _adviceApplicationService.AddProductAdvice(_mentor.Id, _product.Id, _redSemaphore.Id, "some label", "some intro",
            //                                             "Advice 1", "some keywords", true);

            //using (var productRepository = RepositoryFactory.Build<IProductRepository, Product>())
            //{
            //    var product = productRepository.FindOne(p => p != null);
            //    product.ProductAdvices.Count.ShouldEqual(1);
            //    product.ProductAdvices[0].Published.ShouldBeTrue();
            //}

            //_adviceApplicationService.UnpublishAdvice(addedAdvice.Id.Value);

            //using (var productRepository = RepositoryFactory.Build<IProductRepository, Product>())
            //{
            //    var product = productRepository.FindOne(p => p != null);
            //    product.ProductAdvices.Count.ShouldEqual(1);
            //    product.ProductAdvices[0].Published.ShouldBeFalse();
            //    product.ProductAdvices[0].UnpublishDate.ShouldNotBeNull();
            //}
        }

    //    [Test, Ignore]
    //    public void ShouldPublishUnpublishedAdviceAndCreateNewVersion()
    //    {
    //        var addedAdvice = _adviceApplicationService.AddAdvice<ProductAdvice, Product>(_mentor.Id, _product, _redSemaphore.Id, "some label", "some intro",
    //                                                     "Advice 1", "some keywords", true);

    //        _adviceApplicationService.UnpublishAdvice<ProductAdvice>(addedAdvice.Id.Value);

    //        _product = _productRepository.FindOne(p => p != null);
    //        _product.ProductAdvices.Count.ShouldEqual(1);
    //        var advice = _product.ProductAdvices[0];
    //        advice.Published.ShouldBeFalse();

    //        _adviceApplicationService.PublishAdvice<ProductAdvice>(advice.Id.Value);

    //        using (var dc = GetNewDataContext())
    //        {
    //            var product = dc.GetTable<Product>().Where(p => p != null).FirstOrDefault();
    //            product.ProductAdvices.Count.ShouldEqual(2);
    //            product.ProductAdvices[0].Published.ShouldBeFalse();
    //            product.ProductAdvices[0].UnpublishDate.ShouldNotBeNull();
    //            product.ProductAdvices[1].Published.ShouldBeTrue();
    //            product.ProductAdvices[1].UnpublishDate.ShouldBeNull();
    //        }
    //    }

    //    [Test, Ignore]
    //    public void ShouldUpdateUnpublishedAdvice()
    //    {
    //        var adviceToUpdate = _adviceApplicationService.AddAdvice<ProductAdvice, Product>(_mentor.Id, _product, _redSemaphore.Id, "some label", "some intro",
    //                                                     "unpublished advice", "some keywords", false);

    //        _adviceApplicationService.UpdateAdvice<ProductAdvice>(adviceToUpdate.Id.Value, _greenSemaphore.Id, "", "", "updated published advice",
    //                                                 "", false);

    //        _product = _productRepository.FindOne(p => p.Id != 0);
    //        _product.ProductAdvices.Count.ShouldEqual(1);

    //        using (var dataContext = GetNewDataContext())
    //        {
    //            var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
    //            advices.Count().ShouldEqual(1);
    //        }

    //        var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
    //        mentors.Count.ShouldEqual(1);
    //        var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
    //        semaphores.Count.ShouldEqual(2);
    //    }

    //    [Test, Ignore]
    //    public void ShouldUpdatePublishedAdviceAndCreateNewUnpublishedVersion()
    //    {
    //        var adviceToUpdate = _adviceApplicationService.AddAdvice<ProductAdvice, Product>(_mentor.Id, _product, _redSemaphore.Id, "some label", "some intro",
    //                                                      "published parent advice", "some keywords", true);

    //        _adviceApplicationService.UpdateAdvice<ProductAdvice>(adviceToUpdate.Id.Value, _greenSemaphore.Id, "", "", "updated published advice child advice",
    //                                                 "", false);

    //        using (var dataContext = GetNewDataContext())
    //        {
    //            var advices = dataContext.GetTable<AdviceBase>().Where(x => x != null);
    //            advices.Count().ShouldEqual(2);

    //            var products = dataContext.GetTable<Product>().Where(p => p != null);
    //            products.Count().ShouldEqual(1);
    //            var product = products.First();
    //            product.ProductAdvices.Count.ShouldEqual(2);
    //            product.ProductAdvices[0].Published.ShouldBeTrue();
    //            product.ProductAdvices[1].Published.ShouldBeFalse();
                
    //        }

    //        var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
    //        mentors.Count.ShouldEqual(1);
    //        var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
    //        semaphores.Count.ShouldEqual(2);
    //    }

    //    [Test, Ignore]
    //    public void ShouldUpdatePublishedAdviceAndCreateNewPublishedVersion()
    //    {
    //        var adviceToUpdate = _adviceApplicationService.AddAdvice<ProductAdvice, Product>(_mentor.Id, _product, _redSemaphore.Id, "some label", "some intro",
    //                                                      "published parent advice", "some keywords", true);

    //        _adviceApplicationService.UpdateAdvice<ProductAdvice>(adviceToUpdate.Id.Value, _greenSemaphore.Id, "", "", "updated published advice child advice",
    //                                                 "", true);

    //        using (var dataContext = GetNewDataContext())
    //        {
    //            var product = dataContext.GetTable<Product>().Where(p => p != null).FirstOrDefault();
    //            product.ProductAdvices.Count.ShouldEqual(2);
    //            product.ProductAdvices[0].Published.ShouldBeFalse();
    //            product.ProductAdvices[0].UnpublishDate.ShouldNotBeNull();
    //            product.ProductAdvices[1].Published.ShouldBeTrue();
    //            product.ProductAdvices[1].PublishDate.ShouldNotBeNull();
    //            var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
    //            advices.Count().ShouldEqual(2);
    //        }

    //        var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
    //        mentors.Count.ShouldEqual(1);
    //        var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
    //        semaphores.Count.ShouldEqual(2);
    //    }

    //    [Test, Ignore]
    //    public void ShouldPublishUnpublishedUpdatedAdvice()
    //    {
    //        var adviceToUpdate = _adviceApplicationService.AddAdvice<ProductAdvice, Product>(_mentor.Id, _product, _redSemaphore.Id, "some label", "some intro",
    //                                                      "published parent advice", "some keywords", true);

    //        var updatedAdvice = _adviceApplicationService.UpdateAdvice<ProductAdvice>(adviceToUpdate.Id.Value, _greenSemaphore.Id, "", "", "updated published advice child advice",
    //                                                 "", false);

    //        _adviceApplicationService.PublishAdvice<ProductAdvice>(updatedAdvice.Id.Value);

    //        using (var dataContext = GetNewDataContext())
    //        {
    //            var product = dataContext.GetTable<Product>().Where(p => p != null).FirstOrDefault();
    //            product.ProductAdvices.Count.ShouldEqual(2);
    //            product.ProductAdvices[0].Published.ShouldBeFalse();
    //            product.ProductAdvices[0].UnpublishDate.ShouldNotBeNull();
    //            product.ProductAdvices[1].Published.ShouldBeTrue();
    //            product.ProductAdvices[1].PublishDate.ShouldNotBeNull();
    //            var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
    //            advices.Count().ShouldEqual(2);
    //        }

    //        var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
    //        mentors.Count.ShouldEqual(1);
    //        var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
    //        semaphores.Count.ShouldEqual(2);
    //    }
    }
}

