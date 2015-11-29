using System;
using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class IngredientBuilder
    {
        public static Ingredient BuildIngredient()
        {
            return BuildIngredient("New ingredient");
        }

        public static Ingredient BuildIngredient(string ingredientName)
        {
            return new Ingredient { IngredientName = ingredientName, LastUpdated = DateTime.Now };
        }
    }
}
