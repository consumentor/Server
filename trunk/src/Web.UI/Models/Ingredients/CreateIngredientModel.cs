using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Consumentor.Shopgun.Web.UI.Models.Ingredients
{
    public class CreateIngredientModel
    {
        [Required]
        [DisplayName("Ingredient name")]
        public string IngredientName { get; set; }
    }
}
