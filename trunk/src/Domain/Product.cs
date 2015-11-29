using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Products")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Product : IAdviceable<ProductAdvice>
    {
        public Product()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetProductAdvices = new EntitySet<ProductAdvice>(onAdd => onAdd.Product = this,
                                                                    onRemove => { onRemove.Product = null; });
            _entitySetProductStatistics = new EntitySet<ProductStatistic>(onAdd => onAdd.Product = this,
                                                                          onRemove => { onRemove.Product = null; });
            _entitySetAllergyInformation = new EntitySet<AllergyInformation>(onAdd => onAdd.Product = this,
                                                                             onRemove => { onRemove.Product = null; });
            _entitySetProductIngredients = new EntitySet<ProductIngredient>(onAdd => onAdd.Product = this,
                                                                             onRemove => { onRemove.Product = null; });
            _entitySetProductCertificationMarks =
                new EntitySet<ProductCertificationMark>(onAdd => onAdd.Product = this,
                                                        onRemove => { onRemove.Product = null; });
            //_entitySetProductCategories =
            //    new EntitySet<Product_ProductCategory>(onAdd => onAdd.Product = this,
            //                                           onRemove => onRemove.Product = null);
        }

        [OnDeserializing]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Parameter 'context' of 'DomainObject.OnDeserializing(StreamingContext)' is never used. Remove the parameter or use it in the method body.")]
        private void OnDeserializing(StreamingContext context)
        {
            SetupEntitySets();
        }

        #region Simple datatype properties

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember(Name = "productName")]
        public string ProductName { get; set; }

        [Column(DbType = "NVarChar(14) not null", CanBeNull = false)]
        [DataMember(Name = "globalTradeItemNumber")]
        public string GlobalTradeItemNumber { get; set; }

        [Column]
        [DataMember(Name = "quantity")]
        public int? Quantity { get; set; }

        [Column(DbType = "NVarChar(50)", CanBeNull = true)]
        [DataMember(Name = "quantityUnit")]
        public string QuantityUnit { get; set; }

        [Column(CanBeNull = true)]
        [DataMember(Name = "tableOfContents")]
        public string TableOfContent { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        public string Labels { get; set; }

        [DataMember(Name = "categoryDescription")]
        [Column(CanBeNull = true)]
        public string Description { get; set; }

        [Column]
        [DataMember(Name = "lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [Column]
        [DataMember(Name = "imageUrlSmall")]
        public string ImageUrlSmall { get; set; }

        [Column]
        [DataMember(Name = "imageUrlMedium")]
        public string ImageUrlMedium { get; set; }

        [Column]
        [DataMember(Name = "imageUrlLarge")]
        public string ImageUrlLarge { get; set; }

        [Column]
        [DataMember(Name = "durability")]
        public string Durability { get; set; }

        [Column]
        [DataMember(Name = "supplierArticleNumber")]
        public string SupplierArticleNumber { get; set; }

        [Column]
        [DataMember(Name = "nutrition")]
        public string Nutrition { get; set; }

        [Column]
        [DataMember(Name = "website")]
        public string Website { get; set; }

        [Column(CanBeNull = true)]
        public int? DabasArticleId { get; set; }

        [Column]
        [DataMember(Name = "gpcCode")]
        public string GPCCode { get; set; }


        [Column]
        [DataMember(Name = "originCountryName")]
        public string OriginCountryName { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        [DataMember(Name = "youtubeVideoId")]
        public string YoutubeVideoId { get; set; }

        [DataMember(Name = "producerId")]
        public int ProducerId { get; set; }

        #endregion

        #region EntityRefs

        [Column(CanBeNull = true)]
        public int? BrandId { get; set; }
        private EntityRef<Brand> _entityRefBrand = default(EntityRef<Brand>);
        [Association(ThisKey = "BrandId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefBrand")]
        [DataMember(Name = "brand")]
        public Brand Brand
        {
            get { return _entityRefBrand.Entity; }
            set
            {
                if (value != null)
                {
                    BrandId = value.Id;
                }
                _entityRefBrand.Entity = value;
            }
        }

        [Column(CanBeNull = true)]
        public int? OriginCountryId { get; set; }
        private EntityRef<Country> _entityRefCountry = default(EntityRef<Country>);
        [Association(ThisKey = "OriginCountryId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCountry")]
        [DataMember(Name = "country")]
        public Country OriginCountry
        {
            get { return _entityRefCountry.Entity; }
            set
            {
                if (value != null)
                {
                    OriginCountryId = value.Id;
                }
                _entityRefCountry.Entity = value;
            }
        }

        [Column(CanBeNull = true)]
        public int? ProductCategoryId { get; set; }
        private EntityRef<ProductCategory> _entityRefProductCategory = default(EntityRef<ProductCategory>);
        [Association(ThisKey = "ProductCategoryId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefProductCategory")]
        [DataMember(Name = "productCategory")]
        public ProductCategory ProductCategory
        {
            get { return _entityRefProductCategory.Entity; }
            set
            {
                if (value != null)
                {
                    ProductCategoryId = value.Id;
                }
                _entityRefProductCategory.Entity = value;
            }
        }

        #endregion

        #region EntitySets

        private EntitySet<ProductAdvice> _entitySetProductAdvices;
        [Association(ThisKey = "Id", OtherKey = "ProductsId", Storage = "_entitySetProductAdvices")]
        [DataMember(Name = "productAdvices")]
        public IList<ProductAdvice> ProductAdvices
        {
            get { return _entitySetProductAdvices; }
            set { _entitySetProductAdvices.Assign(value); }
        }

        private EntitySet<ProductStatistic> _entitySetProductStatistics;
        [Association(ThisKey = "Id", OtherKey = "ProductId", Storage = "_entitySetProductStatistics")]
        public IList<ProductStatistic> ProductStatistics
        {
            get { return _entitySetProductStatistics; }
            set { _entitySetProductStatistics.Assign(value); }
        }


        private EntitySet<AllergyInformation> _entitySetAllergyInformation;
        [Association(ThisKey = "Id", OtherKey = "ProductId", Storage = "_entitySetAllergyInformation")]
        [DataMember(Name = "allergyInformation")]
        public IList<AllergyInformation> AllergyInformation
        {
            get { return _entitySetAllergyInformation; }
            set { _entitySetAllergyInformation.Assign(value); }
        }

        #endregion

        #region EntitySets many-to-many

        #region Ingredients

        private EntitySet<ProductIngredient> _entitySetProductIngredients;
        [Association(ThisKey = "Id", OtherKey = "ProductId", Storage = "_entitySetProductIngredients")]
        private EntitySet<ProductIngredient> ProductIngredients
        {
            get { return _entitySetProductIngredients; }
            set { _entitySetProductIngredients.Assign(value); }
        }

        [DataMember(Name = "ingredients")]
        public IList<Ingredient> Ingredients
        {
            get { return ProductIngredients.Select(x => x.Ingredient).ToList(); }
            set
            {
                foreach (var ingredient in value)
                {
                    AddIngredient(ingredient);
                }
            }
        }

        public void AddIngredient(Ingredient ingredient)
        {
            if (_entitySetProductIngredients.Where(x => x.Ingredient == ingredient).Count() != 0)
            {
                return;
            }
            _entitySetProductIngredients.Add(new ProductIngredient
                                                 {
                                                     Product = this,
                                                     ProductId = Id,
                                                     Ingredient = ingredient,
                                                     IngredientId = ingredient.Id
                                                 });
        }

        public void RemoveIngredient(Ingredient ingredient)
        {
            var productIngredient =
                _entitySetProductIngredients.Where(x => x.Ingredient == ingredient);
            if (productIngredient.Count() != 0)
            {
                _entitySetProductIngredients.Remove(productIngredient.First());
            }
        }

        #endregion

        #region CertificationMarks

        private EntitySet<ProductCertificationMark> _entitySetProductCertificationMarks;
        [Association(ThisKey = "Id", OtherKey = "ProductId", Storage = "_entitySetProductCertificationMarks")]
        private EntitySet<ProductCertificationMark> ProductCertificationMarks
        {
            get { return _entitySetProductCertificationMarks; }
            set { _entitySetProductCertificationMarks.Assign(value); }
        }

        [DataMember(Name = "certificationMarks")]
        public IList<CertificationMark> CertificationMarks
        {
            get { return _entitySetProductCertificationMarks.Select(x => x.CertificationMark).ToList(); }
            set
            {
                foreach (var certificationMark in value)
                {
                    AddCertificationMark(certificationMark);
                }
            }
        }

        public void AddCertificationMark(CertificationMark certificationMark)
        {
            if (_entitySetProductCertificationMarks.Where(x => x.CertificationMark == certificationMark).Count() != 0)
            {
                return;
            }
            _entitySetProductCertificationMarks.Add(new ProductCertificationMark
                                                        {
                                                            Product = this,
                                                            ProductId = Id,
                                                            CertificationMark = certificationMark,
                                                            CertificationMarkId = certificationMark.Id
                                                        });
        }

        public void RemoveCertificationMark(CertificationMark certificationMark)
        {
            var productCertificationMarks =
                _entitySetProductCertificationMarks.Where(x => x.CertificationMark == certificationMark);
            if(productCertificationMarks.Count() != 0)
            {
                _entitySetProductCertificationMarks.Remove(productCertificationMarks.First());
            }
        }

        # endregion

        #region ProductCategories

        //private EntitySet<Product_ProductCategory> _entitySetProductCategories;
        //[Association(ThisKey = "Id", OtherKey = "ProductId", Storage = "_entitySetProductCategories")]
        //private EntitySet<Product_ProductCategory> Product_ProductCategories
        //{
        //    get { return _entitySetProductCategories; }
        //    set { _entitySetProductCategories.Assign(value); }
        //}

        //[DataMember(Name = "productCategories")]
        //public IList<ProductCategory> ProductCategories
        //{
        //    get { return _entitySetProductCategories.Select(x => x.ProductCategory).ToList(); }
        //    set
        //    {
        //        foreach (var productCategories in value)
        //        {
        //            AddProductCategory(productCategories);
        //        }
        //    }
        //}

        //public void AddProductCategory(ProductCategory productCategories)
        //{
        //    if (_entitySetProductCategories.Count(x => x.ProductCategory == productCategories) != 0)
        //    {
        //        return;
        //    }
        //    _entitySetProductCategories.Add(new Product_ProductCategory
        //    {
        //        Product = this,
        //        ProductId = Id,
        //        ProductCategory = productCategories,
        //        ProductCategoryId = productCategories.Id
        //    });
        //}

        //public void RemoveProductCategory(ProductCategory productCategory)
        //{
        //    var productProductCategories =
        //        _entitySetProductCategories.Where(x => x.ProductCategory == productCategory);
        //    if (productProductCategories.Count() != 0)
        //    {
        //        _entitySetProductCategories.Remove(productProductCategories.First());
        //    }
        //}

        #endregion

        #endregion

        public void AddAdvice(ProductAdvice advice)
        {
            _entitySetProductAdvices.Add(advice);
        }

        public void RemoveAdvice(ProductAdvice adviceToRemove)
        {
            _entitySetProductAdvices.Remove(adviceToRemove);
        }
    }
}
