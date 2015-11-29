namespace Microsoft.Web.Mvc {
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Web.Resources;

    public sealed class MvcDynamicSessionModule : IHttpModule {

        private static readonly object _controllerFactoryItemKey = new object();
        private static readonly ControllerSessionStateCache _controllerSessionStateCache = new ControllerSessionStateCache();
        private static readonly bool _useLegacyRoutingPipeline = (typeof(HttpContext).Assembly.GetType("System.Web.SessionState.SessionStateBehavior", false /* throwOnError */) == null);

        private ControllerBuilder _controllerBuilder;

        internal ControllerBuilder ControllerBuilder {
            get {
                return _controllerBuilder ?? ControllerBuilder.Current;
            }
            set {
                _controllerBuilder = value;
            }
        }

        public void Dispose() {
        }

        private static IDynamicSessionStateConfigurator GetSessionStateConfigurator(HttpContextBase httpContext) {
            if (_useLegacyRoutingPipeline) {
                return new DynamicSessionStateConfigurator35(httpContext);
            }
            else {
                return new DynamicSessionStateConfigurator40(httpContext);
            }
        }

        private static ControllerSessionState GetSessionStateMode(IController controller) {
            if (controller == null) {
                return ControllerSessionState.Default;
            }

            return _controllerSessionStateCache.GetSessionStateMode(controller.GetType());
        }

        public void Init(HttpApplication application) {
            // In ASP.NET 3.5, this works by replacing the original MvcHandler with a new IHttpHandler
            // that implements the correct marker interface for the session behavior we wish to support.
            // However, in ASP.NET 4, we can't set the Context property because PostMapRequestHandler
            // is now too late due to other Routing changes. However, we can detect the presence of new
            // ASP.NET 4 Session APIs and call into them if they're available, which mitigates this problem.

            application.PostMapRequestHandler += (sender, e) => {
                HttpContextBase context = new HttpContextWrapper(((HttpApplication)sender).Context);
                IDynamicSessionStateConfigurator configurator = GetSessionStateConfigurator(context);
                SetSessionStateMode(context, configurator);
            };

            application.EndRequest += (sender, e) => {
                HttpContextBase context = new HttpContextWrapper(((HttpApplication)sender).Context);
                PossiblyReleaseController(context);
            };
        }

        internal static void PossiblyReleaseController(HttpContextBase context) {
            // It's possible that the factory can create the cached controller but that the MVC pipeline doesn't run
            // or doesn't make it as far as the CreateController() call. In this case, we need to release the cached
            // controller manually in case the controller is a pooled resource.

            MvcDynamicSessionControllerFactory factory = context.Items[_controllerFactoryItemKey] as MvcDynamicSessionControllerFactory;
            if (factory != null) {
                factory.ReleaseCachedController(context);
            }
        }

        internal void SetSessionStateMode(HttpContextBase context, IDynamicSessionStateConfigurator configurator) {
            MvcHandler mvcHandler = context.Handler as MvcHandler;
            if (mvcHandler == null) {
                // Either MvcHttpHandler was called directly, Routing hasn't run, or Routing has run
                // and the chosen handler isn't MVC. There's nothing we can do here.
                return;
            }

            // Check to see that our factory is installed, otherwise the controller factory might be asked to
            // create two instances of the controller for every request, which could lead to resource or
            // scalability issues.
            RequestContext requestContext = mvcHandler.RequestContext;
            MvcDynamicSessionControllerFactory factory = ControllerBuilder.GetControllerFactory() as MvcDynamicSessionControllerFactory;
            if (factory == null) {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture,
                    MvcResources.MvcDynamicSessionModule_WrongControllerFactory, typeof(MvcDynamicSessionControllerFactory)));
            }

            context.Items[_controllerFactoryItemKey] = factory; // save a reference to this factory so that we can dispose of the cached controller
            string controllerName = requestContext.RouteData.GetRequiredString("controller");
            IController controller = factory.CreateCachedController(requestContext, controllerName);
            ControllerSessionState sessionStateMode = GetSessionStateMode(controller);
            configurator.ConfigureSessionState(sessionStateMode);
        }

        private sealed class ControllerSessionStateCache : ReaderWriterCache<Type, ControllerSessionState> {
            public ControllerSessionState GetSessionStateMode(Type type) {
                return FetchOrCreateItem(type, () => ReadSessionStateModeFromType(type));
            }

            private static ControllerSessionState ReadSessionStateModeFromType(Type type) {
                ControllerSessionStateAttribute attr = type.GetCustomAttributes(typeof(ControllerSessionStateAttribute), true /* inherit */).OfType<ControllerSessionStateAttribute>().FirstOrDefault();
                return (attr != null) ? attr.Mode : ControllerSessionState.Default;
            }
        }

    }
}
