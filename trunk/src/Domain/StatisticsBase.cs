using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Statistics")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    [InheritanceMapping(Code = "ProductStatistic", Type = typeof(ProductStatistic), IsDefault = true)]
    [InheritanceMapping(Code = "IngredientStatistic", Type = typeof(IngredientStatistic))]
    [InheritanceMapping(Code = "BrandStatistic", Type = typeof(BrandStatistic))]
    [InheritanceMapping(Code = "CompanyStatistic", Type = typeof(CompanyStatistic))]
    [InheritanceMapping(Code = "CountryStatistic", Type = typeof(CountryStatistic))]
    [InheritanceMapping(Code = "ConceptStatistic", Type = typeof(ConceptStatistic))]
    [InheritanceMapping(Code = "AdviceRequestStatistic", Type = typeof(AdviceRequestStatistic))]
    [KnownType(typeof(ProductStatistic))]
    [KnownType(typeof(IngredientStatistic))]
    [KnownType(typeof(AdviceRequestStatistic))]
    public abstract class StatisticsBase
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false, DbType = DatabaseType.IntIdentity)]
        protected int Id { get; set; }

        [Column(IsDiscriminator = true, CanBeNull = false, DbType = DatabaseType.Nvarchar50)]
        protected virtual string ItemType { get; set; }

        [Column(CanBeNull = true)]
        public int? UserId { get; set; }

        private EntityRef<User> _entityRefUser = default(EntityRef<User>);
        [Association(ThisKey = "UserId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefUser")]
        public User User
        {
            get { return _entityRefUser.Entity; }
            set { _entityRefUser.Entity = value; }
        }

        [Column(CanBeNull = false, DbType = DatabaseType.Datetime2)]
        public DateTime Timestamp { get; set; }

        [Column(CanBeNull = true)]
        public string UserAgent { get; set; }

        [Column(CanBeNull = true)]
        public string Imei { get; set; }

        [Column(CanBeNull = true)]
        public string Model { get; set; }

        [Column(CanBeNull = true)]
        public string OsVersion { get; set; }
    }
}
