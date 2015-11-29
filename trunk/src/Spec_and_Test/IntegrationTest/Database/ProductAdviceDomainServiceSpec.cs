using System.Data.Linq;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.DomainService.Server;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Repository;
using IntegrationTest.HelperClasses;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using ShopGunSpecBase;


namespace IntegrationTest.Database
{
    [TestFixture, Category("Integration")]
    public class ProductAdviceDomainServiceSpec : DatabaseSpecBase
    {
        private IProductRepository _productRepository;
        private IRepository<Mentor> _mentorRepository;
        private IRepository<Semaphore> _semaphoreRepository;
        private IRepository<ProductAdvice> _productAdviceRepository;


        private IProductAdviceDomainService _productAdviceDomainService;

        private ProductBuilder _productBuilder;
        private Mentor _mentor;
        private Semaphore _redSemaphore;
        private Semaphore _greenSemaphore;
        private Product _product;

        protected override void Before_all_specs()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof(Base).Assembly);
            _productBuilder = new ProductBuilder();

            _productAdviceRepository = new Repository<ProductAdvice>(GetNewDataContext());
            _productAdviceDomainService =
                new ProductAdviceDomainService(new ProductRepository(GetNewDataContext()),
                                                   _productAdviceRepository, GetNewDataContext());

            _productRepository = new ProductRepository(GetNewDataContext());
            _semaphoreRepository = new Repository<Semaphore>(GetNewDataContext());
            _mentorRepository = new Repository<Mentor>(GetNewDataContext());     

            _mentor = new Mentor
                          {
                MentorName = "Consumentor"
            };
            _mentorRepository.Add(_mentor);
            _mentorRepository.Persist();

            _redSemaphore = new Semaphore
                             {
                ColorName = "Red",
                Value = -1
            };
            _semaphoreRepository.Add(_redSemaphore);
            _greenSemaphore = new Semaphore
                                  {
                                      ColorName = "Green",
                                      Value = 1
                                  };
            _semaphoreRepository.Add(_greenSemaphore);
            _semaphoreRepository.Persist();

            _product = ProductBuilder.BuildProduct();
            _productRepository.Add(_product);
            _productRepository.Persist();

            base.Before_each_spec();
        }

        protected override void Before_each_spec()
        {
            _productAdviceRepository = new Repository<ProductAdvice>(GetNewDataContext());
            _productAdviceDomainService =
                new ProductAdviceDomainService(new ProductRepository(GetNewDataContext()),
                                                   _productAdviceRepository, GetNewDataContext());

            _productRepository = new ProductRepository(GetNewDataContext());
            _semaphoreRepository = new Repository<Semaphore>(GetNewDataContext());
            _mentorRepository = new Repository<Mentor>(GetNewDataContext());

            using (var dataContext = GetNewDataContext())
            {
                var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
                dataContext.GetTable<AdviceBase>().DeleteAllOnSubmit(advices);
                dataContext.SubmitChanges();
            }
        }

        [Test (Description = "IntegrationTest")]
        public void ShouldAddNewPurchaseAdviceWithNoDuplicateMentorsAndSemaphores()
        {
            var addedAdvice = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                         "Advice 1", "some keywords", false);

            var addedAdvice2 = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                         "Advice 2", "some keywords", false);
            
            addedAdvice.Id.ShouldNotEqual(addedAdvice2.Id);

            _product = _productRepository.FindOne(p => p.Id != 0);
            _product.ProductAdvices.Count.ShouldEqual(2);

            var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
            mentors.Count.ShouldEqual(1);
            var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
            semaphores.Count.ShouldEqual(2);
        }

        [Test]
        public void ShouldPublishAdvice()
        {
            var addedAdvice = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                         "Advice 1", "some keywords", false);

            _product = _productRepository.FindOne(p => p.Id != 0);
            _product.ProductAdvices.Count.ShouldEqual(1);
            var advice = _product.ProductAdvices[0];
            advice.Published.ShouldBeFalse();

            _productAdviceDomainService.PublishProductAdvice(advice);

            using (var dc = GetNewDataContext())
            {
                var product = dc.GetTable<Product>().Where(p => p != null).FirstOrDefault();
                product.ProductAdvices.Count.ShouldEqual(1);
                product.ProductAdvices[0].Published.ShouldBeTrue();
                product.ProductAdvices[0].PublishDate.ShouldNotBeNull();
            }
        }

        [Test]
        public void ShouldPublishUnpublishedAdviceAndCreateNewVersion()
        {
            var addedAdvice = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                         "Advice 1", "some keywords", true);

            _productAdviceDomainService.UnpublishProductAdvice(addedAdvice);

            _product = _productRepository.FindOne(p => p != null);
            _product.ProductAdvices.Count.ShouldEqual(1);
            var advice = _product.ProductAdvices[0];
            advice.Published.ShouldBeFalse();

            _productAdviceDomainService.PublishProductAdvice(advice);

            using (var dc = GetNewDataContext())
            {
                var product = dc.GetTable<Product>().Where(p => p != null).FirstOrDefault();
                product.ProductAdvices.Count.ShouldEqual(2);
                product.ProductAdvices[0].Published.ShouldBeFalse();
                product.ProductAdvices[0].UnpublishDate.ShouldNotBeNull();
                product.ProductAdvices[1].Published.ShouldBeTrue();
                product.ProductAdvices[1].UnpublishDate.ShouldBeNull();
            }
        }

        [Test]
        public void ShouldUnpublishAdvice()
        {
            var addedAdvice = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                         "Advice 1", "some keywords", true);

            _product = _productRepository.FindOne(p => p.Id != 0);
            _product.ProductAdvices.Count.ShouldEqual(1);
            var advice = _product.ProductAdvices[0];
            advice.Published.ShouldBeTrue();

            _productAdviceDomainService.UnpublishProductAdvice(advice);

            using (var dc = GetNewDataContext())
            {
                var product = dc.GetTable<Product>().Where(p => p != null).FirstOrDefault();
                product.ProductAdvices.Count.ShouldEqual(1);
                product.ProductAdvices[0].Published.ShouldBeFalse();
                product.ProductAdvices[0].UnpublishDate.ShouldNotBeNull();
            }
        }

        [Test]
        public void ShouldUpdateUnpublishedAdvice()
        {
            var adviceToUpdate = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                         "unpublished advice", "some keywords", false);
            adviceToUpdate.Advice = "updated Advice";

            _productAdviceDomainService.UpdateProductAdvice(adviceToUpdate, _greenSemaphore, "", "", "updated published advice",
                                                     "", true);

            _product = _productRepository.FindOne(p => p.Id != 0);
            _product.ProductAdvices.Count.ShouldEqual(1);

            using (var dataContext = GetNewDataContext())
            {
                var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
                advices.Count().ShouldEqual(1);
            }

            var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
            mentors.Count.ShouldEqual(1);
            var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
            semaphores.Count.ShouldEqual(2);
        }

        [Test]
        public void ShouldUpdatePublishedAdviceAndCreateNewUnpublishedVersion()
        {
            var adviceToUpdate = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                          "published parent advice", "some keywords", true);

            _productAdviceDomainService.UpdateProductAdvice(adviceToUpdate, _greenSemaphore, "", "", "updated published advice child advice",
                                                     "", false);

            using (var dataContext = GetNewDataContext())
            {
                var product = dataContext.GetTable<Product>().Where(p => p != null).FirstOrDefault();
                product.ProductAdvices.Count.ShouldEqual(2);
                product.ProductAdvices[0].Published.ShouldBeTrue();
                product.ProductAdvices[1].Published.ShouldBeFalse();
                var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
                advices.Count().ShouldEqual(2);
            }

            var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
            mentors.Count.ShouldEqual(1);
            var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
            semaphores.Count.ShouldEqual(2);
        }

        [Test]
        public void ShouldUpdatePublishedAdviceAndCreateNewPublishedVersion()
        {
            var adviceToUpdate = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                          "published parent advice", "some keywords", true);

            _productAdviceDomainService.UpdateProductAdvice(adviceToUpdate, _greenSemaphore, "", "", "updated published advice child advice",
                                                     "", true);

            using (var dataContext = GetNewDataContext())
            {
                var product = dataContext.GetTable<Product>().Where(p => p != null).FirstOrDefault();
                product.ProductAdvices.Count.ShouldEqual(2);
                product.ProductAdvices[0].Published.ShouldBeFalse();
                product.ProductAdvices[0].UnpublishDate.ShouldNotBeNull();
                product.ProductAdvices[1].Published.ShouldBeTrue();
                product.ProductAdvices[1].PublishDate.ShouldNotBeNull();
                var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
                advices.Count().ShouldEqual(2);
            }

            var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
            mentors.Count.ShouldEqual(1);
            var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
            semaphores.Count.ShouldEqual(2);
        }

        [Test]
        public void ShouldPublishUnpublishedUpdatedAdvice()
        {
            var adviceToUpdate = _productAdviceDomainService.AddProductAdvice(_mentor, _product, _redSemaphore, "some label", "some intro",
                                                          "published parent advice", "some keywords", true);

            var updatedAdvice = _productAdviceDomainService.UpdateProductAdvice(adviceToUpdate, _greenSemaphore, "", "", "updated published advice child advice",
                                                     "", false);

            _productAdviceDomainService.PublishProductAdvice(updatedAdvice);

            using (var dataContext = GetNewDataContext())
            {
                var product = dataContext.GetTable<Product>().Where(p => p != null).FirstOrDefault();
                product.ProductAdvices.Count.ShouldEqual(2);
                product.ProductAdvices[0].Published.ShouldBeFalse();
                product.ProductAdvices[0].UnpublishDate.ShouldNotBeNull();
                product.ProductAdvices[1].Published.ShouldBeTrue();
                product.ProductAdvices[1].PublishDate.ShouldNotBeNull();
                var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
                advices.Count().ShouldEqual(2);
            }

            var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
            mentors.Count.ShouldEqual(1);
            var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
            semaphores.Count.ShouldEqual(2);
        }

    }


}
