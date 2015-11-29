namespace Microsoft.Web.Mvc {
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Routing;
    using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

    public static class ControllerExtensions {

        // Shortcut to allow users to write this.RedirectToAction(x => x.OtherMethod()) to redirect
        // to a different method on the same controller.
        public static RedirectToRouteResult RedirectToAction<TController>(this TController controller, Expression<Action<TController>> action) where TController : Controller {
            return RedirectToAction<TController>((Controller)controller, action);
        }

        public static RedirectToRouteResult RedirectToAction<TController>(this Controller controller, Expression<Action<TController>> action) where TController : Controller {
            if (controller == null) {
                throw new ArgumentNullException("controller");
            }

            RouteValueDictionary routeValues = ExpressionHelper.GetRouteValuesFromExpression(action);
            return new RedirectToRouteResult(routeValues);
        }

    }
}
