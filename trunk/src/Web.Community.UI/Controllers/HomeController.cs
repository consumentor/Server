using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server;

namespace Consumentor.Shopgun.Web.Community.UI.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private IAdviceSearchApplicationService _adviceSearchApplicationService;

        public HomeController(IAdviceSearchApplicationService adviceSearchApplicationService)
        {
            _adviceSearchApplicationService = adviceSearchApplicationService;
        }

        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AutoCompleteSearch(string query, int maxPerCategory)
        {
            var result = _adviceSearchApplicationService.SearchListInfo("", query, maxPerCategory, null);

            return Json(result);
        }
    }
}
