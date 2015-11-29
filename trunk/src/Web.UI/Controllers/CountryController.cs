using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class CountryController : BaseController
    {
        private readonly ICountryApplicationService _countryApplicationService;
        //private readonly IBrandApplicationService _brandApplicationService;
        //private readonly ICompanyApplicationService _companyApplicationService;
        //private readonly IIngredientApplicationService _ingredientApplicationService;

        public CountryController(ICountryApplicationService countryApplicationService)
        {
            _countryApplicationService = countryApplicationService;
            //_ingredientApplicationService = ingredientApplicationService;
        }

        //
        // GET: /Country/

        public ActionResult Index()
        {
            var countrys = _countryApplicationService.GetAllCountries();
            return View(countrys);
        }

        public ActionResult Create()
        {
            ViewData["Ingredients"] = _countryApplicationService.GetAllCountries();

            return View();
        }

        [HttpPost]
        public ActionResult Create(Country newCountry)
        {
            return View();
        }

        //public ActionResult Edit(int id)
        //{
        //    var country = _countryApplicationService.GetCountryById(id);
        //    return View(country);
        //}

        //[HttpPost]
        //public ActionResult Edit(Country newCountry)
        //{
        //    return View();
        //}
    }
}
