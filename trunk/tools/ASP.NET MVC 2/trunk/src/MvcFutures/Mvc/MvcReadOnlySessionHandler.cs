namespace Microsoft.Web.Mvc {
    using System;
    using System.Web;
    using System.Web.SessionState;

    internal class MvcReadOnlySessionHandler : MvcDynamicSessionHandler, IReadOnlySessionState {

        public MvcReadOnlySessionHandler(IHttpAsyncHandler originalHandler)
            : base(originalHandler) {
        }

    }
}
