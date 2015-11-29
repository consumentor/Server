using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.Gateway;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Mapper
{
    public abstract class ProductMapper<TGateway> : IMapper<Product, TGateway>
    {
        internal readonly RepositoryFactory RepositoryFactory;

        internal ProductMapper(RepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        internal Company MapCompany(string companyName)
        {
            using (var companyRepository = RepositoryFactory.Build<IRepository<Company>, Company>())
            {
                companyRepository.ToggleDeferredLoading(false);
                Company company;
                try
                {
                    company = companyRepository.FindOne(c => c.CompanyName == companyName);
                }
                catch (ArgumentException)
                {
                    company = new Company { CompanyName = companyName, LastUpdated = DateTime.Now };
                }

                return company;
            }
        }

        internal Brand MapBrand(string brandName)
        {
            using (var brandRepository = RepositoryFactory.Build<IRepository<Brand>, Brand>())
            {
                brandRepository.ToggleDeferredLoading(false);

                Brand brand;
                try
                {
                    brand = brandRepository.FindOne(b => b.BrandName == brandName);
                }
                catch (ArgumentException)
                {
                    brand = new Brand { BrandName = brandName, LastUpdated = DateTime.Now };      
                }
                return brand;
            }
        }

        internal IList<Ingredient> MapTableOfContentsToIngredients(string tableOfContents)
        {
            var ingredientList = new List<Ingredient>();
            if (tableOfContents != null)
            {

                using (var ingredientRepository = RepositoryFactory.Build<IIngredientRepository, Ingredient>())
                {
                    ingredientRepository.ToggleDeferredLoading(false);

                    foreach (
                        var ingredientName in
                            tableOfContents.Split(new[] {',', ' ', '(', ')'},
                                                  StringSplitOptions.RemoveEmptyEntries))
                    {
                        var name = ingredientName;
                        var ingredient = ingredientRepository.Find(i => i.IngredientName.Equals(name)).FirstOrDefault();
                        if (ingredient != null && !ingredientList.Contains(ingredient))
                        {
                            ingredient.IngredientAdvices =
                                ingredient.IngredientAdvices.Where(a => a.Published).ToList();

                            ingredientList.Add(ingredient);
                        }
                    }
                }
            }
            return ingredientList;
        }

        internal Ingredient MapIngredient(string ingredientName)
        {
            Ingredient ingredient;
            using (var ingredientRepository = RepositoryFactory.Build<IIngredientRepository, Ingredient>())
            {
                ingredientRepository.ToggleDeferredLoading(false);

                try
                {
                    ingredient = ingredientRepository.FindOne(i => i.IngredientName == ingredientName);
                }
                catch (ArgumentException)
                {
                    ingredient = new Ingredient{IngredientName = ingredientName, LastUpdated = DateTime.Now};
                }
            }
            return ingredient;
        }

        internal Country MapOriginCountry(string countryName)
        {
            Country country;
            try
            {
                using (var countryRepository = RepositoryFactory.Build<IRepository<Country>, Country>())
                {
                    countryRepository.ToggleDeferredLoading(false);

                    country = countryRepository.FindOne(c => c.CountryCode.Name == countryName);
                }
            }
            catch (ArgumentException)
            {
                country = new Country{CountryCode = new CountryCode{Name = countryName}, LastUpdated = DateTime.Now};
            }
            return country;
        }

        public abstract TGateway Map(Product source);
        public abstract Product Map(TGateway source);
    }
}
