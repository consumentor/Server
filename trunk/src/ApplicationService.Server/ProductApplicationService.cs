using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Domain.Extensions;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.DomainService.Server.ExternalInformationProvider;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class ProductApplicationService : IProductApplicationService
    {
        public ILogger Log { get; set; }

        private readonly RepositoryFactory _repositoryFactory;
        private readonly IProductRepository _productRepository;
        private readonly IIngredientApplicationService _ingredientApplicationService;
        private readonly IOpvProductInformationDomainService _opvProductInformationDomainService;
        private readonly IGepirProductInformationDomainService _gepirProductInformationDomainService;

        public ProductApplicationService(IOpvProductInformationDomainService opvProductInformationDomainService, IGepirProductInformationDomainService gepirProductInformationDomainService, IProductRepository productRepository, RepositoryFactory repositoryFactory, IIngredientApplicationService ingredientApplicationService)
        {
            _repositoryFactory = repositoryFactory;
            _ingredientApplicationService = ingredientApplicationService;

            _productRepository = productRepository;
            _gepirProductInformationDomainService = gepirProductInformationDomainService;
            _opvProductInformationDomainService = opvProductInformationDomainService;
        }

        private IProductRepository GetProductRepository()
        {
            return _repositoryFactory.Build<IProductRepository, Product>();
        }

        #region CRUD

        public bool DoesProductExist(string gtin)
        {
            bool exists;
            using (var productRepository = GetProductRepository())
            {
                exists = productRepository.Find(p => p.GlobalTradeItemNumber == gtin).Any();
            }
            return exists;
        }

        /// <summary>
        /// Searches for product with <paramref name="globalTradeItemNumber"/> in own and external databases, merges results and returns
        /// the merged product.
        /// </summary>
        /// <param name="globalTradeItemNumber"></param>
        /// <param name="onlyPublishedAdvices"></param>
        /// <returns></returns>
        public Product FindProductByGtin(string globalTradeItemNumber, bool onlyPublishedAdvices)
        {
            if (globalTradeItemNumber.IsGtin())
            {
                var productsToMerge = new List<Product>();

                //var opvProduct = _opvProductInformationDomainService.GetProduct(globalTradeItemNumber);
                //if (opvProduct != null)
                //{
                //    productsToMerge.Add(opvProduct);
                //}

                var mergedProduct = FindProductAndMerge(globalTradeItemNumber, productsToMerge);

                // we just fetch the gtin owner from Gepir and see if we find the respective company in the database. If it exists we indicate on the product the company's id in the producerId field
                if (mergedProduct == null || string.IsNullOrEmpty(mergedProduct.ProductName))
                {
                    Log.Debug("Product not in database - trying to fetch info from GEPIR");
                    var gepirProduct = _gepirProductInformationDomainService.GetProduct(globalTradeItemNumber);
                    if (gepirProduct != null && gepirProduct.Brand != null && gepirProduct.Brand.CompanyId.HasValue)
                    {
                        gepirProduct.ProducerId = gepirProduct.Brand.CompanyId.Value;
                        gepirProduct.Brand = null;
                        mergedProduct = gepirProduct;
                    }
                }

                return mergedProduct;
            }
            return null;
        }

        #endregion


        public Product CreateProduct(Product productToCreate)
        {
            var existingProduct =
                _productRepository.Find(x => x.GlobalTradeItemNumber == productToCreate.GlobalTradeItemNumber).
                    FirstOrDefault();
            if (existingProduct != null)
            {
                Log.Debug("Tried to create a product that already exists: {0} with Id {1}", existingProduct.ProductName, existingProduct.Id);
                throw new ArgumentException("Can't create two products with the same GTIN");
            }
            productToCreate.LastUpdated = DateTime.Now;

            SetBrand(productToCreate, productToCreate.BrandId);
            SetCountry(productToCreate, productToCreate.OriginCountryId);

            _productRepository.Add(productToCreate);
            try
            {
                _productRepository.Persist();
                Log.Debug("Product {0} created with Id {1}", productToCreate.ProductName, productToCreate.Id);
            }
            catch (NotSupportedException e)
            {
                Log.Debug("Product not created: {0}", e.Message);
                throw;
            }

            return productToCreate;
        }

        public Product GetProduct(int productId)
        {
            return _productRepository.FindOne(p => p.Id == productId);
        }

        public Product GetMergedProduct(int productId)
        {
            var gtin = _productRepository.FindOne(p => p.Id == productId).GlobalTradeItemNumber;
            //var opvProduct = _opvProductInformationDomainService.GetProduct(gtin);
            //var gepirProduct = _gepirProductInformationDomainService.GetProduct(gtin);
            var product = FindProductAndMerge(gtin, new List<Product> {/* opvProduct , gepirProduct*/ });
            return product;
        }

        public IList<Product> GetAllProducts()
        {
            return _productRepository.Find(p => p != null).OrderBy(x => x.ProductName).ToList();
        }

        public Product UpdateProduct(Product updatedProduct)
        {
            var productToUpdate = _productRepository.FindOne(b => b.Id == updatedProduct.Id);

            SetBrand(productToUpdate, updatedProduct.BrandId);
            SetCountry(productToUpdate, updatedProduct.OriginCountryId);

            PropertyInfo[] properties = typeof(Product).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                // Only work with strings
                if (p.PropertyType != typeof(string))
                {
                    continue;
                }

                // If not writable then cannot null it; if not readable then cannot check it's value
                if (!p.CanWrite || !p.CanRead)
                {
                    continue;
                }

                MethodInfo mget = p.GetGetMethod(false);
                MethodInfo mset = p.GetSetMethod(false);

                // Get and set methods have to be public
                if (mget == null)
                {
                    continue;
                }
                if (mset == null)
                {
                    continue;
                }

                p.SetValue(productToUpdate, p.GetValue(updatedProduct, null), null);
            }

            productToUpdate.LastUpdated = DateTime.Now;

            _productRepository.Persist();
            Log.Debug("Product {0} with Id {1} updated", productToUpdate.ProductName, productToUpdate.Id);
            return productToUpdate;
        }

        public bool DeleteProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public Product FindProductByGtinInOwnDatabase(string gtin, bool onlyPublishedAdvices)
        {
            try
            {
                var result = _productRepository.Find(p => p.GlobalTradeItemNumber == gtin).FirstOrDefault();
                if (result == null)
                {
                    Log.Debug("No product in database for GTIN {0}", gtin);
                    return null;
                }
                var ingredients = _ingredientApplicationService.FindIngredientsForTableOfContents(result.TableOfContent);
                foreach (var ingredient in ingredients)
                {
                    result.AddIngredient(ingredient);
                }
                if (onlyPublishedAdvices)
                {
                    RemoveNonpublishedAdvices(result);
                }
                return result;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Looks up product by <paramref name="gtin"/> locally and merges information with products in <paramref name="productsToMerge"/>.
        /// </summary>
        /// <param name="gtin">Global Trade Item Number</param>
        /// <param name="productsToMerge"></param>
        /// <remarks>Information is not added up. If local product has data in "TableOfContent" property then this is used, otherwise ToC
        /// from next product and so on.</remarks>
        /// <returns></returns>
        public Product FindProductAndMerge(string gtin, IList<Product> productsToMerge)
        {
            Log.Debug("FindProductAndMerge - {0}.", gtin);
            var myProduct = FindProductByGtinInOwnDatabase(gtin, true);

            if (myProduct == null)
            {
                Log.Debug("No product for GTIN {0} found in own database.", gtin);
                //TODO should we add an empty product into our DB here?
                myProduct = new Product { GlobalTradeItemNumber = gtin, LastUpdated = DateTime.Now };
                //_productRepository.Add(myProduct);_productRepository.Persist();
            }
            else
            {
                Log.Debug("Product with id {0} found for GTIN {1}", myProduct.Id, gtin);
            }


            Product mergedProduct;
            if (productsToMerge != null && productsToMerge.Any())
            {
                Log.Debug("Merging {0} products.", productsToMerge.Count);
                mergedProduct = new Product
                                    {
                                        Id = myProduct.Id,
                                        GlobalTradeItemNumber = myProduct.GlobalTradeItemNumber,
                                        LastUpdated = myProduct.LastUpdated
                                    };

                productsToMerge.Insert(0, myProduct);

                PropertyInfo[] properties = typeof (Product).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo p in properties)
                {
                    // Only work with strings
                    if (p.PropertyType != typeof (string))
                    {
                        continue;
                    }

                    // If not writable then cannot null it; if not readable then cannot check it's value
                    if (!p.CanWrite || !p.CanRead)
                    {
                        continue;
                    }

                    MethodInfo mget = p.GetGetMethod(false);
                    MethodInfo mset = p.GetSetMethod(false);

                    // Get and set methods have to be public
                    if (mget == null)
                    {
                        continue;
                    }
                    if (mset == null)
                    {
                        continue;
                    }

                    foreach (var product in productsToMerge.Where(prod => prod != null))
                    {
                        if (!string.IsNullOrEmpty((string) p.GetValue(product, null)))
                        {
                            p.SetValue(mergedProduct, p.GetValue(product, null), null);
                        }
                    }
                }

                foreach (var product in productsToMerge.Where(p => p != null))
                {
                    if (mergedProduct.Quantity == null)
                    {
                        mergedProduct.Quantity = product.Quantity;
                    }
                    if (mergedProduct.Brand == null)
                    {
                        mergedProduct.Brand = product.Brand;
                    }
                    else if (string.IsNullOrEmpty(mergedProduct.Brand.BrandName) && product.Brand != null)
                    {
                        mergedProduct.Brand.BrandName = product.Brand.BrandName;
                    }

                    if (mergedProduct.Brand != null && product.Brand != null && mergedProduct.Brand.Owner == null)
                    {
                        mergedProduct.Brand.Owner = product.Brand.Owner;
                    }

                    if (mergedProduct.ProductAdvices.Count == 0)
                    {
                        mergedProduct.ProductAdvices = product.ProductAdvices.ToList();
                    }
                    if (mergedProduct.AllergyInformation.Count == 0)
                    {
                        mergedProduct.AllergyInformation = product.AllergyInformation;
                    }
                    if (mergedProduct.CertificationMarks.Count == 0)
                    {
                        foreach (var certificationMark in product.CertificationMarks)
                        {
                            mergedProduct.AddCertificationMark(certificationMark);
                        }
                    }
                    if (mergedProduct.Ingredients.Count == 0)
                    {
                        foreach (var ingredient in product.Ingredients)
                        {
                            mergedProduct.AddIngredient(ingredient);
                        }
                    }
                    if (mergedProduct.ProductCategory == null)
                    {
                        mergedProduct.ProductCategory = product.ProductCategory;
                    }
                }
            }
            else
            {
                Log.Debug("No products to merge.");
                mergedProduct = myProduct;
            }

            RemoveNonpublishedAdvices(mergedProduct);

            if (string.IsNullOrEmpty(mergedProduct.ImageUrlSmall))
            {
                mergedProduct.ImageUrlSmall = @"http://shopgun.se/images/productimages/missing_image_66.jpg";
            }
            if (string.IsNullOrEmpty(mergedProduct.ImageUrlMedium))
            {
                mergedProduct.ImageUrlMedium = @"http://shopgun.se/images/productimages/missing_image_150.jpg";
            }
            if (string.IsNullOrEmpty(mergedProduct.ImageUrlLarge))
            {
                mergedProduct.ImageUrlLarge = @"http://shopgun.se/images/productimages/missing_image_300.jpg";
            }

            return mergedProduct;
        }

        private void RemoveNonpublishedAdvices(Product product)
        {
            Log.Debug("Removing unpublished advices.");
            product.ProductAdvices = product.ProductAdvices.Where(a => a.Published).OrderBy(x => x.Semaphore.Value).ToList();
            if (product.Brand != null)
            {
                product.Brand.BrandAdvices = product.Brand.BrandAdvices.Where(x => x.Published).OrderBy(x => x.Semaphore.Value).ToList();
                if (product.Brand.Owner != null)
                {
                    product.Brand.Owner.CompanyAdvices =
                        product.Brand.Owner.CompanyAdvices.Where(x => x.Published).OrderBy(x => x.Semaphore.Value).ToList();
                }
            }

            if (product.OriginCountry != null)
            {
                product.OriginCountry.CountryAdvices =
                    product.OriginCountry.CountryAdvices.Where(x => x.Published).OrderBy(x => x.Semaphore.Value).ToList();
            }
        }

        public IList<Product> FindProducts(string productName, bool onlyPublishedAdvices)
        {
            //Todo implement with regex
            var products = _productRepository.Find(p => p.ProductName.Equals(productName) ||
                                                       p.ProductName.StartsWith(productName + " ") ||
                                                       p.ProductName.Contains(" " + productName + " ") ||
                                                       p.ProductName.EndsWith(" " + productName)).OrderBy(x => x.ProductName).ToList();
            var result = new List<Product>();

            foreach (var product in products)
            {
                var ingredients = _ingredientApplicationService.FindIngredientsForTableOfContents(product.TableOfContent);
                foreach (var ingredient in ingredients)
                {
                    product.AddIngredient(ingredient);
                }
                if (onlyPublishedAdvices)
                {
                    RemoveNonpublishedAdvices(product);
                }
                result.Add(product);
            }
            return result;
        }

        public IList<Product> GetProductsByBrand(string brand)
        {
            return _productRepository.Find(p => p.Brand.BrandName == brand).OrderBy(x => x.ProductName).ToList();
        }

        #region Ingredient

        public Product AddIngredientToProduct(int productId, int ingredientId)
        {
            var product = _productRepository.FindOne(p => p.Id == productId);
            var ingredient = _productRepository.GetIngredient(ingredientId);
            product.AddIngredient(ingredient);
            _productRepository.MergePersist();
            return product;
        }

        public Product RemoveIngredientFromProduct(int productId, int ingredientId)
        {
            var product = _productRepository.FindOne(p => p.Id == productId);
            var brand = product.Brand;
            var ingredient = _productRepository.GetIngredient(ingredientId);
            product.RemoveIngredient(ingredient);
            _productRepository.DeleteProductIngredientItem(productId, ingredientId);
            _productRepository.MergePersist();
            //TODO Why the hack does product.Brand get set to null here???!!!!
            product.Brand = brand;
            _productRepository.MergePersist();
            return product;
        }

        public Product AddIngredientsToProduct(Product product, IList<Ingredient> ingredients)
        {
            if (ingredients == null)
            {
                ingredients = new List<Ingredient>();
            }
            return AddIngredientsToProduct(product.Id, ingredients.Select(x => x.Id).ToArray());
        }

        public Product AddIngredientsToProduct(int productId, int[] ingredientIds)
        {
            var product = _productRepository.FindOne(p => p.Id == productId);
            foreach (var ingredientId in ingredientIds)
            {
                product.AddIngredient(_productRepository.GetIngredient(ingredientId));
            }
            _productRepository.MergePersist();
            return product;
        }

        #endregion


        // Todo: I think this actually should be managed by the repository /SB
        #region Helper methods
        private void SetCountry(Product productToCreate, int? originCountryId)
        {
            if (originCountryId == null)
            {
                productToCreate.OriginCountry = null;
                return;
            }
            using (var countryRepository = _repositoryFactory.Build<IRepository<Country>, Country>())
            {
                var county = countryRepository.FindOne(x => x.Id == originCountryId);
                productToCreate.OriginCountry = _productRepository.FindDomainObject(county);
            }
        }

        private void SetBrand(Product productToCreate, int? brandId)
        {
            if (brandId == null)
            {
                productToCreate.Brand = null;
                return;
            }
            using (var brandRepository = _repositoryFactory.Build<IRepository<Brand>, Brand>())
            {
                var brand = brandRepository.FindOne(x => x.Id == brandId);
                productToCreate.Brand = _productRepository.FindDomainObject(brand);
            }
        }
        #endregion

        #region CertificationMark

        public Product AddCertificationMarkToProduct(int productId, int certificationMarkId)
        {
            var product = _productRepository.FindOne(p => p.Id == productId);
            var certificationMark = _productRepository.GetCertificationMark(certificationMarkId);
            product.AddCertificationMark(certificationMark);
            _productRepository.MergePersist();
            return product;
        }

        public Product RemoveCertificationMarkFromProduct(int productId, int certificationMarkId)
        {
            var product = _productRepository.FindOne(p => p.Id == productId);
            var brand = product.Brand;
            var certificationMark = _productRepository.GetCertificationMark(certificationMarkId);
            product.RemoveCertificationMark(certificationMark);
            _productRepository.DeleteProductCertificationMarkItem(productId, certificationMarkId);
            _productRepository.MergePersist();
            //TODO Why the hack does product.Brand get set to null here but not product.Country???!!!!
            product.Brand = brand;
            _productRepository.MergePersist();
            return product;
        }

        public Product RemoveAllCertificationMarksFromProduct(int productId)
        {
            var product = _productRepository.FindOne(x => x.Id == productId);
            var certificationMarkIds = product.CertificationMarks.Select(x => x.Id);
            foreach (var certificationMarkId in certificationMarkIds)
            {
                RemoveCertificationMarkFromProduct(productId, certificationMarkId);
            }

            return product;
        }

        public Product GetProductForAdvice(int adviceId)
        {
            Product product;
            using (var productRepository = GetProductRepository())
            {
                product =
                    productRepository.Find(p => p.ProductAdvices.Any(a => a.Id == adviceId)).ToList().FirstOrDefault();
            }
            return product;
        }

        #endregion
    }
}
