using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "AllergyInformation")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class AllergyInformation
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(4000)")]
        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [Column(CanBeNull = true)]
        public int? ProductId { get; set; }
        private EntityRef<Product> _entityRefProduct = default(EntityRef<Product>);
        [Association(ThisKey = "ProductId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefProduct")]
        internal Product Product
        {
            get { return _entityRefProduct.Entity; }
            set { _entityRefProduct.Entity = value; }
        }

        [Column]
        internal int IngredientId { get; set; }
        private EntityRef<Ingredient> _entityRefIngredient = default(EntityRef<Ingredient>);
        [Association(ThisKey = "IngredientId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefIngredient")]
        [DataMember(Name = "allergene")]
        public Ingredient Allergene
        {
            get { return _entityRefIngredient.Entity; } 
            set { _entityRefIngredient.Entity = value; }
        }
    }
}
