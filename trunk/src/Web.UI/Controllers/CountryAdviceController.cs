using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.CustomAttributes;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class CountryAdviceController : AdviceController
    {
        private readonly ICountryApplicationService _countryApplicationService;

        public CountryAdviceController(IAdviceApplicationService adviceApplicationService
            , ICountryApplicationService countryApplicationService
            , IMentorApplicationService mentorApplicationService
            , ISemaphoreApplicationService semaphoreApplicationService)
            : base(adviceApplicationService, mentorApplicationService, semaphoreApplicationService)
        {
            _countryApplicationService = countryApplicationService;
        }

        [HttpPost]
        public JsonResult CountryAdvices(int? page, int? rows, string sidx, string sord, string searchMask)
        {
            var countries = _adviceApplicationService.GetCountriesWithAdvicesByMentor(CurrentMentor);
            if (!string.IsNullOrEmpty(searchMask))
            {
                countries = (from country in countries
                               where country.CountryCode.Name.ToLower().Contains(searchMask.ToLower())
                               select country).ToList();
            }
            var countryIdsAndNames = from country in countries
                                     orderby country.CountryCode.Name
                                   select new {country.Id, CountryName = country.CountryCode.Name};
            page = page.HasValue ? page.Value - 1 : 0;
            var pagedList = countryIdsAndNames.AsQueryable().ToPagedList(page.Value, rows ?? 10);

            return BuildJsonResult(pagedList);
        }

        [Authorize]
        public ActionResult Create(int? id)
        {
            var countries = _countryApplicationService.GetAllCountries();
            ViewData["Countries"] = new SelectList(countries, "Id", "CountryCode.Name", id);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Create(CountryAdvice countryAdvice, FormCollection form)
        {
            if (countryAdvice.CountrysId == null)
            {
                ModelState.AddModelError("CountrysId", "Please choose a country...");
            }
            ValidateAdvice(countryAdvice);

            if (ModelState.IsValid)
            {
                try
                {
                    _adviceApplicationService.AddCountryAdvice(CurrentMentor, countryAdvice);

                    return RedirectToAction("Index", "Advice");
                }
                catch
                {
                    return RedirectToAction("Create");
                }
            }
            var countries = _countryApplicationService.GetAllCountries();
            ViewData["Countries"] = new SelectList(countries, "Id", "CountryCode.Name", countryAdvice.CountrysId);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View(countryAdvice);
        }

        //
        // GET: /CountryAdvice/Edit/5
        [Authorize]
        [CheckPermission("id")]
        public ActionResult Edit(int id)
        {
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(id) as CountryAdvice;
            var country = _countryApplicationService.GetCountry(advice.CountrysId.Value);
            ViewData["Country"] = country;
            return View(advice);

        }

        //
        // POST: /CountryAdvice/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(CountryAdvice countryAdvice, FormCollection form)
        {
            ValidateAdvice(countryAdvice);
            if (ModelState.IsValid)
            {
                _adviceApplicationService.UpdateAdvice(countryAdvice);
                return RedirectToAction("Index", "Advice");
            }
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(countryAdvice.Id.Value) as CountryAdvice;
            var country = _countryApplicationService.GetCountry(advice.CountrysId.Value);
            ViewData["Country"] = country;
            return View(advice);
        }

        public JsonResult GetAdvicesForCountry(int? id)
        {
            var countries = _adviceApplicationService.GetCountriesWithAdvicesByMentor(CurrentMentor);
            var country = countries.Single(x => x.Id == id.Value);
            var advices = country.CountryAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id);

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
