using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Web.Security;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Users")]
    public class User
    {
        public User()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetProductStatistics = new EntitySet<ProductStatistic>(onAdd => onAdd.User = this, onRemove => { onRemove.User = null; });
            _entitySetIngredientStatistics = new EntitySet<IngredientStatistic>(onAdd => onAdd.User = this, onRemove => { onRemove.User = null; });
            _entitySetBrandStatistics = new EntitySet<BrandStatistic>(onAdd => onAdd.User = this, onRemove => { onRemove.User = null; });
            _entitySetCompanyStatistics = new EntitySet<CompanyStatistic>(onAdd => onAdd.User = this, onRemove => { onRemove.User = null; });
            _entitySetConceptStatistics = new EntitySet<ConceptStatistic>(onAdd => onAdd.User = this, onRemove => { onRemove.User = null; });
            _entitySetCountryStatistics = new EntitySet<CountryStatistic>(onAdd => onAdd.User = this, onRemove => { onRemove.User = null; });
        }

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(DbType = "varchar(128) not null")]
        public string Password { get; set; }

        [Column (DbType = "varchar(64)")]
        public string FirstName { get; set; }

        [Column (DbType = "varchar(64)")]
        public string LastName { get; set; }

        [Column (DbType = "varchar(50) not null")]
        public string UserName { get; set; }

        [Column(DbType = "varchar(128)")]
        public string DisplayName { get; set; }

        [Column(DbType = "varchar(255) not null")]
        public string Email { get; set; }

        [Column (CanBeNull = true)]
        public bool IsLockedOut { get; set; }

        [Column]
        public DateTime CreationDate { get; set; }

        [Column]
        public DateTime LastLoginDate { get; set; }

        [Column]
        public DateTime LastPasswordChangedDate { get; set; }

        [Column ]
        public DateTime LastActivity { get; set; }

        [Column]
        public DateTime LastLockedOutDate { get; set; }


        [Column(CanBeNull = true)]
        public int? MentorId { get; set; }
        private EntityRef<Mentor> _entityRefMentor = default(EntityRef<Mentor>);

        [Association(IsForeignKey = true, ThisKey = "MentorId", OtherKey = "Id", Storage = "_entityRefMentor")]
        public Mentor Mentor
        {
            get { return _entityRefMentor.Entity; }
            set { _entityRefMentor.Entity = value; }
        }

        //extensions
        public MembershipUser ToMembershipUser()
        {
            MembershipUser membershipUser;
            try
            {
                membershipUser = new MembershipUser("ShopgunMembershipProvider", UserName, Id, Email,
                                      string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now,
                                      DateTime.Now, DateTime.Now, DateTime.Now);
                return membershipUser;
            }
            catch (Exception)
            {
                membershipUser = new MembershipUser("ClientAuthenticationMembershipProvider", UserName, Id, Email,
                                      string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now,
                                      DateTime.Now, DateTime.Now, DateTime.Now);
                return membershipUser;
            }
        }

        private EntitySet<ProductStatistic> _entitySetProductStatistics;
        [Association(ThisKey = "Id", OtherKey = "UserId", Storage = "_entitySetProductStatistics")]
        public IList<ProductStatistic> ProductStatistics
        {
            get { return _entitySetProductStatistics; }
            set { _entitySetProductStatistics.Assign(value); }
        }

        private EntitySet<IngredientStatistic> _entitySetIngredientStatistics;
        [Association(ThisKey = "Id", OtherKey = "UserId", Storage = "_entitySetIngredientStatistics")]
        public IList<IngredientStatistic> IngredientStatistics
        {
            get { return _entitySetIngredientStatistics; }
            set { _entitySetIngredientStatistics.Assign(value); }
        }

        private EntitySet<BrandStatistic> _entitySetBrandStatistics;
        [Association(ThisKey = "Id", OtherKey = "UserId", Storage = "_entitySetBrandStatistics")]
        public IList<BrandStatistic> BrandStatistics
        {
            get { return _entitySetBrandStatistics; }
            set { _entitySetBrandStatistics.Assign(value); }
        }

        private EntitySet<CompanyStatistic> _entitySetCompanyStatistics;
        [Association(ThisKey = "Id", OtherKey = "UserId", Storage = "_entitySetCompanyStatistics")]
        public IList<CompanyStatistic> CompanyStatistics
        {
            get { return _entitySetCompanyStatistics; }
            set { _entitySetCompanyStatistics.Assign(value); }
        }

        private EntitySet<ConceptStatistic> _entitySetConceptStatistics;
        [Association(ThisKey = "Id", OtherKey = "UserId", Storage = "_entitySetConceptStatistics")]
        public IList<ConceptStatistic> ConceptStatistics
        {
            get { return _entitySetConceptStatistics; }
            set { _entitySetConceptStatistics.Assign(value); }
        }

        private EntitySet<CountryStatistic> _entitySetCountryStatistics;
        [Association(ThisKey = "Id", OtherKey = "UserId", Storage = "_entitySetCountryStatistics")]
        public IList<CountryStatistic> CountryStatistics
        {
            get { return _entitySetCountryStatistics; }
            set { _entitySetCountryStatistics.Assign(value); }
        }
       
    }
}
