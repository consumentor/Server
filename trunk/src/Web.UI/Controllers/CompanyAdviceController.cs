using System.Linq;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.CustomAttributes;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class CompanyAdviceController : AdviceController
    {
        private readonly ICompanyApplicationService _companyApplicationService;

        public CompanyAdviceController(IAdviceApplicationService adviceApplicationService
            , IMentorApplicationService mentorApplicationService
            , ISemaphoreApplicationService semaphoreApplicationService
            , ICompanyApplicationService companyApplicationService) 
            : base(adviceApplicationService, mentorApplicationService, semaphoreApplicationService)
        {
            _companyApplicationService = companyApplicationService;
        }

        [HttpPost]
        public JsonResult CompanyAdvices(int? page, int? rows, string sidx, string sord, string searchMask)
        {
            var companies = _adviceApplicationService.GetCompaniesWithAdvicesByMentor(CurrentMentor);
            if (!string.IsNullOrEmpty(searchMask))
            {
                companies = (from company in companies
                               where company.CompanyName.ToLower().Contains(searchMask.ToLower())
                               select company).ToList();
            }
            var companyIdsAndNames = from company in companies
                                     orderby company.CompanyName
                                     select new
                                             {
                                                 company.Id,
                                                 company.CompanyName,
                                                 CompanyAdvices =
                                                    company.CompanyAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id)
                                             };

            page = page.HasValue ? page.Value - 1 : 0;
            var pagedList = companyIdsAndNames.AsQueryable().ToPagedList(page.Value, rows ?? 10);

            return BuildJsonResult(pagedList);
        }

        [Authorize]
        public ActionResult Create(int? id)
        {
            var companies = _companyApplicationService.GetAllCompanies();
            ViewData["Companies"] = new SelectList(companies, "Id", "CompanyName", id);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Create(CompanyAdvice companyAdvice, FormCollection form)
        {
            if (companyAdvice.CompanysId == null)
            {
                ModelState.AddModelError("CompanysId", "Please choose a company.");
            }
            ValidateAdvice(companyAdvice);

            if (ModelState.IsValid)
            {
                try
                {
                    _adviceApplicationService.AddCompanyAdvice(CurrentMentor, companyAdvice);

                    return RedirectToAction("Index", "Advice");
                }
                catch
                {
                    return RedirectToAction("Create");
                }
            }

            var companies = _companyApplicationService.GetAllCompanies();
            ViewData["Companies"] = new SelectList(companies, "Id", "CompanyName", companyAdvice.CompanysId);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View(companyAdvice);
        }

        //
        // GET: /ConceptAdvice/Edit/5
        [Authorize]
        [CheckPermission("id")]
        public ActionResult Edit(int id)
        {
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(id) as CompanyAdvice;
            var company = _companyApplicationService.GetCompany(advice.CompanysId.Value);
            ViewData["Company"] = company;
            return View(advice);
        }

        //
        // POST: /ConceptAdvice/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(CompanyAdvice companyAdvice, FormCollection form)
        {
            ValidateAdvice(companyAdvice);
            if (ModelState.IsValid)
            {
                _adviceApplicationService.UpdateAdvice(companyAdvice);
                return RedirectToAction("Index", "Advice");   
            }
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(companyAdvice.Id.Value) as CompanyAdvice;
            var company = _companyApplicationService.GetCompany(advice.CompanysId.Value);
            ViewData["Company"] = company;
            return View(advice);
        }

        public JsonResult GetAdvicesForCompany(int? id)
        {
            var companies = _adviceApplicationService.GetCompaniesWithAdvicesByMentor(CurrentMentor);
            var company = companies.Single(x => x.Id == id.Value);
            var advices = company.CompanyAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id);

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
