using System;
using System.Linq;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Domain.Extensions;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductApplicationService _productApplicationService;
        
        private readonly IBrandApplicationService _brandApplicationService;
        private readonly ICountryApplicationService _countryApplicationService;
        private readonly IIngredientApplicationService _ingredientApplicationService;
        private readonly ICertificationMarkDomainService _certificationMarkDomainService;

        public ProductController(IIngredientApplicationService ingredientApplicationService, IBrandApplicationService brandApplicationService, ICountryApplicationService countryApplicationService, IProductApplicationService productApplicationService, ICertificationMarkDomainService certificationMarkDomainService)
        {
            _productApplicationService = productApplicationService;
            _certificationMarkDomainService = certificationMarkDomainService;
            
            _countryApplicationService = countryApplicationService;
            _brandApplicationService = brandApplicationService;
            _ingredientApplicationService = ingredientApplicationService;
        }

        //
        // GET: /Product/

        public ActionResult Index()
        {
            //var products = _productApplicationService.GetAllProducts();
            return View();
        }

        [HttpPost]
        public JsonResult GetProducts(int? page, int? rows, string sidx, string sord, string searchMask)
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
            var productInfo = from product in products
                              select new
                                         {
                                             product.Id,
                                             product.ProductName,
                                             product.GlobalTradeItemNumber,
                                             product.Brand.BrandName
                                         };

            page = page.HasValue ? page.Value - 1 : 0;

            var data = productInfo.AsQueryable().ToPagedList(page.Value, rows ?? 10);

            double totalCount = data.TotalCount;
            double pageSize = data.PageSize;
            var jsonData = new
                               {
                                   total = Math.Round(totalCount/pageSize, MidpointRounding.AwayFromZero),
                                   page = data.PageIndex + 1,
                                   records = data.TotalCount,
                                   rows = data
                               };
            return Json(jsonData);
        }


        public ActionResult CreateProduct()
        {
            InitViewData();

            ViewData["Ingredients"] = _ingredientApplicationService.GetAllIngredients();

            return View();
        }

        [HttpPost]
        public ActionResult CreateProduct(Product newProduct, FormCollection formCollection)
        {
            if (_productApplicationService.FindProductByGtinInOwnDatabase(newProduct.GlobalTradeItemNumber, false) != null)
            {
                ModelState.AddModelError("GlobalTradeItemNumber", "Product already exists");
                InitViewData();
                ViewData["Ingredients"] = _ingredientApplicationService.GetAllIngredients();
                return View(newProduct);
            }
            if (!newProduct.GlobalTradeItemNumber.IsGtin())
            {
                ModelState.AddModelError("GlobalTradeItemNumber", "Not a proper GTIN");
                InitViewData();
                ViewData["Ingredients"] = _ingredientApplicationService.GetAllIngredients();
                return View(newProduct);
            }
            
                try
                {
                    newProduct = _productApplicationService.CreateProduct(newProduct);

                    var certificationMarks = formCollection.GetValues("CertificationMarks") ?? new string[] { };
                    _productApplicationService.RemoveAllCertificationMarksFromProduct(newProduct.Id);
                    foreach (var certificationMark in certificationMarks)
                    {
                        int certificationMarkId;
                        if (int.TryParse(certificationMark, out certificationMarkId))
                        {
                            _productApplicationService.AddCertificationMarkToProduct(newProduct.Id, int.Parse(certificationMark));
                        }
                    }

                    return RedirectToAction("Index");

                }
                catch
                {
               
                }
            

            InitViewData();
            ViewData["Ingredients"] = _ingredientApplicationService.GetAllIngredients();
            return View(newProduct);
        }

        public ActionResult EditProduct(int id)
        {
            var product = _productApplicationService.GetProduct(id);

           InitViewData();

            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product updatedProduct, FormCollection formCollection)
        {
            try
            {
                _productApplicationService.UpdateProduct(updatedProduct);

                var certificationMarks = formCollection.GetValues("CertificationMarks") ?? new string[]{};
                _productApplicationService.RemoveAllCertificationMarksFromProduct(updatedProduct.Id);
                foreach (var certificationMark in certificationMarks)
                {
                    int certificationMarkId;
                    if (int.TryParse(certificationMark, out certificationMarkId))
                    {
                        _productApplicationService.AddCertificationMarkToProduct(updatedProduct.Id, int.Parse(certificationMark));
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private void InitViewData()
        {
            var brands = _brandApplicationService.GetAllBrands();
            ViewData["Brand"] = new SelectList(brands, "Id", "BrandName");

            var countries = _countryApplicationService.GetAllCountries();
            ViewData["OriginCountry"] = new SelectList(countries, "Id", "CountryCode.Name");

            var certificationMarks = _certificationMarkDomainService.GetAllCertificationMarks();
            ViewData["CertificationMarks"] = certificationMarks;
        }
    }
}
