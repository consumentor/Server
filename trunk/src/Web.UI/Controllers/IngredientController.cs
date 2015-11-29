using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.CustomAttributes;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class IngredientController : BaseController
    {
        private readonly IIngredientApplicationService _ingredientApplicationService;

        public IngredientController(IIngredientApplicationService ingredientApplicationService)
        {
            _ingredientApplicationService = ingredientApplicationService;
        }

        //
        // GET: /Ingredients/
        public ActionResult Index()
        {
            var ingredientList = _ingredientApplicationService.GetAllIngredients();

            ViewData.Model = ingredientList;
            //return View(ingredientList);
            return View();
        }

        [Authorize]
        public ActionResult CreateIngredient()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateIngredient(Ingredient ingredient, FormCollection form)
        {
            //if (ModelState.IsValid)
            //{

            //Ingredient ingredient = new Ingredient();
            // Deserialize (Include white list!)
            bool isModelUpdated = TryUpdateModel(ingredient, new[] { "IngredientName" }, form.ToValueProvider());
            
            // Validate
            if (String.IsNullOrEmpty(ingredient.IngredientName))
                ModelState.AddModelError("IngredientName", "Ingredient Name is required!");

            if (ModelState.IsValid)
            {
                var newIngredient = _ingredientApplicationService.CreateIngredient(ingredient.IngredientName);

                if (newIngredient != null)
                {
                    return RedirectToAction("EditIngredient", new{id = newIngredient.Id});
                }
                ModelState.AddModelError("IngredientName", "Ingredient or AlternativeIngredientName already exists!");
            }

            return View(ingredient);
        }

        [Authorize]
        public ActionResult EditIngredient(int id)
        {
            var ingredient = _ingredientApplicationService.GetIngredient(id);
            var ingredients = _ingredientApplicationService.GetAllIngredients();
            ingredients.Remove(ingredient);
            ViewData["Ingredients"] = new SelectList(ingredients, "Id", "IngredientName");
            ViewData["AlternativeNames"] = new SelectList(ingredient.AlternativeIngredientNames);

            return View(ingredient);
        }

        [Authorize()]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditIngredient(Ingredient ingredient, FormCollection form)
        {
            _ingredientApplicationService.UpdateIngredient(ingredient);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteIngredient(int id)
        {
            var ingredient = _ingredientApplicationService.GetIngredient(id);
            var ingredients = _ingredientApplicationService.GetAllIngredients();
            ingredients.Remove(ingredient);
            ViewData["SubstitutingIngredientId"] = new SelectList(ingredients, "Id", "IngredientName");

            return View(ingredient);
        }

        [HttpPost]
        public ActionResult DeleteIngredient(Ingredient ingredientToDelete, FormCollection form)
        {
            int substitutingIngredientId;
            int.TryParse(form["SubstitutingIngredientId"], out substitutingIngredientId);

            _ingredientApplicationService.DeleteIngredient(ingredientToDelete.Id, substitutingIngredientId);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddAlternativeIngredientName(int ingredientId, string alternativeName)
        {
            bool result = _ingredientApplicationService.AddAlternativeName(ingredientId, alternativeName);
            return Json(result);
        }

        [Authorize]
        [HttpPost]
        public JsonResult RemoveAlternativeIngredientName(int ingredientId, string[] alternativeNamesToRemove)
        {
            var result = true;
            foreach (var name in alternativeNamesToRemove)
            {
                if (!_ingredientApplicationService.RemoveAlternativeName(ingredientId, name))
                {
                    result = false;
                }
            }
            return Json(result);
        }
    }
}
