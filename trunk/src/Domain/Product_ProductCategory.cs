using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Product_ProductCategory")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Product_ProductCategory
    {
        [Column(CanBeNull = false, DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int ProductId { get; set; }
        private EntityRef<Product> _entityRefProduct = default(EntityRef<Product>);
        [Association(ThisKey = "ProductId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefProduct", DeleteOnNull = true)]
        internal Product Product
        {
            get { return _entityRefProduct.Entity; }
            set { _entityRefProduct.Entity = value; }
        }

        [Column(CanBeNull = false, DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int ProductCategoryId { get; set; }
        private EntityRef<ProductCategory> _entityRefProductCategory = default(EntityRef<ProductCategory>);
        [Association(ThisKey = "ProductCategoryId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefProductCategory")]
        internal ProductCategory ProductCategory
        {
            get { return _entityRefProductCategory.Entity; }
            set { _entityRefProductCategory.Entity = value; }
        }
    }
}
