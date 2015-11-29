namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class AreaRegistrationContextTest {

        [TestMethod]
        public void ConstructorSetsProperties() {
            // Arrange
            string areaName = "the_area";
            RouteCollection routes = new RouteCollection();

            // Act
            AreaRegistrationContext context = new AreaRegistrationContext(areaName, routes);

            // Assert
            Assert.AreEqual(areaName, context.AreaName);
            Assert.AreSame(routes, context.Routes);
        }

        [TestMethod]
        public void ConstructorThrowsIfAreaNameIsEmpty() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new AreaRegistrationContext("", new RouteCollection());
                }, "areaName");
        }

        [TestMethod]
        public void ConstructorThrowsIfAreaNameIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new AreaRegistrationContext(null, new RouteCollection());
                }, "areaName");
        }

        [TestMethod]
        public void ConstructorThrowsIfRoutesIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new AreaRegistrationContext("the_area", null);
                }, "routes");
        }

        [TestMethod]
        public void MapRouteWithEmptyStringNamespaces() {
            // Arrange
            string[] implicitNamespaces = new string[] { "implicit_1", "implicit_2" };
            string[] explicitNamespaces = new string[0];

            RouteCollection routes = new RouteCollection();
            AreaRegistrationContext context = new AreaRegistrationContext("the_area", routes);
            ReplaceCollectionContents(context.Namespaces, implicitNamespaces);

            // Act
            Route route = context.MapRoute("the_name", "the_url", explicitNamespaces);

            // Assert
            Assert.AreEqual(route, routes["the_name"]);
            Assert.AreEqual("the_area", route.DataTokens["area"]);
            Assert.AreEqual(true, route.DataTokens["UseNamespaceFallback"]);
            Assert.IsNull(route.DataTokens["namespaces"]);
        }

        [TestMethod]
        public void MapRouteWithExplicitNamespaces() {
            // Arrange
            string[] implicitNamespaces = new string[] { "implicit_1", "implicit_2" };
            string[] explicitNamespaces = new string[] { "explicit_1", "explicit_2" };

            RouteCollection routes = new RouteCollection();
            AreaRegistrationContext context = new AreaRegistrationContext("the_area", routes);
            ReplaceCollectionContents(context.Namespaces, implicitNamespaces);

            // Act
            Route route = context.MapRoute("the_name", "the_url", explicitNamespaces);

            // Assert
            Assert.AreEqual(route, routes["the_name"]);
            Assert.AreEqual("the_area", route.DataTokens["area"]);
            Assert.AreEqual(false, route.DataTokens["UseNamespaceFallback"]);
            CollectionAssert.AreEqual(explicitNamespaces, (string[])route.DataTokens["namespaces"]);
        }

        [TestMethod]
        public void MapRouteWithImplicitNamespaces() {
            // Arrange
            string[] implicitNamespaces = new string[] { "implicit_1", "implicit_2" };
            string[] explicitNamespaces = new string[] { "explicit_1", "explicit_2" };

            RouteCollection routes = new RouteCollection();
            AreaRegistrationContext context = new AreaRegistrationContext("the_area", routes);
            ReplaceCollectionContents(context.Namespaces, implicitNamespaces);

            // Act
            Route route = context.MapRoute("the_name", "the_url");

            // Assert
            Assert.AreEqual(route, routes["the_name"]);
            Assert.AreEqual("the_area", route.DataTokens["area"]);
            Assert.AreEqual(false, route.DataTokens["UseNamespaceFallback"]);
            CollectionAssert.AreEqual(implicitNamespaces, (string[])route.DataTokens["namespaces"]);
        }

        [TestMethod]
        public void MapRouteWithoutNamespaces() {
            // Arrange
            RouteCollection routes = new RouteCollection();
            AreaRegistrationContext context = new AreaRegistrationContext("the_area", routes);

            // Act
            Route route = context.MapRoute("the_name", "the_url");

            // Assert
            Assert.AreEqual(route, routes["the_name"]);
            Assert.AreEqual("the_area", route.DataTokens["area"]);
            Assert.IsNull(route.DataTokens["namespaces"]);
            Assert.AreEqual(true, route.DataTokens["UseNamespaceFallback"]);
        }

        private static void ReplaceCollectionContents(ICollection<string> collectionToReplace, IEnumerable<string> newContents) {
            collectionToReplace.Clear();
            foreach (string item in newContents) {
                collectionToReplace.Add(item);
            }
        }

    }
}
