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
    public class IngredientSpec : DatabaseSpecBase
    {
        private IIngredientRepository _ingredientRepository;
        private IRepository<Mentor> _mentorRepository;
        private IRepository<Semaphore> _semaphoreRepository;
        private IRepository<IngredientAdvice> _ingredientAdviceRepository;

        private Mentor _mentor;
        private Semaphore _redSemaphore;
        private Semaphore _greenSemaphore;

        protected override void Before_all_specs()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof(Base).Assembly);

            _ingredientAdviceRepository = new Repository<IngredientAdvice>(GetNewDataContext());

            _ingredientRepository = new IngredientRepository(GetNewDataContext());
            _semaphoreRepository = new Repository<Semaphore>(GetNewDataContext());
            _mentorRepository = new Repository<Mentor>(GetNewDataContext());

            _mentor = MentorBuilder.BuildMentor();
            _mentorRepository.Add(_mentor);
            _mentorRepository.Persist();

            _redSemaphore = SemaphoreBuilder.BuildRedSemaphore();
            _semaphoreRepository.Add(_redSemaphore);
            _greenSemaphore = SemaphoreBuilder.BuildGreenSemaphore();
            _semaphoreRepository.Add(_greenSemaphore);
            _semaphoreRepository.Persist();

            base.Before_each_spec();
        }

        protected override void Before_each_spec()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof(Base).Assembly);
            _ingredientAdviceRepository = new Repository<IngredientAdvice>(GetNewDataContext());
            _ingredientRepository = new IngredientRepository(GetNewDataContext());
            _semaphoreRepository = new Repository<Semaphore>(GetNewDataContext());
            _mentorRepository = new Repository<Mentor>(GetNewDataContext());
        }

        [Test(Description = "IntegrationTest")]
        public void ShouldAddIngredient()
        {
            var ingredient = IngredientBuilder.BuildIngredient();
            _ingredientRepository.Add(ingredient);
            _ingredientRepository.Persist();

            _ingredientRepository.FindOne(i => i != null).IngredientName.ShouldEqual(ingredient.IngredientName);

            using (var dataContext = GetNewDataContext())
            {
                var result = dataContext.GetTable<Ingredient>().Single(i => i != null);
                result.IngredientName.ShouldEqual(ingredient.IngredientName);
            }
        }

        [Test(Description = "IntegrationTest")]
        public void ShouldDeleteIngredient()
        {
            var ingredient = IngredientBuilder.BuildIngredient();
            _ingredientRepository.Add(ingredient);
            _ingredientRepository.Persist();

            _ingredientRepository.Delete(ingredient);
            _ingredientRepository.Persist();
            _ingredientRepository.Find(i => i != null).Count().ShouldEqual(0);

            using (var dataContext = GetNewDataContext())
            {
                var result = dataContext.GetTable<Ingredient>().ToList();
                result.Count.ShouldEqual(0);
            }
        }

        [Test(Description = "IntegrationTest")]
        public void ShouldAddTenAdvicesToIngredient()
        {
            var ingredient = IngredientBuilder.BuildIngredient();
            _ingredientRepository.Add(ingredient);
            _ingredientRepository.Persist();

            for (int i = 0; i < 10; i++)
            {
                var newAdvice = AdviceBuilder.BuildAdvice<IngredientAdvice>("Advice_" +i);
                ingredient.AddAdvice(newAdvice);
                _ingredientRepository.Persist();
            }

            _ingredientRepository.FindOne(i => i != null).IngredientAdvices.Count.ShouldEqual(10);

            using (var dataContext = GetNewDataContext())
            {
                var result =
                    dataContext.GetTableForType(typeof (IngredientAdvice)).OfType<IngredientAdvice>().ToList();
                result.Count.ShouldEqual(10);
            }
        }

        [Test(Description = "IntegrationTest")]
        public void ShouldAddTenAdvicesToIngredientAndThenRemoveFive()
        {
            var ingredient = IngredientBuilder.BuildIngredient();
            _ingredientRepository.Add(ingredient);
            _ingredientRepository.Persist();

            for (int i = 0; i < 10; i++)
            {
                var newAdvice = AdviceBuilder.BuildAdvice<IngredientAdvice>("Advice_" + i);
                ingredient.AddAdvice(newAdvice);
                _ingredientRepository.Persist();
            }

            var advices = _ingredientRepository.FindOne(i => i != null).IngredientAdvices;

            for (int i = 0; i < 5; i++)
            {
                var indexCopy = i;
                var adviceToDelete = _ingredientAdviceRepository.FindOne(a => a == advices[indexCopy]);
                _ingredientAdviceRepository.Delete(adviceToDelete);
                _ingredientAdviceRepository.Persist();
            }

            _ingredientRepository.Refresh(ingredient);
            _ingredientRepository.FindOne(i => i != null).IngredientAdvices.Count.ShouldEqual(5);

            using (var dataContext = GetNewDataContext())
            {
                var result =
                    dataContext.GetTableForType(typeof(IngredientAdvice)).OfType<IngredientAdvice>().ToList();
                result.Count.ShouldEqual(5);
            }
        }

    }
}

