namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Web.Mvc;

    public static class UriHelperExtensions {
        /// <summary>
        /// Generates the route URL for the resource controller's Retrieve action
        /// </summary>
        /// <param name="url"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string ResourceUrl(this UrlHelper url, string controllerName, object routeValues) {
            return url.ResourceUrl(controllerName, routeValues, ActionType.Retrieve);
        }

        /// <summary>
        /// Generates the route URL for the resource
        /// </summary>
        /// <param name="url"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public static string ResourceUrl(this UrlHelper url, string controllerName, object routeValues, ActionType actionType) {
            switch (actionType) {
                case ActionType.GetUpdateForm:
                    return url.RouteUrl(controllerName + "-editForm", routeValues);
                case ActionType.GetCreateForm:
                    return url.RouteUrl(controllerName + "-createForm");
                case ActionType.Retrieve:
                case ActionType.Delete:
                case ActionType.Update:
                    return url.RouteUrl(controllerName, routeValues);
                case ActionType.Create:
                    return url.RouteUrl(controllerName + "-create");
                case ActionType.Index:
                    return url.RouteUrl(controllerName + "-index");
                default:
                    throw new ArgumentOutOfRangeException("actionType");
            }
        }
    }
}
