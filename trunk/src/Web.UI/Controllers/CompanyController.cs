using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly ICompanyApplicationService _companyApplicationService;
        private readonly ICountryApplicationService _countryApplicationService;

        public CompanyController(ICompanyApplicationService companyApplicationService, ICountryApplicationService countryApplicationService)
        {
            _companyApplicationService = companyApplicationService;
            _countryApplicationService = countryApplicationService;
        }

        public ActionResult Index()
        {
            var companies = _companyApplicationService.GetAllCompanies();
            return View(companies);
        }

        //
        // GET: /Company/Create

        public ActionResult CreateCompany()
        {
            ViewData["Companies"] = _companyApplicationService.GetAllCompanies();
            ViewData["Countries"] = _countryApplicationService.GetAllCountries();
            return View();
        }

        //
        // POST: /Company/Create

        [HttpPost]
        public ActionResult CreateCompany(Company companyToCreate, FormCollection collection)
        {
            try
            {
                //TODO: verify all fields filled
                _companyApplicationService.CreateCompany(companyToCreate);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Company/Edit/5

        public ActionResult EditCompany(int id)
        {
            ViewData["Companies"] = _companyApplicationService.GetAllCompanies();
            ViewData["Countries"] = _countryApplicationService.GetAllCountries();
            var company = _companyApplicationService.GetCompany(id);
            return View(company);
        }

        //
        // POST: /Company/Edit/5

        [HttpPost]
        public ActionResult EditCompany(Company updatedCompany, FormCollection collection)
        {
            try
            {
                // TODO: verify fields
                _companyApplicationService.UpdateCompany(updatedCompany);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteCompany(int id)
        {
            var company = _companyApplicationService.GetCompany(id);
            var childCompanies = _companyApplicationService.GetChildCompanies(company);
            ViewData["ChildCompanies"] = childCompanies;
            var companies = _companyApplicationService.GetAllCompanies();
            companies.Remove(company);
            ViewData["SubstitutingCompany"] = new SelectList(companies, "Id", "CompanyName");

            return View(company);
        }

        [HttpPost]
        public ActionResult DeleteCompany(Company companyToDelete, FormCollection form)
        {
            int substituteCompanyId;
            var substituteSelected = int.TryParse(form["SubstitutingCompany"], out substituteCompanyId);

            _companyApplicationService.DeleteCompany(companyToDelete.Id, substituteSelected ? substituteCompanyId : (int?) null);
            return RedirectToAction("Index");
        }

    }
}
