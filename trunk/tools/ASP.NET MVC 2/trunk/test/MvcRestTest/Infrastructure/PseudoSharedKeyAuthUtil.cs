using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Web.Mvc.Resources;
using System.Web.Security;

namespace MovieApp.Infrastructure {
    public class PseudoSharedKeyAuthUtil {
        //
        // !!! The following is not a legitimate security mechanism !!!
        //
        // The psuedo authentication header should have the form:
        //     x-pseudo-auth: <userName>:<passwordReversed>
        //
        const string authHeaderName = "x-pseudo-auth";

        public static void AuthenticateUser() {
            HttpContext httpContext = HttpContext.Current;
            if (httpContext != null) {
                string authHeader = httpContext.Request.Headers[authHeaderName];
                if (!string.IsNullOrEmpty(authHeader)) {
                    string userName = null;
                    string password = null;
                    string[] authSplit = authHeader.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (authSplit.Length == 2) {
                        userName = authSplit[0];
                        password = new string(authSplit[1].Reverse().ToArray());
                        if ((userName == "bob" && password == "password123") ||
                            (userName == "phil" && password == "123password")) {
                            if (userName == "bob") {
                                httpContext.User = new GenericPrincipal(new GenericIdentity(userName), new string[] { "admin" });
                            }
                            else {
                                httpContext.User = new GenericPrincipal(new GenericIdentity(userName), new string[0]);
                            }
                        }
                    }
                }
            }
        }

        public static void UpdateStatusCodeForFailedAuthentication() {
            HttpContext httpContext = HttpContext.Current;
            if (httpContext != null &&
                httpContext.Response.StatusCode == (int)HttpStatusCode.Found) {
                HttpRequestWrapper request = new HttpRequestWrapper(httpContext.Request);
                if (!request.IsBrowserRequest()) {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    httpContext.Response.ClearContent();
                    try {
                        if (httpContext.Response.Headers.AllKeys.Contains("Location")) {
                            httpContext.Response.Headers.Remove("Location");
                        }
                    }
                    catch (PlatformNotSupportedException) {
                        // We need IIS 7 integrated mode to remove the header.  Just swallow the exception if
                        // we can't remove the header.
                    }
                }
            }
        }
    }
}
