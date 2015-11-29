using System;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using IntegrationTest.HelperClasses;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using ShopGunSpecBase;

namespace IntegrationTest.Database
{
    [TestFixture, Category("Integration")]
    public class ProductIngredientsSpec : DatabaseSpecBase
    {
        private IRepository<Ingredient> _ingredientRepository;
        private IRepository<Product> _productRepository;
        
        private Product _product;

        protected override void Before_all_specs()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof(Base).Assembly);
            

            var ingredient1 = new Ingredient {IngredientName = "Hop", LastUpdated = DateTime.Now};
            var ingredient2 = new Ingredient {IngredientName = "Malt", LastUpdated = DateTime.Now};
            var ingredient3 = new Ingredient {IngredientName = "Water", LastUpdated = DateTime.Now };

            _ingredientRepository = new Repository<Ingredient>(GetNewDataContext());
            _ingredientRepository.Add(ingredient1);
            _ingredientRepository.Add(ingredient2);
            _ingredientRepository.Add(ingredient3);
            _ingredientRepository.Persist();

            _product = ProductBuilder.BuildProduct();
            _product.AddIngredient(ingredient1);
            _product.AddIngredient(ingredient2);
            _product.AddIngredient(ingredient3);
            _productRepository = new Repository<Product>(GetNewDataContext());
            _productRepository.Add(_product);
            _productRepository.Persist();

            base.Before_each_spec();
        }

        protected override void Before_each_spec()
        {
           
        }

        [Test (Description = "IntegrationTest")]
        public void ShouldFindAProductWithThreeIngredients()
        {
            using (var dataContext = GetNewDataContext())
            {
                var product = dataContext.GetTable<Product>().Where(p => p != null).Single();
                product.Ingredients.Count.ShouldEqual(3);
            }
        }

        [Test]
        public void ShouldFindOneAdviceOnProductIngredient()
        {
            var product = _productRepository.FindOne(p => p != null);
            var ingredientAdvice = AdviceBuilder.BuildAdvice<IngredientAdvice>();
            product.Ingredients[0].IngredientAdvices.Add(ingredientAdvice);
            _productRepository.Persist();

            using (var dataContext = GetNewDataContext())
            {
                var prod = dataContext.GetTable<Product>().Where(p => p != null).Single();
                prod.Ingredients[0].IngredientAdvices.Count.ShouldEqual(1);
            }
        }
    }
}


