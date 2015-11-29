namespace System.Web.Mvc.Html.Test {
    using System;
    using System.IO;
    using System.Web.Mvc.Resources;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ChildActionExtensionsTest {
        Mock<HtmlHelper> htmlHelper;
        Mock<HttpContextBase> httpContext;
        Mock<RouteBase> route;
        Mock<IViewDataContainer> viewDataContainer;

        RouteData originalRouteData;
        RouteCollection routes;
        ViewContext viewContext;
        VirtualPathData virtualPathData;

        [TestInitialize]
        public void Setup() {
            route = new Mock<RouteBase>();
            route.Expect(r => r.GetVirtualPath(It.IsAny<RequestContext>(), It.IsAny<RouteValueDictionary>()))
                 .Returns(() => virtualPathData);

            virtualPathData = new VirtualPathData(route.Object, "~/VirtualPath");

            routes = new RouteCollection();
            routes.Add(route.Object);

            originalRouteData = new RouteData();

            string returnValue = "";
            httpContext = new Mock<HttpContextBase>();
            httpContext.Expect(hc => hc.Request.ApplicationPath).Returns("~");
            httpContext.Expect(hc => hc.Response.ApplyAppPathModifier(It.IsAny<string>()))
                       .Callback<string>(s => returnValue = s)
                       .Returns(() => returnValue);
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()));

            viewContext = new ViewContext {
                RequestContext = new RequestContext(httpContext.Object, originalRouteData)
            };

            viewDataContainer = new Mock<IViewDataContainer>();

            htmlHelper = new Mock<HtmlHelper>(viewContext, viewDataContainer.Object, routes);
        }

        [TestMethod]
        public void GuardClauses() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => ChildActionExtensions.ActionHelper(null /* htmlHelper */, "abc", null /* controllerName */, null /* routeValues */, null /* textWriter */),
                "htmlHelper"
            );

            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => ChildActionExtensions.ActionHelper(htmlHelper.Object, null /* actionName */, null /* controllerName */, null /* routeValues */, null /* textWriter */),
                "actionName"
            );

            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => ChildActionExtensions.ActionHelper(htmlHelper.Object, String.Empty /* actionName */, null /* controllerName */, null /* routeValues */, null /* textWriter */),
                "actionName"
            );
        }

        [TestMethod]
        public void ServerExecuteCalledWithWrappedChildActionMvcHandler() {
            // Arrange
            IHttpHandler callbackHandler = null;
            TextWriter callbackTextWriter = null;
            bool callbackPreserveForm = false;
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()))
                       .Callback<IHttpHandler, TextWriter, bool>(
                           (handler, textWriter, preserveForm) =>
                           {
                               callbackHandler = handler;
                               callbackTextWriter = textWriter;
                               callbackPreserveForm = preserveForm;
                           });
            TextWriter stringWriter = new StringWriter();

            // Act
            ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", null /* controllerName */, null /* routeValues */, stringWriter);

            // Assert
            Assert.IsNotNull(callbackHandler);
            HttpHandlerUtil.ServerExecuteHttpHandlerWrapper wrapper = callbackHandler as HttpHandlerUtil.ServerExecuteHttpHandlerWrapper;
            Assert.IsNotNull(wrapper);
            Assert.IsNotNull(wrapper.InnerHandler);
            ChildActionExtensions.ChildActionMvcHandler childHandler = wrapper.InnerHandler as ChildActionExtensions.ChildActionMvcHandler;
            Assert.IsNotNull(childHandler);
            Assert.AreSame(stringWriter, callbackTextWriter);
            Assert.IsTrue(callbackPreserveForm);
        }

        [TestMethod]
        public void RouteDataTokensIncludesParentActionViewContext() {
            // Arrange
            MvcHandler mvcHandler = null;
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()))
                       .Callback<IHttpHandler, TextWriter, bool>((handler, _, __) => mvcHandler = (MvcHandler)((HttpHandlerUtil.ServerExecuteHttpHandlerWrapper)handler).InnerHandler);

            // Act
            ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", null /* controllerName */, null /* routeValues */, null /* textWriter */);

            // Assert
            Assert.AreSame(viewContext, mvcHandler.RequestContext.RouteData.DataTokens[ControllerContext.PARENT_ACTION_VIEWCONTEXT]);
        }

        [TestMethod]
        public void RouteValuesIncludeNewActionName() {
            // Arrange
            MvcHandler mvcHandler = null;
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()))
                       .Callback<IHttpHandler, TextWriter, bool>((handler, _, __) => mvcHandler = (MvcHandler)((HttpHandlerUtil.ServerExecuteHttpHandlerWrapper)handler).InnerHandler);

            // Act
            ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", null /* controllerName */, null /* routeValues */, null /* textWriter */);

            // Assert
            RouteData routeData = mvcHandler.RequestContext.RouteData;
            Assert.AreEqual("actionName", routeData.Values["action"]);
        }

        [TestMethod]
        public void RouteValuesIncludeOldControllerNameWhenControllerNameIsNullOrEmpty() {
            // Arrange
            originalRouteData.Values["controller"] = "oldController";
            MvcHandler mvcHandler = null;
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()))
                       .Callback<IHttpHandler, TextWriter, bool>((handler, _, __) => mvcHandler = (MvcHandler)((HttpHandlerUtil.ServerExecuteHttpHandlerWrapper)handler).InnerHandler);

            // Act
            ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", null /* controllerName */, null /* routeValues */, null /* textWriter */);

            // Assert
            RouteData routeData = mvcHandler.RequestContext.RouteData;
            Assert.AreEqual("oldController", routeData.Values["controller"]);
        }

        [TestMethod]
        public void RouteValuesIncludeNewControllerNameWhenControllNameIsNotEmpty() {
            // Arrange
            originalRouteData.Values["controller"] = "oldController";
            MvcHandler mvcHandler = null;
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()))
                       .Callback<IHttpHandler, TextWriter, bool>((handler, _, __) => mvcHandler = (MvcHandler)((HttpHandlerUtil.ServerExecuteHttpHandlerWrapper)handler).InnerHandler);

            // Act
            ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", "newController", null /* routeValues */, null /* textWriter */);

            // Assert
            RouteData routeData = mvcHandler.RequestContext.RouteData;
            Assert.AreEqual("newController", routeData.Values["controller"]);
        }

        [TestMethod]
        public void PassedRouteValuesOverrideParentRequestRouteValues() {
            // Arrange
            originalRouteData.Values["name1"] = "value1";
            originalRouteData.Values["name2"] = "value2";
            MvcHandler mvcHandler = null;
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()))
                       .Callback<IHttpHandler, TextWriter, bool>((handler, _, __) => mvcHandler = (MvcHandler)((HttpHandlerUtil.ServerExecuteHttpHandlerWrapper)handler).InnerHandler);

            // Act
            ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", null /* controllerName */, new RouteValueDictionary { { "name2", "newValue2" } }, null /* textWriter */);

            // Assert
            RouteData routeData = mvcHandler.RequestContext.RouteData;
            Assert.AreEqual("value1", routeData.Values["name1"]);
            Assert.AreEqual("newValue2", routeData.Values["name2"]);
        }

        [TestMethod]
        public void RouteValuesDoesNotIncludeExplicitlyPassedAreaName() {
            // Arrange
            Route route = routes.MapRoute("my-area", "my-area");
            route.DataTokens["area"] = "myArea";
            MvcHandler mvcHandler = null;
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()))
                       .Callback<IHttpHandler, TextWriter, bool>((handler, _, __) => mvcHandler = (MvcHandler)((HttpHandlerUtil.ServerExecuteHttpHandlerWrapper)handler).InnerHandler);

            // Act
            ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", null /* controllerName */, new RouteValueDictionary { { "area", "myArea" } }, null /* textWriter */);

            // Assert
            RouteData routeData = mvcHandler.RequestContext.RouteData;
            Assert.IsFalse(routeData.Values.ContainsKey("area"));
        }

        [TestMethod]
        public void RouteValuesDoesIncludesExplicitlyPassedAreaNameIfAreasNotInUse() {
            // Arrange
            Route route = routes.MapRoute("my-area", "my-area");
            MvcHandler mvcHandler = null;
            httpContext.Expect(hc => hc.Server.Execute(It.IsAny<IHttpHandler>(), It.IsAny<TextWriter>(), It.IsAny<bool>()))
                       .Callback<IHttpHandler, TextWriter, bool>((handler, _, __) => mvcHandler = (MvcHandler)((HttpHandlerUtil.ServerExecuteHttpHandlerWrapper)handler).InnerHandler);

            // Act
            ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", null /* controllerName */, new RouteValueDictionary { { "area", "myArea" } }, null /* textWriter */);

            // Assert
            RouteData routeData = mvcHandler.RequestContext.RouteData;
            Assert.IsTrue(routeData.Values.ContainsKey("area"));
        }

        [TestMethod]
        public void NoMatchingRouteThrows() {
            // Arrange
            routes.Clear();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => ChildActionExtensions.ActionHelper(htmlHelper.Object, "actionName", null /* controllerName */, null /* routeValues */, null /* textWriter */),
                MvcResources.Common_NoRouteMatched
            );
        }

    }
}
