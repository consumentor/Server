using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Product_CertificationMark")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class ProductCertificationMark
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false, DbType = "int NOT NULL")]
        public int ProductId { get; set; }
        private EntityRef<Product> _entityRefProduct = default(EntityRef<Product>);
        [Association(ThisKey = "ProductId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefProduct")]
        internal Product Product
        {
            get { return _entityRefProduct.Entity; }
            set { _entityRefProduct.Entity = value; }
        }

        [Column(CanBeNull = false, DbType = "int NOT NULL")]
        public int CertificationMarkId { get; set; }
        private EntityRef<CertificationMark> _entityRefCertificationMark = default(EntityRef<CertificationMark>);
        [Association(ThisKey = "CertificationMarkId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCertificationMark")]
        internal CertificationMark CertificationMark
        {
            get { return _entityRefCertificationMark.Entity; }
            set { _entityRefCertificationMark.Entity = value; }
        }
    }
}