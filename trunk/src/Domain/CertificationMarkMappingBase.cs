using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "CertificationMarkMappings")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    [InheritanceMapping(Code = "OpvCertificationMarkMapping", Type = typeof(OpvCertificationMarkMapping), IsDefault = true)]
    [KnownType(typeof(OpvCertificationMarkMapping))]
    public class CertificationMarkMappingBase
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        [Column(IsDiscriminator = true, DbType = "varchar(55) not null")]
        protected virtual string CertificationMarkMappingType { get; set; }

        [Column(CanBeNull = false)]
        public int? CertificationMarkId { get; set; }
        private EntityRef<CertificationMark> _entityRefCertificationMark = default(EntityRef<CertificationMark>);

        [Association(ThisKey = "CertificationMarkId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCertificationMark")]
        public CertificationMark CertificationMark
        {
            get { return _entityRefCertificationMark.Entity; }
            set { _entityRefCertificationMark.Entity = value; }
        }

        [Column(CanBeNull = true)]
        public int? ProviderCertificationId { get; set; }

        [Column(DbType = "NVarChar(255)")]
        public string ProviderCertificationName { get; set; }
    }
}
