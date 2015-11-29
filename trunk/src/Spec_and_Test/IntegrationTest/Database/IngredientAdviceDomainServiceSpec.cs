using System.Linq;
using Consumentor.ShopGun.Configuration;
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
    public class IngredientAdviceDomainServiceSpec : DatabaseSpecBase
    {
        private IIngredientRepository _ingredientRepository;
        private IRepository<Mentor> _mentorRepository;
        private IRepository<Semaphore> _semaphoreRepository;
        private IRepository<IngredientAdvice> _ingredientAdviceRepository;


        private IIngredientAdviceDomainService _ingredientAdviceDomainService;

        private Mentor _mentor;
        private Semaphore _redSemaphore;
        private Semaphore _greenSemaphore;
        private Ingredient _ingredient;

        protected override void Before_all_specs()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof(Base).Assembly);

            IConfiguration configuration = new BasicConfiguration();
            var container = configuration.Container;

            _ingredientRepository = new IngredientRepository(GetNewDataContext());
            _semaphoreRepository = new Repository<Semaphore>(GetNewDataContext());
            _mentorRepository = new Repository<Mentor>(GetNewDataContext());

            _ingredientAdviceRepository = new Repository<IngredientAdvice>(GetNewDataContext());
            _ingredientAdviceDomainService = new IngredientAdviceDomainService(_ingredientRepository,
                                                                               _ingredientAdviceRepository,
                                                                               GetNewDataContext());

           

            _mentor = MentorBuilder.BuildMentor();
            _mentorRepository.Add(_mentor);
            _mentorRepository.Persist();

            _redSemaphore = SemaphoreBuilder.BuildRedSemaphore();
            _semaphoreRepository.Add(_redSemaphore);
            _greenSemaphore = SemaphoreBuilder.BuildGreenSemaphore();
            _semaphoreRepository.Add(_greenSemaphore);
            _semaphoreRepository.Persist();

            _ingredient = IngredientBuilder.BuildIngredient();
            _ingredientRepository.Add(_ingredient);
            _ingredientRepository.Persist();

            base.Before_each_spec();
        }

        protected override void Before_each_spec()
        {
            _ingredientRepository = new IngredientRepository(GetNewDataContext());
            _semaphoreRepository = new Repository<Semaphore>(GetNewDataContext());
            _mentorRepository = new Repository<Mentor>(GetNewDataContext());

            _ingredientAdviceRepository = new Repository<IngredientAdvice>(GetNewDataContext());
            _ingredientAdviceDomainService = new IngredientAdviceDomainService(_ingredientRepository,
                                                                               _ingredientAdviceRepository,
                                                                               GetNewDataContext());

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
            var addedAdvice = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
                                                         "Advice 1", "some keywords", false);

            var addedAdvice2 = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
                                                         "Advice 2", "some keywords", false);
            
            addedAdvice.Id.ShouldNotEqual(addedAdvice2.Id);

            _ingredient = _ingredientRepository.FindOne(p => p.Id != 0);
            _ingredient.IngredientAdvices.Count.ShouldEqual(2);

            var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
            mentors.Count.ShouldEqual(1);
            var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
            semaphores.Count.ShouldEqual(2);
        }

        [Test]
        public void ShouldPublishAdvice()
        {
            var addedAdvice = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
                                                         "Advice 1", "some keywords", false);

            _ingredient = _ingredientRepository.FindOne(p => p.Id != 0);
            _ingredient.IngredientAdvices.Count.ShouldEqual(1);
            var advice = _ingredient.IngredientAdvices[0];
            advice.Published.ShouldBeFalse();

            _ingredientAdviceDomainService.PublishIngredientAdvice(advice);

            using (var dc = GetNewDataContext())
            {
                var ingredient = dc.GetTable<Ingredient>().Where(p => p != null).FirstOrDefault();
                ingredient.IngredientAdvices.Count.ShouldEqual(1);
                ingredient.IngredientAdvices[0].Published.ShouldBeTrue();
                ingredient.IngredientAdvices[0].PublishDate.ShouldNotBeNull();
            }
        }

        //[Test]
        //public void ShouldPublishUnpublishedAdviceAndCreateNewVersion()
        //{
        //    var addedAdvice = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
        //                                                 "Advice 1", "some keywords", true);

        //    _ingredientAdviceDomainService.UnpublishIngredientAdvice(addedAdvice);

        //    _ingredient = _ingredientRepository.FindOne(p => p != null);
        //    _ingredient.IngredientAdvices.Count.ShouldEqual(1);
        //    var advice = _ingredient.IngredientAdvices[0];
        //    advice.Published.ShouldBeFalse();

        //    _ingredientAdviceDomainService.PublishIngredientAdvice(advice);

        //    using (var dc = GetNewDataContext())
        //    {
        //        var ingredient = dc.GetTable<Ingredient>().Where(p => p != null).FirstOrDefault();
        //        ingredient.IngredientAdvices.Count.ShouldEqual(2);
        //        ingredient.IngredientAdvices[0].Published.ShouldBeFalse();
        //        ingredient.IngredientAdvices[0].UnpublishDate.ShouldNotBeNull();
        //        ingredient.IngredientAdvices[1].Published.ShouldBeTrue();
        //        ingredient.IngredientAdvices[1].UnpublishDate.ShouldBeNull();
        //    }
        //}

        [Test]
        public void ShouldUnpublishAdvice()
        {
            var addedAdvice = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
                                                         "Advice 1", "some keywords", true);

            _ingredient = _ingredientRepository.FindOne(p => p.Id != 0);
            _ingredient.IngredientAdvices.Count.ShouldEqual(1);
            var advice = _ingredient.IngredientAdvices[0];
            advice.Published.ShouldBeTrue();

            _ingredientAdviceDomainService.UnpublishIngredientAdvice(advice);

            using (var dc = GetNewDataContext())
            {
                var ingredient = dc.GetTable<Ingredient>().Where(p => p != null).FirstOrDefault();
                ingredient.IngredientAdvices.Count.ShouldEqual(1);
                ingredient.IngredientAdvices[0].Published.ShouldBeFalse();
                ingredient.IngredientAdvices[0].UnpublishDate.ShouldNotBeNull();
            }
        }

        [Test]
        public void ShouldUpdateAdvice()
        {
            var adviceToUpdate = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
                                                         "unpublished advice", "some keywords", false);
            adviceToUpdate.Advice = "updated Advice";

            _ingredientAdviceDomainService.UpdateIngredientAdvice(adviceToUpdate, _greenSemaphore, "", "", "updated published advice",
                                                     "", true);

            _ingredient = _ingredientRepository.FindOne(p => p.Id != 0);
            _ingredient.IngredientAdvices.Count.ShouldEqual(1);

            using (var dataContext = GetNewDataContext())
            {
                var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
                advices.Count().ShouldEqual(1);
            }

            var mentors = _mentorRepository.Find(x => x != null).ToList();
            mentors.Count.ShouldEqual(1);
            var semaphores = _semaphoreRepository.Find(x => x != null).ToList();
            semaphores.Count.ShouldEqual(2);
        }

        //[Test]
        //public void ShouldUpdatePublishedAdviceAndCreateNewUnpublishedVersion()
        //{
        //    var adviceToUpdate = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
        //                                                  "published parent advice", "some keywords", true);

        //    _ingredientAdviceDomainService.UpdateIngredientAdvice(adviceToUpdate, _greenSemaphore, "", "", "updated published advice child advice",
        //                                             "", false);

        //    using (var dataContext = GetNewDataContext())
        //    {
        //        var ingredient = dataContext.GetTable<Ingredient>().Where(p => p != null).FirstOrDefault();
        //        ingredient.IngredientAdvices.Count.ShouldEqual(2);
        //        ingredient.IngredientAdvices[0].Published.ShouldBeTrue();
        //        ingredient.IngredientAdvices[1].Published.ShouldBeFalse();
        //        var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
        //        advices.Count().ShouldEqual(2);
        //    }

        //    var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
        //    mentors.Count.ShouldEqual(1);
        //    var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
        //    semaphores.Count.ShouldEqual(2);
        //}

        //[Test]
        //public void ShouldUpdatePublishedAdviceAndCreateNewPublishedVersion()
        //{
        //    var adviceToUpdate = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
        //                                                  "published parent advice", "some keywords", true);

        //    _ingredientAdviceDomainService.UpdateIngredientAdvice(adviceToUpdate, _greenSemaphore, "", "", "updated published advice child advice",
        //                                             "", true);

        //    using (var dataContext = GetNewDataContext())
        //    {
        //        var ingredient = dataContext.GetTable<Ingredient>().Where(p => p != null).FirstOrDefault();
        //        ingredient.IngredientAdvices.Count.ShouldEqual(2);
        //        ingredient.IngredientAdvices[0].Published.ShouldBeFalse();
        //        ingredient.IngredientAdvices[0].UnpublishDate.ShouldNotBeNull();
        //        ingredient.IngredientAdvices[1].Published.ShouldBeTrue();
        //        ingredient.IngredientAdvices[1].PublishDate.ShouldNotBeNull();
        //        var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
        //        advices.Count().ShouldEqual(2);
        //    }

        //    var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
        //    mentors.Count.ShouldEqual(1);
        //    var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
        //    semaphores.Count.ShouldEqual(2);
        //}

        //[Test]
        //public void ShouldPublishUnpublishedUpdatedAdvice()
        //{
        //    var adviceToUpdate = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
        //                                                  "published parent advice", "some keywords", true);

        //    var updatedAdvice = _ingredientAdviceDomainService.UpdateIngredientAdvice(adviceToUpdate, _greenSemaphore, "", "", "updated published advice child advice",
        //                                             "", false);

        //    _ingredientAdviceDomainService.PublishIngredientAdvice(updatedAdvice);

        //    using (var dataContext = GetNewDataContext())
        //    {
        //        var ingredient = dataContext.GetTable<Ingredient>().Where(p => p != null).FirstOrDefault();
        //        ingredient.IngredientAdvices.Count.ShouldEqual(2);
        //        ingredient.IngredientAdvices[0].Published.ShouldBeFalse();
        //        ingredient.IngredientAdvices[0].UnpublishDate.ShouldNotBeNull();
        //        ingredient.IngredientAdvices[1].Published.ShouldBeTrue();
        //        ingredient.IngredientAdvices[1].PublishDate.ShouldNotBeNull();
        //        var advices = dataContext.GetTable<AdviceBase>().Where(x => x.Id != 0);
        //        advices.Count().ShouldEqual(2);
        //    }

        //    var mentors = _mentorRepository.Find(x => !x.MentorName.Equals(string.Empty)).ToList();
        //    mentors.Count.ShouldEqual(1);
        //    var semaphores = _semaphoreRepository.Find(x => !x.Value.Equals(-99)).ToList();
        //    semaphores.Count.ShouldEqual(2);
        //}

        [Test]
        public void ShouldDeleteAdvice()
        {
            var addedAdvice = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
                                                         "Advice 1", "some keywords", false);

            var addedAdvice2 = _ingredientAdviceDomainService.AddIngredientAdvice(_mentor, _ingredient, _redSemaphore, "some label", "some intro",
                                                         "Advice 2", "some keywords", false);

            _ingredientRepository.FindOne(i => i != null).IngredientAdvices.Count.ShouldEqual(2);

            _ingredientAdviceDomainService.DeleteIngredientAdviceById((int) addedAdvice.Id);

            using (var dataContext = GetNewDataContext())
            {
                var ingredient = dataContext.GetTable<Ingredient>().Where(p => p != null).FirstOrDefault();
                ingredient.IngredientAdvices.Count.ShouldEqual(1);
                ingredient.IngredientAdvices[0].Id.ShouldEqual(addedAdvice2.Id);
            }

            var ingred = _ingredientRepository.FindOne(i => i != null);
            ingred.IngredientAdvices.Count.ShouldEqual(1);            
        }

    }


}

