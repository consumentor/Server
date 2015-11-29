namespace Microsoft.Web.Mvc.AspNet4 {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Web.Mvc.AspNet4.Resources;

    [AttributeUsage(AttributeTargets.Property)]
    public class RemoteAttribute : ValidationAttribute, IClientValidatable {
        protected RemoteAttribute(string parameterName) {
            if (String.IsNullOrWhiteSpace(parameterName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "parameterName");
            }

            ParameterName = parameterName;
            RouteData = new RouteValueDictionary();
        }

        public RemoteAttribute(string action, string controller, string parameterName) :
            this(action, controller, null /* areaName */, parameterName) {
        }

        public RemoteAttribute(string action, string controller, string areaName, string parameterName)
            : this(parameterName) {
            if (String.IsNullOrWhiteSpace(action)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "action");
            }
            if (String.IsNullOrWhiteSpace(controller)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controller");
            }

            RouteData["controller"] = controller;
            RouteData["action"] = action;

            if (!String.IsNullOrWhiteSpace(areaName)) {
                RouteData["area"] = areaName;
            }
        }

        public RemoteAttribute(string routeName, string parameterName)
            : this(parameterName) {
            if (String.IsNullOrWhiteSpace(routeName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "routeName");
            }

            RouteName = routeName;
        }

        protected string ParameterName { get; set; }

        protected RouteValueDictionary RouteData { get; set; }

        protected virtual RouteCollection Routes {
            get {
                return RouteTable.Routes;
            }
        }

        protected string RouteName { get; set; }

        protected virtual string GetUrl(ControllerContext controllerContext) {
            var pathData = Routes.GetVirtualPathForArea(controllerContext.RequestContext,
                                                        RouteName,
                                                        RouteData);

            if (pathData == null)
                throw new InvalidOperationException("No route matched!");

            return pathData.VirtualPath;
        }

        public override bool IsValid(object value) {
            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {
            ModelClientValidationRule rule = new ModelClientValidationRule() {
                ErrorMessage = FormatErrorMessage(metadata.PropertyName),
                ValidationType = "remote"
            };

            rule.ValidationParameters["url"] = GetUrl(context);
            rule.ValidationParameters["parameterName"] = ParameterName;
            return new ModelClientValidationRule[] { rule };
        }
    }
}
