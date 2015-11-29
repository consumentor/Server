using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace Consumentor.ShopGun.Domain
{
    public class IngredientStatistic : StatisticsBase
    {
        [Column(CanBeNull = true)]
        protected int? IngredientId { get; set; }

        private EntityRef<Ingredient> _entityRefIngredient = default(EntityRef<Ingredient>);

        [Association(ThisKey = "IngredientId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefIngredient")]
        internal Ingredient Ingredient
        {
            get { return _entityRefIngredient.Entity; }
            set { _entityRefIngredient.Entity = value; }
        }
    }
}
