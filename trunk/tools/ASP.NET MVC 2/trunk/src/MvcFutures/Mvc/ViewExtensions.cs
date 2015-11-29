namespace Microsoft.Web.Mvc {
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

    public static class ViewExtensions {

        public static void RenderRoute(this HtmlHelper helper, RouteValueDictionary routeValues) {
            if (routeValues == null) {
                throw new ArgumentNullException("routeValues");
            }

            string actionName = (string)routeValues["action"];
            helper.RenderAction(actionName, routeValues);
        }

        public static void RenderAction<TController>(this HtmlHelper helper, Expression<Action<TController>> action) where TController : Controller {
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(action);

            foreach (var entry in helper.ViewContext.RouteData.Values) {
                if (!rvd.ContainsKey(entry.Key)) {
                    rvd.Add(entry.Key, entry.Value);
                }
            }

            RenderRoute(helper, rvd);
        }

    }
}
