using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Context;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class PerformanceTestController : Controller
    {
        private readonly IAdviceSearchApplicationService _adviceSearchApplicationService;
        private readonly IProductApplicationService _productApplicationService;

        public PerformanceTestController(IAdviceSearchApplicationService adviceSearchApplicationService, IProductApplicationService productApplicationService)
        {
            _adviceSearchApplicationService = adviceSearchApplicationService;
            _productApplicationService = productApplicationService;
        }


        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(HttpPostedFileBase uploadFile)
        {
            if (uploadFile.ContentLength > 0)
            {
                var streamReader = new StreamReader(uploadFile.InputStream);

                var gtinList = new List<string>();
                while (!streamReader.EndOfStream)
                {
                    gtinList.Add(streamReader.ReadLine());
                }

                var productsWithInfo = new List<string>();
                var productsWithAdvices = new List<string>();
                foreach (var gtin in gtinList)
                {
                    var result = _productApplicationService.FindProductByGtin(gtin, true);
                    if (!string.IsNullOrEmpty(result.ProductName))
                    {
                        productsWithInfo.Add(result.ProductName);
                    }
                    if (result.ProductAdvices.Count > 0
                        || result.Brand.BrandAdvices.Count > 0
                        || (result.Brand.Owner != null && result.Brand.Owner.CompanyAdvices.Count > 0)
                        || result.Ingredients.Any(x => x.IngredientAdvices.Count > 0))
                    {
                        productsWithAdvices.Add(result.ProductName);
                    }
                }
                ViewData["ProdWithInfo"] = productsWithInfo;
                ViewData["ProdWithAdvices"] = productsWithAdvices;
            }
            return View();
        }

    }
}
