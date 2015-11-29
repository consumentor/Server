namespace Microsoft.Web.Mvc {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public sealed class MvcDynamicSessionControllerFactory : IControllerFactory {

        private static readonly object _controllerItemKey = new object();

        private readonly IControllerFactory _originalFactory;

        public MvcDynamicSessionControllerFactory()
            : this(new DefaultControllerFactory()) {
        }

        public MvcDynamicSessionControllerFactory(IControllerFactory originalFactory) {
            if (originalFactory == null) {
                throw new ArgumentNullException("originalFactory");
            }

            _originalFactory = originalFactory;
        }

        // This method is called at most once for each request, and if it's called it's called within
        // PostMapRequestHandler.
        internal IController CreateCachedController(RequestContext requestContext, string controllerName) {
            // The ControllerContainer is used because CreateController() might return null, and we want
            // to be sure we've called it only once for each request.
            ControllerContainer cc = new ControllerContainer() {
                Controller = _originalFactory.CreateController(requestContext, controllerName)
            };

            requestContext.HttpContext.Items[_controllerItemKey] = cc;
            return cc.Controller;
        }

        public IController CreateController(RequestContext requestContext, string controllerName) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }

            IController controller;
            if (TryGetAndRemoveControllerFromCache(requestContext.HttpContext, out controller)) {
                return controller;
            }
            else {
                return _originalFactory.CreateController(requestContext, controllerName);
            }
        }

        internal void ReleaseCachedController(HttpContextBase httpContext) {
            IController controller;
            TryGetAndRemoveControllerFromCache(httpContext, out controller);
            if (controller != null) {
                ReleaseController(controller);
            }
        }

        public void ReleaseController(IController controller) {
            _originalFactory.ReleaseController(controller);
        }

        // If Items contains this key, then the handler is asking for the controller that was cached
        // during PostMapRequestHandler. We need to remove the key so that all calls *except the first*
        // that occur within the handler don't use the cached item.
        private static bool TryGetAndRemoveControllerFromCache(HttpContextBase httpContext, out IController controller) {
            ControllerContainer cc = httpContext.Items[_controllerItemKey] as ControllerContainer;
            if (cc != null) {
                httpContext.Items.Remove(_controllerItemKey);
                controller = cc.Controller;
                return true;
            }
            else {
                controller = null;
                return false;
            }
        }

        private sealed class ControllerContainer {
            public IController Controller { get; set; }
        }

    }
}
