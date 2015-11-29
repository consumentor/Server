using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Mvc.Resources;
using MovieApp.Infrastructure;

namespace MovieApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapResourceRoute("Movies", "{id}");

            routes.MapResourceRoute("EdmMovies", "{id}");

            routes.MapRoute(
                "POX",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = "" }
            );

            routes.MapRoute(
                "DefaultMovies",
                "",
                new { controller = "Movies", action = "Index", id = "" }
            );

            routes.MapRoute(
                "DefaultEdmMovies",
                "",
                new { controller = "EdmMovies", action = "Index", id = "" }
            );
        }

        protected void Application_Start()
        {
            // We use this hook to inject our ResourceControllerActionInvoker which can smartly map HTTP verbs to Actions
            ResourceControllerFactory factory = new ResourceControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(factory);

            // We use this hook to inject the ResourceModelBinder behavior which can de-serialize from xml/json formats 
            ModelBinders.Binders.DefaultBinder = new ResourceModelBinder();

            AtomFormatHandler atom = new AtomFormatHandler();
            FormatManager.Current.RequestFormatHandlers.Add(atom);
            FormatManager.Current.ResponseFormatHandlers.Add(atom);
            JavaScriptSerializerFormatHandler jsonHandler = new JavaScriptSerializerFormatHandler();
            FormatManager.Current.RequestFormatHandlers.Add(jsonHandler);
            FormatManager.Current.ResponseFormatHandlers.Add(jsonHandler);

            FormatManager.Current.FormatHelper = new ChromeAwareFormatHelper();
            
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            PseudoSharedKeyAuthUtil.AuthenticateUser();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            PseudoSharedKeyAuthUtil.UpdateStatusCodeForFailedAuthentication();
        }

        // example of how to repalce the just manager
        class MyFormatManager : FormatManager
        {
            static void Initialize()
            {
                // Replace the OOB request and response handlers for json
                FormatManager.Current = new MyFormatManager();
            }

            public MyFormatManager()
            {
                XmlFormatHandler xmlHandler = new XmlFormatHandler();
                this.RequestFormatHandlers.Add(xmlHandler);
                this.ResponseFormatHandlers.Add(xmlHandler);
                JsonFormatHandler jsonHandler = new JsonFormatHandler();
                this.RequestFormatHandlers.Add(jsonHandler);
                this.ResponseFormatHandlers.Add(jsonHandler);
                AtomFormatHandler atom = new AtomFormatHandler();
                this.RequestFormatHandlers.Add(atom);
                this.ResponseFormatHandlers.Add(atom);
                JavaScriptSerializerFormatHandler json2Handler = new JavaScriptSerializerFormatHandler();
                this.RequestFormatHandlers.Add(json2Handler);
                this.ResponseFormatHandlers.Add(json2Handler);
            }
        }

        // this FormatHelper is to woarkaround the Chrome browser's behavior of advertising a preference to receive xml
        // which it does by sending the following Accept header:
        // Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
        class ChromeAwareFormatHelper : DefaultFormatHelper
        {
            public override List<ContentType> GetResponseFormats(RequestContext requestContext)
            {
                List<ContentType> result = base.GetResponseFormats(requestContext);
                if (result != null)
                {
                    string userAgent =  requestContext.HttpContext.Request.UserAgent;
                    // quick & dirty check for Chrome, we're trying to match something like this:
                    // User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/532.5 (KHTML, like Gecko) Chrome/4.0.249.78 Safari/532.5
                    if (userAgent != null && userAgent.Contains(" Chrome/"))
                    {
                        foreach (ContentType ct in result)
                        {
                            // if we find html, we'll just move it at the top of the list
                            if (ct.MediaType.EndsWith("/html"))
                            {
                                result.Remove(ct);
                                result.Insert(0, ct);
                                break;
                            }
                        }
                    }
                }
                return result;
            }
        }
    }
}
