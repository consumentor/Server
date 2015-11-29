using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "CertificationMarks")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class CertificationMark
    {
        public CertificationMark()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetProductCertificationMarks =
                new EntitySet<ProductCertificationMark>(onAdd => onAdd.CertificationMark = this,
                                                        onRemove =>
                                                        {
                                                            onRemove.CertificationMark = null;
                                                        });
        }

        [OnDeserializing]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Parameter 'context' of 'DomainObject.OnDeserializing(StreamingContext)' is never used. Remove the parameter or use it in the method body.")]
        private void OnDeserializing(StreamingContext context)
        {
            SetupEntitySets();
        }

        private EntitySet<ProductCertificationMark> _entitySetProductCertificationMarks;
        [Association(ThisKey = "Id", OtherKey = "CertificationMarkId", Storage = "_entitySetProductCertificationMarks")]
        internal IList<ProductCertificationMark> ProductCertificationMarks
        {
            get { return _entitySetProductCertificationMarks; }
            set { _entitySetProductCertificationMarks.Assign(value); }
        }

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember(Name = "certificationName")]
        public string CertificationName { get; set; }

        [Column(CanBeNull = false)]
        [DataMember(Name = "description")]
        public string Description { get; set; }


        [Column(CanBeNull = true)]
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [Column(DbType = "NVarChar(255)")]
        public string Gs1Code { get; set; }
        
        [Column(DbType = "NVarChar(255) not null", CanBeNull = true)]
        [DataMember(Name = "certificationMarkImageUrlSmall")]
        public string CertificationMarkImageUrlSmall { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = true)]
        [DataMember(Name = "certificationMarkImageUrlMedium")]
        public string CertificationMarkImageUrlMedium { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = true)]
        [DataMember(Name = "certificationMarkImageUrlLarge")]
        public string CertificationMarkImageUrlLarge { get; set; }

        [Column(CanBeNull = true)]
        public int? MentorId { get; set; }
        private EntityRef<Mentor> _entityRefMentor = default(EntityRef<Mentor>);
        [Association(ThisKey = "MentorId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefMentor")]
        public Mentor Certifier 
        {
            get { return _entityRefMentor.Entity; } 
            set
            {
                MentorId = value != null ? value.Id : (int?) null;
                _entityRefMentor.Entity = value;
            } 
        }
    }
}
