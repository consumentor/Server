namespace System.Web.Mvc {
    using System;

    public class HttpUnauthorizedResult : ActionResult {

        public override void ExecuteResult(ControllerContext context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            // HTTP 401 is the status code for unauthorized access. Other code might
            // intercept this and perform some special logic. For example, the
            // FormsAuthenticationModule looks for 401 responses and instead redirects
            // the user to the login page.
            context.HttpContext.Response.StatusCode = 401;
        }
    }
}
