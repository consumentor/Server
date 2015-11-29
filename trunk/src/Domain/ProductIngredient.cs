using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Product_Ingredient")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class ProductIngredient
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
        public int IngredientId { get; set; }
        private EntityRef<Ingredient> _entityRefIngredient = default(EntityRef<Ingredient>);
        [Association(ThisKey = "IngredientId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefIngredient")]
        internal Ingredient Ingredient
        {
            get { return _entityRefIngredient.Entity; }
            set { _entityRefIngredient.Entity = value; }
        }

    }
}
