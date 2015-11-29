using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Brands")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Brand : IAdviceable<BrandAdvice>
    {
        public Brand()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetProducts = new EntitySet<Product>(onAdd => onAdd.Brand = this, onRemove => { onRemove.Brand = null; });
            _entitySetBrandAdvices = new EntitySet<BrandAdvice>(onAdd => onAdd.Brand = this, onRemove => { onRemove.Brand = null; });
            _entitySetBrandStatistics = new EntitySet<BrandStatistic>(onAdd => onAdd.Brand = this, onRemove => { onRemove.Brand = null; });
        }

        [OnDeserializing]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Parameter 'context' of 'DomainObject.OnDeserializing(StreamingContext)' is never used. Remove the parameter or use it in the method body.")]
        private void OnDeserializing(StreamingContext context)
        {
            SetupEntitySets();
        }

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) NOT NULL")]
        [DataMember(Name = "brandName")]
        public string BrandName { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        [DataMember(Name = "logotypeUrl")]
        public string LogotypeUrl { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        [DataMember(Name = "address")]
        public string Address { get; set; }

        [Column(DbType = "NVarChar(50)", CanBeNull = true)]
        [DataMember(Name = "postCode")]
        public string PostCode { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        [DataMember(Name = "city")]
        public string City { get; set; }

        [Column(DbType = "NVarChar(100)", CanBeNull = true)]
        [DataMember(Name = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        [DataMember(Name = "urlToHomePage")]
        public string URLToHomePage { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        [DataMember(Name = "contactEmailAddress")]
        public string ContactEmailAddress { get; set; }

        [Column(DbType = "NVarChar(MAX)", CanBeNull = true)]
        [DataMember(Name = "description")]
        [UIHint("Html")]
        public string Description { get; set; }

        [Column(DbType = "bit", CanBeNull = false)]
        [DataMember(Name = "isMember")]
        public bool IsMember { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        [DataMember(Name = "youtubeVideoId")]
        public string YoutubeVideoId { get; set; }


        //TODO: CanBeNull = true is just temporary!!!
        [Column(CanBeNull = true)]
        public int? CompanyId { get; set; }
        private EntityRef<Company> _entityRefCompany = default(EntityRef<Company>);
        [System.Data.Linq.Mapping.Association(ThisKey = "CompanyId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCompany")]

        [DataMember(Name = "owner")]
        public Company Owner
        {
            get { return _entityRefCompany.Entity; }
            set
            {
                if (value != null)
                {
                    CompanyId = value.Id;
                }
                _entityRefCompany.Entity = value;
            }
        }

        private EntitySet<Product> _entitySetProducts;
        [System.Data.Linq.Mapping.Association(ThisKey = "Id", OtherKey = "BrandId", Storage = "_entitySetProducts")]
        public IList<Product> Products
        {
            get { return _entitySetProducts; }
            set { _entitySetProducts.Assign(value); }
        }

        private EntitySet<BrandAdvice> _entitySetBrandAdvices;
        [System.Data.Linq.Mapping.Association(ThisKey = "Id", OtherKey = "BrandsId", Storage = "_entitySetBrandAdvices")]
        [DataMember(Name = "brandAdvices")]
        public IList<BrandAdvice> BrandAdvices
        {
            get { return _entitySetBrandAdvices; }
            set { _entitySetBrandAdvices.Assign(value); }
        }

        private EntitySet<BrandStatistic> _entitySetBrandStatistics;
        [System.Data.Linq.Mapping.Association(ThisKey = "Id", OtherKey = "BrandId", Storage = "_entitySetBrandStatistics")]
        public IList<BrandStatistic> BrandStatistics
        {
            get { return _entitySetBrandStatistics; }
            set { _entitySetBrandStatistics.Assign(value); }
        }

        [Column]
        [DataMember(Name = "lastUpdated")]
        public DateTime LastUpdated { get; set; }

        public void AddAdvice(BrandAdvice advice)
        {
            _entitySetBrandAdvices.Add(advice);
        }

        public void RemoveAdvice(BrandAdvice adviceToRemove)
        {
            _entitySetBrandAdvices.Remove(adviceToRemove);
        }
    }
}
