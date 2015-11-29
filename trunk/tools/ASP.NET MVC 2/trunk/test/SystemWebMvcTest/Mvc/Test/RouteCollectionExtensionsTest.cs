namespace System.Web.Mvc.Test {
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RouteCollectionExtensionsTest {

        private static string[] _nameSpaces = new string[] { "nsA.nsB.nsC", "ns1.ns2.ns3" };

        [TestMethod]
        public void GetVirtualPathForAreaDoesNotStripAreaTokenIfAreasNotInUse() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            routes.MapRoute(
                "Default",
                "no-area/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = "" }
            );

            RequestContext requestContext = GetRequestContext(null);
            RouteValueDictionary values = new RouteValueDictionary() {
                { "controller", "home" },
                { "action", "about" },
                { "area", "some-area" }
            };

            // Act
            VirtualPathData vpd = routes.GetVirtualPathForArea(requestContext, values);

            // Assert
            Assert.IsNotNull(vpd);
            Assert.AreEqual(routes["Default"], vpd.Route);

            // note presence of 'area' query string parameter; RVD should not be modified if areas not in use
            Assert.AreEqual("/app/no-area/home/about?area=some-area", vpd.VirtualPath);
        }

        [TestMethod]
        public void GetVirtualPathForAreaForwardsCallIfRouteNameSpecified() {
            // Arrange
            RouteCollection routes = GetRouteCollection();
            RequestContext requestContext = GetRequestContext(null);
            RouteValueDictionary values = new RouteValueDictionary() {
                { "controller", "home" },
                { "action", "index" },
                { "area", "some-area" }
            };

            // Act
            VirtualPathData vpd = routes.GetVirtualPathForArea(requestContext, "admin_default", values);

            // Assert
            Assert.IsNotNull(vpd);
            Assert.AreEqual(routes["admin_default"], vpd.Route);

            // note presence of 'area' query string parameter; RVD should not be modified if route name was provided
            Assert.AreEqual("/app/admin-area?area=some-area", vpd.VirtualPath);
        }

        [TestMethod]
        public void GetVirtualPathForAreaThrowsIfRoutesIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    RouteCollectionExtensions.GetVirtualPathForArea(null, null, null);
                }, "routes");
        }

        [TestMethod]
        public void GetVirtualPathForAreaWillJumpBetweenAreasExplicitly() {
            // Arrange
            RouteCollection routes = GetRouteCollection();
            RequestContext requestContext = GetRequestContext(null);
            RouteValueDictionary values = new RouteValueDictionary() {
                { "controller", "home" },
                { "action", "tenmostrecent" },
                { "tag", "some-tag" },
                { "area", "blog" }
            };

            // Act
            VirtualPathData vpd = routes.GetVirtualPathForArea(requestContext, values);

            // Assert
            Assert.IsNotNull(vpd);
            Assert.AreEqual(routes["blog_whatsnew"], vpd.Route);
            Assert.AreEqual("/app/whats-new/some-tag", vpd.VirtualPath);
        }

        [TestMethod]
        public void GetVirtualPathForAreaWillNotJumpBetweenAreasImplicitly() {
            // Arrange
            RouteCollection routes = GetRouteCollection();
            RequestContext requestContext = GetRequestContext("admin");
            RouteValueDictionary values = new RouteValueDictionary() {
                { "controller", "home" },
                { "action", "tenmostrecent" },
                { "tag", "some-tag" }
            };

            // Act
            VirtualPathData vpd = routes.GetVirtualPathForArea(requestContext, values);

            // Assert
            Assert.IsNotNull(vpd);
            Assert.AreEqual(routes["admin_default"], vpd.Route);
            Assert.AreEqual("/app/admin-area/home/tenmostrecent?tag=some-tag", vpd.VirtualPath);
        }

        [TestMethod]
        public void MapRoute3() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act
            routes.MapRoute("RouteName", "SomeUrl");

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
            Assert.AreEqual(0, route.DataTokens.Count);
        }

        [TestMethod]
        public void MapRoute3WithNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            //string[] namespaces = new string[] { "nsA.nsB.nsC", "ns1.ns2.ns3" };

            // Act
            routes.MapRoute("RouteName", "SomeUrl", _nameSpaces);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNotNull(route.DataTokens);
            Assert.IsNotNull(route.DataTokens["Namespaces"]);
            string[] routeNameSpaces = route.DataTokens["Namespaces"] as string[];
            Assert.AreEqual(routeNameSpaces.Length, 2);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreSame(routeNameSpaces, _nameSpaces);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute3WithEmptyNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act
            routes.MapRoute("RouteName", "SomeUrl", new string[] { });

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual(0, route.Defaults.Count);
            Assert.AreEqual(0, route.Constraints.Count);
            Assert.AreEqual(0, route.DataTokens.Count);
        }

        [TestMethod]
        public void MapRoute4() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var defaults = new { Foo = "DefaultFoo" };

            // Act
            routes.MapRoute("RouteName", "SomeUrl", defaults);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual("DefaultFoo", route.Defaults["Foo"]);
            Assert.AreEqual(0, route.Constraints.Count);
            Assert.AreEqual(0, route.DataTokens.Count);
        }

        [TestMethod]
        public void MapRoute4WithNameSpaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var defaults = new { Foo = "DefaultFoo" };

            // Act
            routes.MapRoute("RouteName", "SomeUrl", defaults, _nameSpaces);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.IsNotNull(route.DataTokens);
            Assert.IsNotNull(route.DataTokens["Namespaces"]);
            string[] routeNameSpaces = route.DataTokens["Namespaces"] as string[];
            Assert.AreEqual(routeNameSpaces.Length, 2);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreSame(routeNameSpaces, _nameSpaces);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual("DefaultFoo", route.Defaults["Foo"]);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void MapRoute5() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var defaults = new { Foo = "DefaultFoo" };
            var constraints = new { Foo = "ConstraintFoo" };

            // Act
            routes.MapRoute("RouteName", "SomeUrl", defaults, constraints);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreSame(route, routes["RouteName"]);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(MvcRouteHandler));
            Assert.AreEqual("DefaultFoo", route.Defaults["Foo"]);
            Assert.AreEqual("ConstraintFoo", route.Constraints["Foo"]);
            Assert.AreEqual(0, route.DataTokens.Count);
        }

        [TestMethod]
        public void MapRoute5WithNullRouteCollectionThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    RouteCollectionExtensions.MapRoute(null, null, null, null, null);
                },
                "routes");
        }

        [TestMethod]
        public void MapRoute5WithNullUrlThrows() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    routes.MapRoute(null, null /* url */, null, null);
                },
                "url");
        }

        [TestMethod]
        public void IgnoreRoute1WithNullRouteCollectionThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    RouteCollectionExtensions.IgnoreRoute(null, "foo");
                },
                "routes");
        }

        [TestMethod]
        public void IgnoreRoute1WithNullUrlThrows() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    routes.IgnoreRoute(null);
                },
                "url");
        }

        [TestMethod]
        public void IgnoreRoute3() {
            // Arrange
            RouteCollection routes = new RouteCollection();

            // Act
            routes.IgnoreRoute("SomeUrl");

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(StopRoutingHandler));
            Assert.IsNull(route.Defaults);
            Assert.AreEqual(0, route.Constraints.Count);
        }

        [TestMethod]
        public void IgnoreRoute4() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            var constraints = new { Foo = "DefaultFoo" };

            // Act
            routes.IgnoreRoute("SomeUrl", constraints);

            // Assert
            Assert.AreEqual(1, routes.Count);
            Route route = routes[0] as Route;
            Assert.IsNotNull(route);
            Assert.AreEqual("SomeUrl", route.Url);
            Assert.IsInstanceOfType(route.RouteHandler, typeof(StopRoutingHandler));
            Assert.IsNull(route.Defaults);
            Assert.AreEqual(1, route.Constraints.Count);
            Assert.AreEqual("DefaultFoo", route.Constraints["Foo"]);
        }

        [TestMethod]
        public void IgnoreRouteInternalNeverMatchesUrlGeneration() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            routes.IgnoreRoute("SomeUrl");
            Route route = routes[0] as Route;

            // Act
            VirtualPathData vpd = route.GetVirtualPath(new RequestContext(new Mock<HttpContextBase>().Object, new RouteData()), null);

            // Assert
            Assert.IsNull(vpd);
        }

        private static RequestContext GetRequestContext(string currentAreaName) {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Request.ApplicationPath).Returns("/app");
            mockHttpContext.Expect(c => c.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(virtualPath => virtualPath);

            RouteData routeData = new RouteData();
            routeData.DataTokens["area"] = currentAreaName;
            return new RequestContext(mockHttpContext.Object, routeData);
        }

        private static RouteCollection GetRouteCollection() {
            RouteCollection routes = new RouteCollection();
            routes.MapRoute(
                "Default",
                "no-area/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = "" }
            );

            AreaRegistrationContext blogContext = new AreaRegistrationContext("blog", routes);
            blogContext.MapRoute(
                "Blog_WhatsNew",
                "whats-new/{tag}",
                new { controller = "Home", action = "TenMostRecent", tag = "" }
            );
            blogContext.MapRoute(
                "Blog_Default",
                "blog-area/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = "" }
            );

            AreaRegistrationContext adminContext = new AreaRegistrationContext("admin", routes);
            adminContext.MapRoute(
                "Admin_Default",
                "admin-area/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = "" }
            );

            return routes;
        }

    }
}
