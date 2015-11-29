using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Consumentor.ShopGun.Web.CustomAttributes
{
    public class EnableJsonAttribute : ActionFilterAttribute
    {
        private readonly static string[] _jsonTypes = new string[] { "application/json", "text/json" };

        public bool RequireApiAuthorization { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (typeof(RedirectToRouteResult).IsInstanceOfType(filterContext.Result))
                return;

            var acceptTypes = filterContext.HttpContext.Request.AcceptTypes ?? new[] { "text/html" };

            var model = filterContext.Controller.ViewData.Model;

            var contentEncoding = filterContext.HttpContext.Request.ContentEncoding ??
                      Encoding.UTF8;

            if (_jsonTypes.Any(type => acceptTypes.Contains(type)) && CheckApiAuthorization(filterContext))



                filterContext.Result = new JsonResult()
                {
                    Data = model,
                    ContentEncoding = contentEncoding,
                    ContentType = filterContext.HttpContext.Request.ContentType,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
        }

        private bool CheckApiAuthorization(ActionExecutedContext filterContext)
        {
            if (!RequireApiAuthorization)
            {
                return true;
            }

            if (filterContext.HttpContext.Request.Params.AllKeys.Contains("apikey")
                && CheckKeyValidity(filterContext.HttpContext.Request.Params["apikey"]))
            {
                return true;
            }

            filterContext.Result = new HttpUnauthorizedResult();
            return false;
        }

        private bool CheckKeyValidity(string s)
        {
            return s == "moep";
        }
    }
}
