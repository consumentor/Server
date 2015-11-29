using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Consumentor.ShopGun.ApplicationService.CustomAttributes;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class ProductAdviceController : AdviceController
    {

        private readonly IProductApplicationService _productApplicationService;
        private readonly IProductAdviceApplicationService _productAdviceApplicationService;
        private readonly ICompanyApplicationService _companyApplicationService;
        private readonly IBrandApplicationService _brandApplicationService;

        public ProductAdviceController(IAdviceApplicationService adviceApplicationService
            , IProductApplicationService productApplicationService
            , IMentorApplicationService mentorApplicationService
            , ISemaphoreApplicationService semaphoreApplicationService, ICompanyApplicationService companyApplicationService, IBrandApplicationService brandApplicationService, IProductAdviceApplicationService productAdviceApplicationService)
            : base(adviceApplicationService, mentorApplicationService, semaphoreApplicationService)
        {
            _productApplicationService = productApplicationService;
            _brandApplicationService = brandApplicationService;
            _productAdviceApplicationService = productAdviceApplicationService;
            _companyApplicationService = companyApplicationService;
        }

        [HttpPost]
        public JsonResult ProductAdvices(int? page, int? rows, string sidx, string sord, string searchMask)
        {
            var products = _productAdviceApplicationService.GetProductsWithAdvicesByMentor(CurrentMentor.Id);//_productApplicationService.GetProductsWithAdvicesByMentor(CurrentMentor);//_adviceApplicationService.GetProductsWithAdvicesByMentor(CurrentMentor);

            if (!string.IsNullOrEmpty(searchMask))
            {
                int temp;
                if (int.TryParse(searchMask, out temp))
                {
                    products = (from product in products
                                where product.GlobalTradeItemNumber.StartsWith(searchMask)
                                select product).ToList();
                }
                else
                {
                    products = (from product in products
                                where product.ProductName.ToLower().Contains(searchMask.ToLower())
                                select product).ToList();
                }
            }

            var productIdsAndNames = from product in products
                                     orderby product.ProductName
                                     select new
                                             {
                                                 product.Id,
                                                 ProductName = product.ProductName + (product.Brand != null ? (" (" + product.Brand.BrandName + ")") : ""),// ?? product.GlobalTradeItemNumber,
                                                 Gtin = product.GlobalTradeItemNumber,
                                                 ProductAdvices =
                                                    product.ProductAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id)
                                             };
            //put the data in the JSON format
            page = page.HasValue ? page.Value - 1 : 0;
            var pagedList = productIdsAndNames.AsQueryable().ToPagedList(page.Value, rows ?? 20);

            return BuildJsonResult(pagedList);
        }

        [Authorize]
        public ActionResult Create(int? id)
        {
            //var products = _productApplicationService.GetAllProducts();
            //ViewData["Products"] = new SelectList(products, "Id", "ProductName", id);
            var companies = _companyApplicationService.GetAllCompanies();
            ViewData["Companies"] = new SelectList(companies, "Id", "CompanyName");
            var brands = _brandApplicationService.GetAllBrands();
            ViewData["Brands"] = new SelectList(brands, "Id", "BrandName");
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Create(ProductAdvice productAdvice, FormCollection form)
        {
            if (productAdvice.ProductsId == null)
            {
                ModelState.AddModelError("ProductsId", "Please choose a product.");
            }
            ValidateAdvice(productAdvice);
            productAdvice.MentorId = CurrentMentor.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    //_adviceApplicationService.AddProductAdvice(CurrentMentor, productAdvice);
                    _productAdviceApplicationService.SaveProductAdvice(productAdvice);
                    return RedirectToAction("Index", "Advice");
                }
                catch
                {
                    return RedirectToAction("Create");
                }
            }
            var products = _productApplicationService.GetAllProducts();
            ViewData["Products"] = new SelectList(products, "Id", "ProductName", productAdvice.ProductsId);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View(productAdvice);
        }

        //
        // GET: /ProductAdvice/Edit/5
        [Authorize]
        [CheckPermission("id")]
        public ActionResult Edit(int id)
        {
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var product = _productAdviceApplicationService.GetProductForAdvice(id);
            ViewData["Product"] = product;
            return View(product.ProductAdvices.FirstOrDefault(a => a.Id == id));
        }

        //
        // POST: /ProductAdvice/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductAdvice productAdvice, FormCollection form)
        {
            ValidateAdvice(productAdvice);
            if (ModelState.IsValid)
            {
                productAdvice.MentorId = CurrentMentor.Id;
                _productAdviceApplicationService.SaveProductAdvice(productAdvice);
                //_adviceApplicationService.UpdateAdvice(productAdvice);
                return RedirectToAction("Index", "Advice");
            }
            return Edit(productAdvice.Id.Value);
            //SetAdviceTagViewData();
            //ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            //var advice = _adviceApplicationService.GetAdvice(productAdvice.Id.Value) as ProductAdvice;
            //var product = _productApplicationService.GetProduct(advice.ProductsId.Value);
            //ViewData["Product"] = product;
            //return View(advice);
        }

        public JsonResult GetAdvicesForProduct(int? id)
        {
            var product = _productAdviceApplicationService.GetProduct(id.Value);
            var advices = product.ProductAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id);

            var jsonData = new
            {
                rows = advices.AsQueryable().ToPagedList(0, advices.Count())
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProductImageLink(string gtin)
        {
            var product = _productApplicationService.FindProductByGtin(gtin, false);

            var result = Json(product.ImageUrlMedium);

            return result;
        }

        [HttpPost]
        public JsonResult GetProducts(string searchMask, int maxResults)
        {
            var products = _productApplicationService.GetAllProducts();

            if (!string.IsNullOrEmpty(searchMask))
            {
                long temp;
                if (long.TryParse(searchMask, out temp))
                {
                    products = (from product in products
                                where product.GlobalTradeItemNumber.StartsWith(searchMask)
                                select product).ToList();
                }
                else
                {
                   products = (from product in products
                                where product.ProductName.ToLower().Contains(searchMask.ToLower())
                                select product).ToList();
                }
            }

            var result = from product in products.Take(maxResults)
                         select
                             new
                                 {
                                     product.Id,
                                     product.Brand.BrandName,
                                     product.ProductName, 
                                     CompanyName = product.Brand.Owner != null ? product.Brand.Owner.CompanyName : "",
                                     product.GlobalTradeItemNumber
                                 };

            return Json(result);
        }

        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Publish(int id)
        {
            _productAdviceApplicationService.PublishProductAdvice(id);
            return Json(null);
        }

        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Unpublish(int id)
        {
            _productAdviceApplicationService.UnpublishProductAdvice(id);
            return Json(null);
        }


        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            _productAdviceApplicationService.DeleteProductAdvice(id);
            return Json(null);
        }
    }
}
