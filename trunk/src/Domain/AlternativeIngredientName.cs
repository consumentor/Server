using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "AlternativeIngredientNames")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class AlternativeIngredientName
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember(Name = "alternativeName")]
        public string AlternativeName { get; set; }

        [Column(CanBeNull = true)]
        protected int? IngredientId { get; set; }
        private EntityRef<Ingredient> _entityRefIngredient = default(EntityRef<Ingredient>);

        [Association(ThisKey = "IngredientId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefIngredient")]
        public Ingredient Ingredient
        {
            get { return _entityRefIngredient.Entity; }
            set { _entityRefIngredient.Entity = value; }
        }
    }
}
