using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IIngredientApplicationService
    {
        Ingredient GetIngredientById(int ingredientId);
        Ingredient CreateIngredient(string ingredientName);
        Ingredient GetIngredient(int ingredientId);
        Ingredient GetIngredient(int ingredientId, bool onlyPublishedAdvices);
        IList<Ingredient> GetAllIngredients();
        Ingredient UpdateIngredient(Ingredient updatedIngredient);
        bool AddAlternativeName(int ingredientId, string alternativeName);
        bool RemoveAlternativeName(int ingredientId, string alternativeName);
        bool DeleteIngredient(int ingredientId);
        bool DeleteIngredient(int ingredientId, int substitutingIngredientId);
        Ingredient GetIngredientByName(string ingredientName);
        Ingredient FindIngredient(string ingredientName, bool withParenAdvices, bool onlyPublishedAdvices);
        IList<Ingredient> FindIngredients(string ingredientName, bool withParentAdvices, bool onlyPublishedAdvices);
        IList<Ingredient> GetIngredientsWithAdvicesByMentor(Mentor mentor);
        Ingredient AddIngredient(string ingredientName);
        IList<Ingredient> FindIngredientsForTableOfContents(string tableOfContents);
    }
}
