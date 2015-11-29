using System.Linq;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {
        private readonly IStatisticsApplicationService _statisticsApplicationService;

        public HomeController(IStatisticsApplicationService statisticsApplicationService)
        {
            _statisticsApplicationService = statisticsApplicationService;
        }

        public ActionResult Index()
        {
            //TODO: We should retrive this from a page property later when we integrate with a real CMS tool!
            ViewData["Message"] = "Välkomna!";

            var topSearchterms = _statisticsApplicationService.GetTopNSearchterms(10);

            ViewData["TopSearchterms"] = topSearchterms;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
