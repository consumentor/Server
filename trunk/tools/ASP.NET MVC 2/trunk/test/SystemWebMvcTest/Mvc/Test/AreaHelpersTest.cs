namespace System.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AreaHelpersTest {

        [TestMethod]
        public void GetAreaNameFromAreaRouteCollectionRoute() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            AreaRegistrationContext context = new AreaRegistrationContext("area_name", routes);
            Route route = context.MapRoute(null, "the_url");

            // Act
            string areaName = AreaHelpers.GetAreaName(route);

            // Assert
            Assert.AreEqual("area_name", areaName);
        }

        [TestMethod]
        public void GetAreaNameFromIAreaAssociatedItem() {
            // Arrange
            CustomRouteWithArea route = new CustomRouteWithArea();

            // Act
            string areaName = AreaHelpers.GetAreaName(route);

            // Assert
            Assert.AreEqual("area_name", areaName);
        }

        [TestMethod]
        public void GetAreaNameFromRouteData() {
            // Arrange
            RouteData routeData = new RouteData();
            routeData.DataTokens["area"] = "area_name";

            // Act
            string areaName = AreaHelpers.GetAreaName(routeData);

            // Assert
            Assert.AreEqual("area_name", areaName);
        }

        [TestMethod]
        public void GetAreaNameFromRouteDataFallsBackToRoute() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            AreaRegistrationContext context = new AreaRegistrationContext("area_name", routes);
            Route route = context.MapRoute(null, "the_url");
            RouteData routeData = new RouteData(route, new MvcRouteHandler());

            // Act
            string areaName = AreaHelpers.GetAreaName(routeData);

            // Assert
            Assert.AreEqual("area_name", areaName);
        }

        [TestMethod]
        public void GetAreaNameReturnsNullIfRouteNotAreaAware() {
            // Arrange
            Route route = new Route("the_url", new MvcRouteHandler());

            // Act
            string areaName = AreaHelpers.GetAreaName(route);

            // Assert
            Assert.IsNull(areaName);
        }

        private class CustomRouteWithArea : RouteBase, IRouteWithArea {
            public string Area {
                get { return "area_name"; }
            }
            public override RouteData GetRouteData(HttpContextBase httpContext) {
                throw new NotImplementedException();
            }
            public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
                throw new NotImplementedException();
            }
        }

    }
}
