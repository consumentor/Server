using System;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.CustomAttributes;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    [HandleError]
    public class AdviceController : BaseController
    {

        protected readonly IAdviceApplicationService _adviceApplicationService;
        //TODO: We dont use the _semaphoreApplicationService, why do we have here???
        protected readonly ISemaphoreApplicationService _semaphoreApplicationService;
        private readonly IMentorApplicationService _mentorApplicationService;

        public AdviceController(IAdviceApplicationService adviceApplicationService
            , IMentorApplicationService mentorApplicationService
            , ISemaphoreApplicationService semaphoreApplicationService)
        {
            _adviceApplicationService = adviceApplicationService;
            _semaphoreApplicationService = semaphoreApplicationService;
            _mentorApplicationService = mentorApplicationService;
        }

        // GET: /Advices/
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("SuperUser"))
               ViewData["mentors"] = new SelectList(_mentorApplicationService.GetAllMentors(), "Id", "MentorName", CurrentMentor.Id);
            return View();
        }

        public ActionResult AdviceDetails(int id)
        {
            var advice = _adviceApplicationService.GetAdvice(id);
            if (advice == null)
            {
                ViewData["Message"] = "Advice not found.";
            }
            return PartialView(advice);
        }

        public ActionResult IngredientAdvices()
        {
            return AdvicesTable("Ingredient");
        }

        public ActionResult ConceptAdvices()
        {
            return AdvicesTable("Concept", "Term");
        }

        public ActionResult CompanyAdvices()
        {
            return AdvicesTable("Company");
        }

        public ActionResult BrandAdvices()
        {
            return AdvicesTable("Brand");
        }

        public ActionResult ProductAdvices()
        {
            return AdvicesTable("Product");
        }

        public ActionResult CountryAdvices()
        {
            return AdvicesTable("Country");
        }

        public ActionResult AdvicesTable(string ModelName)
        {
            return AdvicesTable(ModelName, "Name");
        }

        public ActionResult AdvicesTable(string ModelName, string ModelTitleSuffix)
        {
            ViewData["AdvicesModelName"] = ModelName;
            ViewData["AdvicesModelTitleSuffix"] = ModelTitleSuffix;
            return PartialView("AdvicesTable");
        }

        #region Mentor
        [HttpPost]
        [Authorize(Roles = "SuperUser")]
        public JsonResult SetMentor(int mentorId)
        {
            Session["MentorId"] = mentorId;
            return Json(null);
        }

        #endregion

        protected void SetAdviceTagViewData()
        {
            var adviceTags = _adviceApplicationService.GetAllAdviceTags();
            ViewData["AdviceTags"] = new SelectList(adviceTags, "Id", "Name");
        }

        protected void ValidateAdvice(AdviceBase advice)
        {
            if (String.IsNullOrEmpty(advice.Label))
            {
                ModelState.AddModelError("Label", "Label is required!");
            }
            if (String.IsNullOrEmpty(advice.Introduction))
            {
                ModelState.AddModelError("Introduction", "Introduction is required!");
            }
            if (String.IsNullOrEmpty(advice.Advice))
            {
                ModelState.AddModelError("Advice", "Advice text is required!");
            }
            if (String.IsNullOrEmpty(advice.KeyWords))
            {
                advice.KeyWords = "";
            }
            if (advice.SemaphoreId == null)
            {
                ModelState.AddModelError("Semaphore", "Please choose a signal for your advice");
            }
        }

        protected JsonResult BuildJsonResult<TDomain>(PagedList<TDomain> pagedList)
        {
            double totalCount = pagedList.TotalCount;
            double pageSize = pagedList.PageSize;
            var jsonData = new
            {
                total = Math.Round( totalCount/pageSize , MidpointRounding.AwayFromZero),
                page = pagedList.PageIndex+1,
                records = pagedList.TotalCount,
                rows = pagedList
            };
            return Json(jsonData);
        }
    }
}
