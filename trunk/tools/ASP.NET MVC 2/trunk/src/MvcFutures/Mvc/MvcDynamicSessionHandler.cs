namespace Microsoft.Web.Mvc {
    using System;
    using System.Web;
    using System.Web.SessionState;

    internal class MvcDynamicSessionHandler : IHttpAsyncHandler, IHttpHandler {

        private readonly IHttpAsyncHandler _originalHandler;

        public MvcDynamicSessionHandler(IHttpAsyncHandler originalHandler) {
            _originalHandler = originalHandler;
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            return _originalHandler.BeginProcessRequest(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result) {
            _originalHandler.EndProcessRequest(result);
        }

        public bool IsReusable {
            get { return _originalHandler.IsReusable; }
        }

        public void ProcessRequest(HttpContext context) {
            _originalHandler.ProcessRequest(context);
        }

    }
}
