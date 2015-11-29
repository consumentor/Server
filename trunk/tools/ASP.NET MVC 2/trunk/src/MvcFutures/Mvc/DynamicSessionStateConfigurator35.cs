namespace Microsoft.Web.Mvc {
    using System;
    using System.Web;
    using System.Web.Mvc;

    internal sealed class DynamicSessionStateConfigurator35 : IDynamicSessionStateConfigurator {

        private readonly HttpContextBase _httpContext;

        public DynamicSessionStateConfigurator35(HttpContextBase httpContext) {
            _httpContext = httpContext;
        }

        public void ConfigureSessionState(ControllerSessionState mode) {
            switch (mode) {
                case ControllerSessionState.Disabled:
                    _httpContext.Handler = new MvcDynamicSessionHandler((MvcHandler)_httpContext.Handler);
                    break;

                case ControllerSessionState.ReadOnly:
                    _httpContext.Handler = new MvcReadOnlySessionHandler((MvcHandler)_httpContext.Handler);
                    break;

                default:
                    // Let the default MvcHandler take care of everything else.
                    break;
            }
        }

    }
}
