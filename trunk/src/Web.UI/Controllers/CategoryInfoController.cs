using System.Web.Mvc;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class CategoryInfoController : BaseController
    {
        private readonly ICategoryInfoDomainService _categoryInfoDomainService;

        public CategoryInfoController(ICategoryInfoDomainService categoryInfoDomainService)
        {
            _categoryInfoDomainService = categoryInfoDomainService;
        }

        //
        // GET: /CategoryInfo/

        public ActionResult Index()
        {
            var categoryInfos = _categoryInfoDomainService.GetAllCategoryInfos();
            return View(categoryInfos);
        }

        //
        // GET: /CategoryInfo/Create

        [Authorize]
        public ActionResult CreateCategoryInfo()
        {
            return View();
        } 

        //
        // POST: /CategoryInfo/Create

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult CreateCategoryInfo(CategoryInfo newCategoryInfo)
        {
            try
            {
                _categoryInfoDomainService.CreateCategoryInfo(newCategoryInfo);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CategoryInfo/Edit/5

        [Authorize]
        public ActionResult EditCategoryInfo(int id)
        {
            var categoryInfo = _categoryInfoDomainService.GetCategoryInfo(id);
            return View(categoryInfo);
        }

        //
        // POST: /CategoryInfo/Edit/5

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult EditCategoryInfo(CategoryInfo updatedCategoryInfo)
        {
            try
            {
                _categoryInfoDomainService.UpdateCategoryInfo(updatedCategoryInfo);
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CategoryInfo/Edit/5
        [Authorize]
        public ActionResult DeleteCategoryInfo(int id)
        {
            var categoryInfo = _categoryInfoDomainService.GetCategoryInfo(id);
            return View(categoryInfo);
        }

        //
        // POST: /CategoryInfo/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult DeleteCategoryInfo(CategoryInfo categoryInfoToDelete)
        {
            try
            {
                _categoryInfoDomainService.DeleteCategoryInfo(categoryInfoToDelete.Id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
