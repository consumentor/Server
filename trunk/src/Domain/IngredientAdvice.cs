using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class IngredientAdvice : AdviceBase
    {
        [Column(CanBeNull = true)]
        public int? IngredientsId { get; set; }
        private EntityRef<Ingredient> _entityRefIngredient = default(EntityRef<Ingredient>);

        [Association(ThisKey = "IngredientsId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefIngredient")]
        internal Ingredient Ingredient
        {
            get { return _entityRefIngredient.Entity; }
            set { _entityRefIngredient.Entity = value; }
        }

        public override string ToString()
        {
            return "Ingredient Advice";
        }

        public override string ItemName
        {
            get { return Ingredient.IngredientName; }
            internal set { throw new NotImplementedException(); }
        }

        protected override object CreateInstanceOfSameTypeAsThis()
        {
            return new IngredientAdvice();
        }

        protected override void SetClonedData(AdviceBase clone)
        {
            ((IngredientAdvice)clone).IngredientsId = IngredientsId;
            base.SetClonedData(clone);
        }
    }
}
