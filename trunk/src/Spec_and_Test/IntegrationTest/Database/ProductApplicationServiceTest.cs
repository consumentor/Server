using System.Collections.Generic;
using System.Linq;
using Consumentor.ShopGun.ApplicationService.Server;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
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
    public class ProductDomainServiceTest : DatabaseSpecBase
    {
        private IProductApplicationService _productApplicationService;
        private IProductRepository _productRepository;
        private Brand _brand;
        private Country _country;
        private Mentor _mentor;
        private CertificationMark _certificationMark;
        private Ingredient _ingredient;

        protected override void Before_all_specs()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof(Base).Assembly);

            _productRepository = new ProductRepository(GetNewDataContext());
            _productApplicationService = new ProductApplicationService(null, null, null, null, null);

            _brand = BrandBuilder.BuildBrand();
            using (var brandRepository = new Repository<Brand>(GetNewDataContext()))
            {
                brandRepository.Add(_brand);
                brandRepository.Persist();
            }

            _country = CountryBuilder.BuildCountry();
            using (var countryRepository = new Repository<Country>(GetNewDataContext()))
            {
                countryRepository.Add(_country);
                countryRepository.Persist();
            }

            _mentor = MentorBuilder.BuildMentor();
            _certificationMark = CertificationMarkBuilder.BuildCertificationMark(_mentor);
            using (var certificationMarkRepository = new Repository<CertificationMark>(GetNewDataContext()))
            {
                certificationMarkRepository.Add(_certificationMark);
                certificationMarkRepository.Persist();
            }

            _ingredient = IngredientBuilder.BuildIngredient();
            using (var ingredientRepository = new Repository<Ingredient>(GetNewDataContext()))
            {
                ingredientRepository.Add(_ingredient);
                ingredientRepository.Persist();
            }
        }

        protected override void Before_each_spec()
        {
        }


        [Test (Description = "IntegrationTest")]
        public void ShouldCreateNewProduct()
        {
            var product = ProductBuilder.BuildProduct(_brand, _country);
            product = _productApplicationService.CreateProduct(product);
            product.Id.ShouldNotBeTheSameAs(0);
        }

        //[Test (Description = "IntegrationTest")]
        //public void ShouldAddCertificationMarkToProduct()
        //{
        //    var product = ProductBuilder.BuildProduct(_brand, _country);
        //    product = _productApplicationService.CreateProduct(product);
        //    _productApplicationService.AddCertificationMarkToProduct(product.Id, _certificationMark.Id);

        //    product.CertificationMarks.Count.ShouldEqual(1);
        //}

        //[Test(Description = "IntegrationTest")]
        //public void ShouldRemoveCertificationMarkFromProduct()
        //{
        //    var product = ProductBuilder.BuildProduct(_brand, _country);
        //    product = _productApplicationService.CreateProduct(product);
        //    _productApplicationService.AddCertificationMarkToProduct(product.Id, _certificationMark.Id);

        //    _productApplicationService.RemoveCertificationMarkFromProduct(product.Id, _certificationMark.Id);
        //    product.CertificationMarks.Count.ShouldEqual(0);
        //}

        //[Test(Description = "IntegrationTest")]
        //public void ShouldAddCertificationMarkOnlyOnce()
        //{
        //    var product = ProductBuilder.BuildProduct(_brand, _country);
        //    product = _productApplicationService.CreateProduct(product);
        //    _productApplicationService.AddCertificationMarkToProduct(product.Id, _certificationMark.Id);
        //    _productApplicationService.AddCertificationMarkToProduct(product.Id, _certificationMark.Id);

        //    product.CertificationMarks.Count().ShouldEqual(1);
        //}


        //[Test(Description = "IntegrationTest")]
        //public void ShouldAddIngredientToProduct()
        //{
        //    var product = ProductBuilder.BuildProduct(_brand, _country);
        //    product = _productApplicationService.CreateProduct(product);
        //    _productApplicationService.AddIngredientToProduct(product.Id, _ingredient.Id);

        //    product.Ingredients.Count.ShouldEqual(1);
        //    using (IRepository<ProductIngredient> productIngredientRepository = new Repository<ProductIngredient>(GetNewDataContext()))
        //    {
        //        var result =
        //            productIngredientRepository.Find(x => x.ProductId == product.Id && x.IngredientId == _ingredient.Id);
        //        result.Count().ShouldEqual(1);
        //    }
        //}

        //[Test(Description = "IntegrationTest")]
        //public void ShouldRemoveIngredientFromProduct()
        //{
        //    var product = ProductBuilder.BuildProduct(_brand, _country);
        //    product = _productApplicationService.CreateProduct(product);
        //    _productApplicationService.AddIngredientToProduct(product.Id, _ingredient.Id);

        //    _productApplicationService.RemoveIngredientFromProduct(product.Id, _ingredient.Id);
        //    product.Ingredients.Count.ShouldEqual(0);
            
        //    using (IRepository<ProductIngredient> productIngredientRepository = new Repository<ProductIngredient>(GetNewDataContext()))
        //    {
        //        var result =
        //            productIngredientRepository.Find(x => x.ProductId == product.Id && x.IngredientId == _ingredient.Id);
        //        result.Count().ShouldEqual(0);
        //    }
        //}

        //[Test(Description = "IntegrationTest")]
        //public void ShouldAddIngredientOnlyOnce()
        //{
        //    var product = ProductBuilder.BuildProduct(_brand, _country);
        //    product = _productApplicationService.CreateProduct(product);
        //    _productApplicationService.AddIngredientToProduct(product.Id, _ingredient.Id);
        //    _productApplicationService.AddIngredientToProduct(product.Id, _ingredient.Id);

        //    product.Ingredients.Count().ShouldEqual(1);
        //}

        //[Test(Description = "IntergrationTest")]
        //public void ShouldAddIngredientsList()
        //{
        //    var product = ProductBuilder.BuildProduct(_brand, _country);
        //    product = _productApplicationService.CreateProduct(product);

        //    var ingredient1 = IngredientBuilder.BuildIngredient("Ingredient1");
        //    var ingredient2 = IngredientBuilder.BuildIngredient("Ingredient2");
        //    using (var ingredientRepository = new Repository<Ingredient>(GetNewDataContext()))
        //    {
        //        ingredientRepository.Add(ingredient1);
        //        ingredientRepository.Add(ingredient2);
        //        ingredientRepository.Persist();
        //    }
        //    var ingredients = new List<Ingredient> {ingredient1, ingredient2};

        //    _productApplicationService.AddIngredientsToProduct(product, ingredients);

        //    product.Ingredients.Count.ShouldEqual(2);
        //    product.Ingredients.Select(x => x.Id).ShouldContain(ingredient1.Id);
        //    product.Ingredients.Select(x => x.Id).ShouldContain(ingredient2.Id);
        //}
    }
}
