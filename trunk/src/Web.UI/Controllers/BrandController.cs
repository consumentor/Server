using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class BrandController : BaseController
    {
        private readonly IBrandApplicationService _brandApplicationService;
        private readonly ICompanyApplicationService _companyApplicationService;

        public BrandController(IBrandApplicationService brandApplicationService, ICompanyApplicationService companyApplicationService)
        {
            _brandApplicationService = brandApplicationService;
            _companyApplicationService = companyApplicationService;
        }

        public ActionResult BrandIndex()
        {
            var brands = _brandApplicationService.GetAllBrands();
            return View(brands);
        }

        //
        // GET: /Brand/Create

        public ActionResult CreateBrand()
        {
            var companies = _companyApplicationService.GetAllCompanies();
            ViewData["OwnerId"] = new SelectList(companies, "Id", "CompanyName");
            return View();
        } 

        //
        // POST: /Brand/Create

        [HttpPost]
        public ActionResult CreateBrand(Brand brandToCreate, FormCollection collection)
        {
            try
            {
                _brandApplicationService.CreateBrand(brandToCreate);
                return RedirectToAction("BrandIndex");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Brand/Edit/5
 
        public ActionResult EditBrand(int id)
        {
            var companies = _companyApplicationService.GetAllCompanies();
            ViewData["OwnerId"] = new SelectList(companies, "Id", "CompanyName");
            
            var brand = _brandApplicationService.GetBrand(id);
            return View(brand);
        }

        //
        // POST: /Brand/Edit/5

        [HttpPost]
        public ActionResult EditBrand(Brand updatedBrand, FormCollection collection)
        {
            try
            {
                _brandApplicationService.UpdateBrand(updatedBrand);
                return RedirectToAction("BrandIndex");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteBrand(int id)
        {
            var brand = _brandApplicationService.GetBrand(id);
            var brands = _brandApplicationService.GetAllBrands();
            brands.Remove(brand);
            ViewData["SubstitutingBrandId"] = new SelectList(brands, "Id", "BrandName");

            return View(brand);
        }

        [HttpPost]
        public ActionResult DeleteBrand(Brand brandToDelete, FormCollection form)
        {
            int substitutingBrandId;
            var substituteSelected = int.TryParse(form["SubstitutingBrandId"], out substitutingBrandId);

            _brandApplicationService.DeleteBrand(brandToDelete.Id, substituteSelected ? substitutingBrandId : (int?) null);
            return RedirectToAction("BrandIndex");
        }
    }
}
