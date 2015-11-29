using System.Linq;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.CustomAttributes;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class BrandAdviceController : AdviceController
    {
        private readonly IBrandApplicationService _brandApplicationService;

        public BrandAdviceController(IAdviceApplicationService adviceApplicationService
            , IBrandApplicationService brandApplicationService
            , IMentorApplicationService mentorApplicationService
            , ISemaphoreApplicationService semaphoreApplicationService)
            : base(adviceApplicationService, mentorApplicationService, semaphoreApplicationService)
        {
            _brandApplicationService = brandApplicationService;
        }

        [HttpPost]
        public JsonResult BrandAdvices(int? page, int? rows, string sidx, string sord, string searchMask)
        {
            var brands = _adviceApplicationService.GetBrandsWithAdvicesByMentor(CurrentMentor);
            if (!string.IsNullOrEmpty(searchMask))
            {
                brands = (from product in brands
                               where product.BrandName.ToLower().Contains(searchMask.ToLower())
                               select product).ToList();
            }
            var brandIdsAndNames = from brand in brands
                                   orderby brand.BrandName
                                   select new
                                           {
                                               brand.Id,
                                               brand.BrandName,
                                               BrandAdvices =
                                                brand.BrandAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id)
                                           };
            page = page.HasValue ? page.Value - 1 : 0;
            var pagedList = brandIdsAndNames.AsQueryable().ToPagedList(page.Value, rows ?? 10);

            return BuildJsonResult(pagedList);
        }

        [Authorize]
        public ActionResult Create(int? id)
        {
            var brands = _brandApplicationService.GetAllBrands();
            ViewData["Brands"] = new SelectList(brands, "Id", "BrandName", id);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Create(BrandAdvice brandAdvice, FormCollection form)
        {
            if (brandAdvice.BrandsId == null)
            {
                ModelState.AddModelError("BrandsId", "Please choose a brand...");
            }
            ValidateAdvice(brandAdvice);

            if (ModelState.IsValid)
            {
                try
                {
                    var mentor = CurrentMentor;

                    _adviceApplicationService.AddBrandAdvice(mentor, brandAdvice);

                    return RedirectToAction("Index", "Advice");
                }
                catch
                {
                    return RedirectToAction("Create");
                }
            }
            var brands = _brandApplicationService.GetAllBrands();
            ViewData["Brands"] = new SelectList(brands, "Id", "BrandName", brandAdvice.BrandsId);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View(brandAdvice);
        }

        //
        // GET: /BrandAdvice/Edit/5
        [Authorize]
        [CheckPermission("id")]
        public ActionResult Edit(int id)
        {
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(id) as BrandAdvice;
            var brand = _brandApplicationService.GetBrand(advice.BrandsId.Value);
            ViewData["Brand"] = brand;
            return View(advice);

        }

        //
        // POST: /BrandAdvice/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(BrandAdvice brandAdvice, FormCollection form)
        {
            ValidateAdvice(brandAdvice);
            if (ModelState.IsValid)
            {
                _adviceApplicationService.UpdateAdvice(brandAdvice);
                return RedirectToAction("Index", "Advice");
            }
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(brandAdvice.Id.Value) as BrandAdvice;
            var brand = _brandApplicationService.GetBrand(advice.BrandsId.Value);
            ViewData["Brand"] = brand;
            return View(advice);
        }

        public JsonResult GetAdvicesForBrand(int? id)
        {
            var brands = _adviceApplicationService.GetBrandsWithAdvicesByMentor(CurrentMentor);
            var brand = brands.Single(x => x.Id == id.Value);
            var advices = brand.BrandAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id);

            var jsonData = new
            {
                rows = advices.AsQueryable().ToPagedList(0, advices.Count())
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
