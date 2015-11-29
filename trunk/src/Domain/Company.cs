using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Consumentor.ShopGun.Domain.DataTransferObject;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Companies")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Company : IAdviceable<CompanyAdvice>
    {
        public Company()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetCompanyAdvices = new EntitySet<CompanyAdvice>(onAdd => onAdd.Company = this, onRemove => { onRemove.Company = null; });
            _entitySetCompanyStatistics = new EntitySet<CompanyStatistic>(onAdd => onAdd.Company = this, onRemove => { onRemove.Company = null; });
            _entitySetCompanyCSRInfo = new EntitySet<CSRInfo>(onAdd => onAdd.Company = this, onRemove => { onRemove.Company = null; });

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
        [DataMember (Name = "companyName")]
        public string CompanyName { get; set; }

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
        [DataMember(Name = "imageUrlMedium")]
        public string ImageUrlMedium { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = true)]
        [DataMember(Name = "imageUrlLarge")]
        public string ImageUrlLarge { get; set; }

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

        [Column(CanBeNull = true)]
        public int? ParentId { get; set; }
        private EntityRef<Company> _entityRefCompany = default(EntityRef<Company>);
        [System.Data.Linq.Mapping.Association(ThisKey = "ParentId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCompany")]

        [DataMember(Name = "owner")]
        public Company Owner
        {
            get { return _entityRefCompany.Entity; }
            set { _entityRefCompany.Entity = value; }
        }

        [Column(CanBeNull = true)]
        public int? CountryId { get; set; }
        private EntityRef<Country> _entityRefCountry = default(EntityRef<Country>);
        [System.Data.Linq.Mapping.Association(ThisKey = "CountryId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCountry")]

        [DataMember(Name = "country")]
        public Country Country
        {
            get { return _entityRefCountry.Entity; }
            set { _entityRefCountry.Entity = value; }
        }

        private EntitySet<CompanyAdvice> _entitySetCompanyAdvices;
        [System.Data.Linq.Mapping.Association(ThisKey = "Id", OtherKey = "CompanysId", Storage = "_entitySetCompanyAdvices")]
        [DataMember(Name = "companyAdvices")]
        public IList<CompanyAdvice> CompanyAdvices
        {
            get { return _entitySetCompanyAdvices; }
            set { _entitySetCompanyAdvices.Assign(value); }

        }

        private EntitySet<CompanyStatistic> _entitySetCompanyStatistics;
        [System.Data.Linq.Mapping.Association(ThisKey = "Id", OtherKey = "CompanyId", Storage = "_entitySetCompanyStatistics")]
        public IList<CompanyStatistic> CompanyStatistics
        {
            get { return _entitySetCompanyStatistics; }
            set { _entitySetCompanyStatistics.Assign(value); }
        }

        private EntitySet<CSRInfo> _entitySetCompanyCSRInfo;
        [System.Data.Linq.Mapping.Association(ThisKey = "Id", OtherKey = "CompanysId", Storage = "_entitySetCompanyCSRInfo")]
        [DataMember(Name = "csrInfos")]
        public IList<CSRInfo> CSRInfos
        {
            get { return _entitySetCompanyCSRInfo; }
            set { _entitySetCompanyCSRInfo.Assign(value); }

        }

        [Column]
        [DataMember(Name = "lastUpdated")]
        public DateTime LastUpdated { get; set; }

        public void AddAdvice(CompanyAdvice advice)
        {
            _entitySetCompanyAdvices.Add(advice);
        }

        public void RemoveAdvice(CompanyAdvice adviceToRemove)
        {
            _entitySetCompanyAdvices.Remove(adviceToRemove);
        }

        public CompanyDetails ToCompanyDto()
        {
            return new CompanyDetails
                       {
                           Id = Id,
                           CompanyName = CompanyName,
                           Address = Address,
                           City = City,
                           Description = Description,
                           IsMember = IsMember,
                           PostCode = PostCode,
                           Url = URLToHomePage,
                           Email = ContactEmailAddress,
                           ImageUrlLarge = ImageUrlLarge,
                           ImageUrlMedium = ImageUrlMedium,
                           PhoneNumber = PhoneNumber
                       };
        }
    }
}
