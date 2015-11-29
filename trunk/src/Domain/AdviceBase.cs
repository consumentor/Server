using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using System.Web;
using System.Linq;
using LinqAssociation = System.Data.Linq.Mapping.AssociationAttribute;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Advices")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    [InheritanceMapping(Code = "CountryAdvice", Type = typeof(CountryAdvice))]
    [InheritanceMapping(Code = "CompanyAdvice", Type = typeof(CompanyAdvice))]
    [InheritanceMapping(Code = "BrandAdvice", Type = typeof(BrandAdvice))]
    [InheritanceMapping(Code = "ConceptAdvice", Type = typeof(ConceptAdvice))]
    [InheritanceMapping(Code = "IngredientAdvice", Type = typeof(IngredientAdvice))]
    [InheritanceMapping(Code = "ProductAdvice", Type = typeof(ProductAdvice), IsDefault = true)]
    [KnownType(typeof(CountryAdvice))]
    [KnownType(typeof(CompanyAdvice))]
    [KnownType(typeof(BrandAdvice))]
    [KnownType(typeof(ConceptAdvice))]
    [KnownType(typeof(IngredientAdvice))]
    [KnownType(typeof(ProductAdvice))]
    public abstract class AdviceBase
    {
        public AdviceBase()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetAdviceRequestStatistics = new EntitySet<AdviceRequestStatistic>(onAdd => onAdd.Advice = this, onRemove => { onRemove.Advice = null; });
            _entitySetUserAdviceRatings = new EntitySet<UserAdviceRating>(onAdd => onAdd.Advice = this, onRemove => { onRemove.Advice = null; });
        }

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        [Column(IsDiscriminator = true, DbType = "varchar(55) not null")]
        protected virtual string AdviceType { get; set; }

        [DataMember(Name = "itemName")]
        public abstract string ItemName { get; internal set; }

        [Column(CanBeNull = false)]
        [DataMember(Name = "mentorId")]
        public int? MentorId { get; set; }
        private EntityRef<Mentor> _entityRefMentor = default(EntityRef<Mentor>);

        [System.Data.Linq.Mapping.Association(ThisKey = "MentorId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefMentor")]
        [DataMember (Name = "mentor")]
        public Mentor Mentor
        {
            get { return _entityRefMentor.Entity; }
            set { _entityRefMentor.Entity = value; }
        }

        [DataMember(Name = "mentorName")]
        public string MentorName { 
            get { return Mentor.MentorName; }
            private set {}
        }

        [Column(DbType = "NVarChar(255)", CanBeNull = false)]
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [Column(CanBeNull = false)]
        [DataMember(Name = "introduction")]
        public string Introduction { get; set; }

        [Column(CanBeNull = false)]
        [DataMember(Name = "advice")]
        [DataType(DataType.Html)]
        [UIHint("Html")]
        public string Advice { get; set; }

        
        public string HtmlDecodedAdvice
        {
            get { return HttpUtility.HtmlDecode(Advice); }
            set { this.Advice = HttpUtility.HtmlDecode(value); }
        }

        [Column(CanBeNull = true)]
        public string KeyWords { get; set; }

        
        [Column(CanBeNull = false)]
        public int? SemaphoreId { get; set; }
        private EntityRef<Semaphore> _entityRefSemaphore = default(EntityRef<Semaphore>);

        [DisplayName("Signal")]
        [LinqAssociation(ThisKey = "SemaphoreId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefSemaphore")]
        [DataMember(Name = "semaphore")]
        public Semaphore Semaphore
        {
            get { return _entityRefSemaphore.Entity; }
            set { _entityRefSemaphore.Entity = value; }
        }

        [DataMember(Name = "semaphoreValue")]
        public int SemaphoreValue { get { return Semaphore.Value; } private set{} }

        [Column(CanBeNull = false)]
        public bool Published { get; set; }

        [Column(CanBeNull = true, DbType = DatabaseType.Datetime2)]
        [DataMember(Name = "publishdate")]
        public DateTime? PublishDate { get; set; }

        [Column(CanBeNull = true, DbType = DatabaseType.Datetime2)]
        [DataMember(Name = "unpublishdate")]
        public DateTime? UnpublishDate { get; set; }

        [Column]
        protected int? PreviousVersionId { get; set; }
        private EntityRef<AdviceBase> _entityRefPreviousVersion = default(EntityRef<AdviceBase>);

        [LinqAssociation(ThisKey = "PreviousVersionId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefPreviousVersion")]
        public AdviceBase PreviousVersion
        {
            get { return _entityRefPreviousVersion.Entity; }
            protected set { _entityRefPreviousVersion.Entity = value; }
        }

        [Column]
        public int? TagId { get; set; }
        private EntityRef<AdviceTag> _entityRefAdviceTag = default(EntityRef<AdviceTag>);

        [LinqAssociation(ThisKey = "TagId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefAdviceTag")]
        public AdviceTag Tag
        {
            get { return _entityRefAdviceTag.Entity; }
            set { _entityRefAdviceTag.Entity = value; }
        }

        [DataMember(Name = "tag")]
        public string TagName
        {
            get { return Tag != null ? Tag.Name : ""; }
            private set { if (value == null) throw new ArgumentNullException("value"); }
        }

        protected abstract object CreateInstanceOfSameTypeAsThis();
        public virtual T CreateNewVersion<T>() where T : class
        {
            var clone = Clone() as AdviceBase;
            // ReSharper disable PossibleNullReferenceException
            clone.PreviousVersion = this;
            return clone as T;
        }
        public object Clone()
        {
            var clone = CreateInstanceOfSameTypeAsThis() as AdviceBase;
            VerifyThatCloneIsOfDefectBaseType(clone);
            SetClonedData(clone);
            return clone;
        }

        public T Clone<T>() where T : class, new()
        {
            return Clone() as T;
        }

        private void VerifyThatCloneIsOfDefectBaseType<T>(T clone)
        {
            var cloneAsDefectBase = clone as AdviceBase;
            if (cloneAsDefectBase == null)
                throw new ArgumentException("Type T must inherit from AdviceBase");
        }

        protected virtual void SetClonedData(AdviceBase clone)
        {
            clone.AdviceType = AdviceType;
            clone.Label = Label;
            clone.Introduction = Introduction;
            clone.Advice = Advice;
            clone.KeyWords = KeyWords;
            clone.Mentor = Mentor;
            clone.Semaphore = Semaphore;

            //clone.Published = Published;
            //clone.PublishDate = PublishDate;
            //clone.UnpublishDate = UnpublishDate;
        }

        private EntitySet<AdviceRequestStatistic> _entitySetAdviceRequestStatistics;
        [LinqAssociation(ThisKey = "Id", OtherKey = "AdviceId", Storage = "_entitySetAdviceRequestStatistics")]
        public IList<AdviceRequestStatistic> AdviceRequestStatistics
        {
            get { return _entitySetAdviceRequestStatistics; }
            set { _entitySetAdviceRequestStatistics.Assign(value); }
        }

        private EntitySet<UserAdviceRating> _entitySetUserAdviceRatings;
        [LinqAssociation(ThisKey = "Id", OtherKey = "AdviceId", Storage = "_entitySetUserAdviceRatings")]
        public IList<UserAdviceRating> UserAdviceRatings
        {
            get { return _entitySetUserAdviceRatings; }
            set { _entitySetUserAdviceRatings.Assign(value); }
        }

        [DataMember(Name = "adviceScore")]
        public int AdviceScore
        {
            get { return UserAdviceRatings.Sum(x => x.Rating); }
            private set { }
        }
    }
}
