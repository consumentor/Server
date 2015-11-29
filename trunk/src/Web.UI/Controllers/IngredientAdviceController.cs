using System.Linq;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.CustomAttributes;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class IngredientAdviceController : AdviceController
    {
        private readonly IIngredientApplicationService _ingredientApplicationService;

        public IngredientAdviceController(IAdviceApplicationService adviceApplicationService
            , IIngredientApplicationService ingredientApplicationService
            , IMentorApplicationService mentorApplicationService
            , ISemaphoreApplicationService semaphoreApplicationService)
            : base(adviceApplicationService, mentorApplicationService, semaphoreApplicationService)
        {
            _ingredientApplicationService = ingredientApplicationService;
        }

        [HttpPost]
        public JsonResult IngredientAdvices(int? page, int? rows, string sidx, string sord, string searchMask)
        {
            var ingredients = _adviceApplicationService.GeIngredientsWithAdvicesByMentor(CurrentMentor);

            if (!string.IsNullOrEmpty(searchMask))
            {
                ingredients = (from ingredient in ingredients
                               where ingredient.IngredientName.ToLower().Contains(searchMask.ToLower())
                               select ingredient).ToList();
            }

            var ingredientIdsAndNames = from i in ingredients
                                        orderby i.IngredientName
                                        select new
                                                   {
                                                       i.Id,
                                                       i.IngredientName,
                                                       IngredientAdvices =
                                                            i.IngredientAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id)
                                                   };
            page = page.HasValue ? page.Value - 1 : 0;
            var pagedList = ingredientIdsAndNames.AsQueryable().ToPagedList(page.Value, rows ?? 10);

            return BuildJsonResult(pagedList);
        }

        public ActionResult Details(int id)
        {
            var ingredientAdvice = _adviceApplicationService.GetAdvice(id) as IngredientAdvice;
            var ingredient = _ingredientApplicationService.GetIngredientById(ingredientAdvice.IngredientsId.Value);
            ViewData["Ingredient"] = ingredient;
            return View(ingredientAdvice);
        }

        [Authorize]
        public ActionResult Create(int? id)
        {
            var ingredients= _ingredientApplicationService.GetAllIngredients();
            ViewData["Ingredients"] = new SelectList(ingredients, "Id", "IngredientName", id); ;
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Create(IngredientAdvice ingredientAdvice, FormCollection form)
        {
            //if (ModelState.IsValid)
            //{

            //Ingredient ingredient = new Ingredient();
            // Deserialize (Include white list!)
            //bool isModelUpdated = TryUpdateModel(ingredientAdvice,
            //                                     new[]
            //                                         {
            //                                             "IngredientsId", "Label", "Introduction", "Advice", "KeyWords",
            //                                             "Semaphore", "Published"
            //                                         }, form.ToValueProvider());

            // Validate

            if (ingredientAdvice.IngredientsId == null)
            {
                ModelState.AddModelError("IngredientsId", "Please choose an ingredient.");
            }
            ValidateAdvice(ingredientAdvice);
            if (ModelState.IsValid)
            {
                try
                {
                    _adviceApplicationService.AddIngredientAdvice(CurrentMentor, ingredientAdvice);

                    return RedirectToAction("Index", "Advice");
                }
                catch
                {
                    return RedirectToAction("Create");
                }
            }
            var ingredients = _ingredientApplicationService.GetAllIngredients();
            ViewData["Ingredients"] = new SelectList(ingredients, "Id", "IngredientName", ingredientAdvice.IngredientsId) ;
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            return View(ingredientAdvice);
        }

        //
        // GET: /IngredientAdvice/Edit/5
        [Authorize]
        [CheckPermission("id")]
        public ActionResult Edit(int id)
        {
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(id) as IngredientAdvice;
            var ingredient = _ingredientApplicationService.GetIngredientById(advice.IngredientsId.Value);
            ViewData["Ingredient"] = ingredient;
            SetAdviceTagViewData();
            return View(advice);
        }

        //
        // POST: /IngredientAdvice/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(IngredientAdvice ingredientAdvice, FormCollection form)
        {
            ValidateAdvice(ingredientAdvice);
            if (ModelState.IsValid)
            {
                _adviceApplicationService.UpdateAdvice(ingredientAdvice);
                return RedirectToAction("Index", "Advice");
            }
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(ingredientAdvice.Id.Value) as IngredientAdvice;
            var ingredient = _ingredientApplicationService.GetIngredientById(advice.IngredientsId.Value);
            ViewData["Ingredient"] = ingredient;
            return View(advice);
        }

        public JsonResult GetAdvicesForIngredient(int? id)
        {
            var ingredients = _adviceApplicationService.GeIngredientsWithAdvicesByMentor(CurrentMentor);
            var ingredient = ingredients.Single(x => x.Id == id.Value);
            var advices = ingredient.IngredientAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id);
            var pagedList = advices.AsQueryable().ToPagedList(0, advices.Count());
            var jsonData = new
            {
                total = pagedList.PageSize - 1,
                page = pagedList.PageIndex,
                records = pagedList.TotalCount,
                rows = pagedList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Publish(int id)
        {
            _adviceApplicationService.PublishAdvice(id);
            return Json(null);
        }

        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Unpublish(int id)
        {
            _adviceApplicationService.UnpublishAdvice(id);
            return Json(null);
        }


        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            _adviceApplicationService.DeleteAdvice(id);
            return Json(null);
        }
    }
}
