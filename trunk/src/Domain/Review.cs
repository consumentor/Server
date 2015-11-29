using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Reviews")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Review
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        protected int Id { get; set; }

        [Column(CanBeNull = false)]
        protected int? ProductId { get; set; }
        private EntityRef<Product> _entityRefProduct = default(EntityRef<Product>);
        [Association(ThisKey = "ProductId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefProduct")]

        [DataMember]
        public Product Product
        {
            get { return _entityRefProduct.Entity; }
            set { _entityRefProduct.Entity = value; }
        }
        
        [Column(CanBeNull = false)]
        protected int? OrganizationId { get; set; }
        private EntityRef<Organization> _entityRefOrganization = default(EntityRef<Organization>);
        [Association(ThisKey = "OrganizationId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefOrganization")]

        [DataMember]
        public Organization Organization
        {
            get { return _entityRefOrganization.Entity; }
            set { _entityRefOrganization.Entity = value; }
        }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember]
        public string Label { get; set; }

        [Column(CanBeNull = false)]
        [DataMember]
        public string Comment { get; set; }

        [Column(CanBeNull = false)]
        protected int? RateId { get; set; }
        private EntityRef<Rate> _entityRefRate = default(EntityRef<Rate>);
        [Association(ThisKey = "RateId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefRate")]

        [DataMember]
        public Rate Rate
        {
            get { return _entityRefRate.Entity; }
            set { _entityRefRate.Entity = value; }
        }
    }
}